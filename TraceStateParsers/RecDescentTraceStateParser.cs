using System;
using System.Text;

namespace TraceStateParsers {
	/// <summary>
	/// Uses a recursive descent parser
	/// tries to avoid allocations by using Spans (string_views)
	/// for every detected entry, there is a callback, that can be used to check for the needed entry
	/// entry-counting and size-checks for every entry are possible
	/// </summary>
	public class RecDescentTraceStateParser {
		private StringBuilder strippedTraceStateSb;
		private string foundTraceStateEntry;

		private readonly string searchKey;
		private readonly string traceState;
		private int strippedEntryCount;

		public RecDescentTraceStateParser(string traceState, string searchKey) {
			this.traceState = traceState;
			this.searchKey = searchKey;
			strippedTraceStateSb = new StringBuilder(traceState.Length);
		}

		private void EntryFound(ReadOnlySpan<char> key, ReadOnlySpan<char> value) {
			if (key.Equals(searchKey, StringComparison.Ordinal)) {
				foundTraceStateEntry = value.ToString();
			} else {
				if (strippedEntryCount > 0) {
					strippedTraceStateSb.Append(", ");
				}
				strippedTraceStateSb.Append(key);
				strippedTraceStateSb.Append("=");
				strippedTraceStateSb.Append(value);
				strippedEntryCount++;
			}
		}

		public static void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			var p = new RecDescentTraceStateParser(traceState, searchKey);
			var parser = new RecDescentTraceStateParserImpl(traceState, new RecDescentTraceStateParserImpl.EntryFoundCallback(p.EntryFound));
			parser.ParseTraceState();

			foundTraceStateEntry = p.foundTraceStateEntry;
			strippedTraceState = p.strippedTraceStateSb.ToString();
		}
	}

	public class RecDescentTraceStateParserImpl {
		public delegate void EntryFoundCallback(ReadOnlySpan<char> key, ReadOnlySpan<char> value);

		private readonly string str; // string to parse
		private readonly int len; // str length
		private char c; // current char
		private int pos; // current pos
		private readonly EntryFoundCallback entryFoundCallback;

		public RecDescentTraceStateParserImpl(string str, EntryFoundCallback entryFoundCallback) {
			this.str = str;
			this.entryFoundCallback = entryFoundCallback;
			this.len = str.Length;
		}

		private bool NextChar() {
			if (pos >= len) {
				return false;
			}
			c = str[pos++];
			return true;
		}

		public void ParseTraceState() {
			do {
				switch (c) {
					case ' ':
					case ',':
						pos++;
						break;
					default:
						ParseEntry();
						break;
				}
			} while (LookAhead());
		}

		private void ParseEntry() {
			if (ParseKey(out var key)) {
				var value = ParseValue();
				entryFoundCallback(key, value);
			}
		}

		private ReadOnlySpan<char> ParseValue() {
			int startPos = pos;
			do {
				switch (c) {
					case ' ':
					case ',':
						return str.AsSpan().Slice(startPos, pos - startPos - 1);
				}
			} while (NextChar());
			// eol
			return str.AsSpan().Slice(startPos, pos - startPos);
		}

		private bool ParseKey(out ReadOnlySpan<char> key) {
			int startPos = pos;
			do {
				switch (c) {
					case '=':
						key = str.AsSpan().Slice(startPos, pos - startPos - 1);
						return true;
				}
			} while (NextChar());
			key = string.Empty.AsSpan();
			return false;
		}

		private bool LookAhead() {
			if (pos >= len) {
				c = (char)0;
				return false;
			}
			c = str[pos];
			return true;
		}
	}
}