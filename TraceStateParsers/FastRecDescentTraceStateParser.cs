using System;
using System.Text;

namespace TraceStateParsers {
	/// <summary>
	/// Compared to the RecDescentTraceStateParser implementation, this impl uses c# "unsafe" code to make array index access faster
	/// </summary>
	public class FastRecDescentTraceStateParser {
		private delegate void EntryFoundCallback(ReadOnlySpan<char> key, ReadOnlySpan<char> value, string searchKey, ref ParserContext ctx);

		private unsafe ref struct ParserContext {
			public char* str; // string to parse
			public int len; // str length
			public char c; // current char
			public int pos; // current pos
			public EntryFoundCallback entryFoundCallback;

			public StringBuilder strippedTraceStateSb;
			public ReadOnlySpan<char> foundTraceStateEntry;
			public int strippedEntryCount;
		}

		public static unsafe void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			if (string.IsNullOrEmpty(traceState)) {
				foundTraceStateEntry = null;
				strippedTraceState = string.Empty;
				return;
			}
			
			fixed (char* traceStatePtr = traceState) {
				var ctx = new ParserContext {
					str = traceStatePtr,
					len = traceState.Length,
					entryFoundCallback = new EntryFoundCallback(EntryFound),
					strippedTraceStateSb = new StringBuilder(traceState.Length),
					c = traceStatePtr[0]
				};
				ParseTraceState(ref ctx, searchKey);

				foundTraceStateEntry = ctx.foundTraceStateEntry.Length == 0 ? null : ctx.foundTraceStateEntry.ToString();
				strippedTraceState = ctx.strippedTraceStateSb.ToString();
			}

		}

		private static void EntryFound(ReadOnlySpan<char> key, ReadOnlySpan<char> value, string searchKey, ref ParserContext ctx) {
			if (key.Equals(searchKey, StringComparison.Ordinal)) {
				ctx.foundTraceStateEntry = value.Trim();
			} else {
				if (ctx.strippedEntryCount > 0) {
					ctx.strippedTraceStateSb.Append(", ");
				}
				ctx.strippedTraceStateSb.Append(key.Trim());
				ctx.strippedTraceStateSb.Append("=");
				ctx.strippedTraceStateSb.Append(value.Trim());
				ctx.strippedEntryCount++;
			}
		}

		private unsafe static bool NextChar(ref ParserContext ctx) {
			if (ctx.pos >= ctx.len) {
				return false;
			}
			ctx.c = ctx.str[ctx.pos++];
			return true;
		}

		private static void ParseTraceState(ref ParserContext ctx, string searchKey) {
			do {
				switch (ctx.c) {
					case ' ':
					case ',':
						ctx.pos++;
						break;
					default:
						ParseEntry(ref ctx, searchKey);
						break;
				}
			} while (LookAhead(ref ctx));
		}

		private static void ParseEntry(ref ParserContext ctx, string searchKey) {
			if (ParseKey(ref ctx, out var key)) {
				var value = ParseValue(ref ctx);
				ctx.entryFoundCallback(key, value, searchKey, ref ctx);
			}
		}

		private unsafe static ReadOnlySpan<char> ParseValue(ref ParserContext ctx) {
			int startPos = ctx.pos;
			do {
				switch (ctx.c) {
					case ' ':
					case ',':
						return new ReadOnlySpan<char>(ctx.str + startPos, ctx.pos - startPos - 1);
				}
			} while (NextChar(ref ctx));
			// eol
			return new ReadOnlySpan<char>(ctx.str + startPos, ctx.pos - startPos);
		}

		private unsafe static bool ParseKey(ref ParserContext ctx, out ReadOnlySpan<char> key) {
			int startPos = ctx.pos;
			do {
				switch (ctx.c) {
					case '=':
						key = new ReadOnlySpan<char>(ctx.str + startPos, ctx.pos - startPos - 1);
						return true;
				}
			} while (NextChar(ref ctx));
			key = string.Empty.AsSpan();
			return false;
		}

		private unsafe static bool LookAhead(ref ParserContext ctx) {
			if (ctx.pos >= ctx.len) {
				ctx.c = (char)0;
				return false;
			}
			ctx.c = ctx.str[ctx.pos];
			return true;
		}
	}
}