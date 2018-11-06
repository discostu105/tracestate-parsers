using System;
using System.Collections.Immutable;
using System.Text;

namespace TraceStateParsers {
	/// <summary>
	/// non-allocating string tokenizer. looks for "," characters (IndexOf) in a loop and returns ReadOnlySpan<char>
	/// </summary>
	public static class FastTokenizerTraceStateParser {
		public static void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			if (string.IsNullOrEmpty(traceState)) {
				foundTraceStateEntry = null;
				strippedTraceState = string.Empty;
				return;
			}

			foundTraceStateEntry = null;
			var nonSearchedEntriesSb = new StringBuilder(traceState.Length);
			int strippedEntryCount = 0;

			var entryEnumerator = traceState.AsSpan().Split(',');
			while(entryEnumerator.MoveNext()) {
				var entry = entryEnumerator.Current;

				var trimmedEntry = entry.Trim();
				if (foundTraceStateEntry == null && trimmedEntry.StartsWith(searchKey, StringComparison.Ordinal)) {
					var keyValueEnumerator = trimmedEntry.Split('=');
					keyValueEnumerator.MoveNext(); keyValueEnumerator.MoveNext();
					foundTraceStateEntry = keyValueEnumerator.Current.Trim().ToString();
				} else {
					if (strippedEntryCount > 0) nonSearchedEntriesSb.Append(", ");
					nonSearchedEntriesSb.Append(trimmedEntry);
					strippedEntryCount++;
				}
			}
			if (foundTraceStateEntry == null) {
				strippedTraceState = traceState.Trim();
			} else {
				strippedTraceState = nonSearchedEntriesSb.ToString();
			}
		}
	}
	

	public static class Extensions {
		public static SpanSplitEnumerator Split(this ReadOnlySpan<char> span, char separator) {
			return new SpanSplitEnumerator(span, separator);
		}

		public ref struct SpanSplitEnumerator {
			private readonly char separator;
			private int pos;
			private ReadOnlySpan<char> span;

			public SpanSplitEnumerator(ReadOnlySpan<char> span, char separator) {
				this.separator = separator;
				this.pos = 0;
				this.span = span;
				this.Current = span;
			}

			public bool MoveNext() {
				if (pos == -1) return false;
				int nextpos = span.IndexOf(separator);
				if (nextpos == -1) {
					pos = nextpos;
					Current = span;
					return true;
				}
				Current = span.Slice(0, nextpos);
				span = span.Slice(nextpos + 1);
				pos = 0;
				return true;
			}

			public ReadOnlySpan<char> Current { get; private set; }
		}
	}
}