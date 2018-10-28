using System;
using System.Text;

namespace TraceStateParsers {
	/// <summary>
	/// Uses a recursive descent parser
	/// tries to avoid allocations by using Spans (string_views)
	/// for every detected entry, there is a callback, that can be used to check for the needed entry
	/// entry-counting and size-checks for every entry are possible
	/// </summary>
	public class FastRecDescentTraceStateParser {
		private delegate void EntryFoundCallback(ReadOnlySpan<char> key, ReadOnlySpan<char> value, string searchKey, ref Output output);
		
		private ref struct Output {
			public StringBuilder strippedTraceStateSb;
			public ReadOnlySpan<char> foundTraceStateEntry;
			public int strippedEntryCount;
		}

		private unsafe ref struct ParserContext {
			public char* str; // string to parse
			public int len; // str length
			public char c; // current char
			public int pos; // current pos
			public EntryFoundCallback entryFoundCallback;
		}

		private static void EntryFound(ReadOnlySpan<char> key, ReadOnlySpan<char> value, string searchKey, ref Output output) {
			if (key.Equals(searchKey, StringComparison.Ordinal)) {
				output.foundTraceStateEntry = value;
			} else {
				if (output.strippedEntryCount > 0) {
					output.strippedTraceStateSb.Append(", ");
				}
				output.strippedTraceStateSb.Append(key);
				output.strippedTraceStateSb.Append("=");
				output.strippedTraceStateSb.Append(value);
				output.strippedEntryCount++;
			}
		}

		public static unsafe void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			if (string.IsNullOrEmpty(traceState)) {
				foundTraceStateEntry = null;
				strippedTraceState = string.Empty;
			}

			var output = new Output {
				strippedTraceStateSb = new StringBuilder(traceState.Length)
			};
			
			fixed (char* traceStatePtr = traceState) {
				// do some work

				var ctx = new ParserContext {
					str = traceStatePtr,
					len = traceState.Length,
					entryFoundCallback = new EntryFoundCallback(EntryFound)
				};
				ParseTraceState(ref ctx, searchKey, ref output);

				foundTraceStateEntry = output.foundTraceStateEntry.Length == 0 ? null : output.foundTraceStateEntry.ToString();
				strippedTraceState = output.strippedTraceStateSb.ToString();
			}

		}

		private unsafe static bool NextChar(ref ParserContext ctx) {
			if (ctx.pos >= ctx.len) {
				return false;
			}
			ctx.c = ctx.str[ctx.pos++];
			return true;
		}

		private static void ParseTraceState(ref ParserContext ctx, string searchKey, ref Output output) {
			do {
				switch (ctx.c) {
					case ' ':
					case ',':
						ctx.pos++;
						break;
					default:
						ParseEntry(ref ctx, searchKey, ref output);
						break;
				}
			} while (LookAhead(ref ctx));
		}

		private static void ParseEntry(ref ParserContext ctx, string searchKey, ref Output output) {
			if (ParseKey(ref ctx, out var key)) {
				var value = ParseValue(ref ctx);
				ctx.entryFoundCallback(key, value, searchKey, ref output);
			}
		}

		private unsafe static ReadOnlySpan<char> ParseValue(ref ParserContext ctx) {
			int startPos = ctx.pos;
			do {
				switch (ctx.c) {
					case ' ':
					case ',':
						return new ReadOnlySpan<char>(ctx.str + startPos, ctx.pos - startPos - 1);
						//return ctx.str.AsSpan().Slice(startPos, ctx.pos - startPos - 1);
				}
			} while (NextChar(ref ctx));
			// eol
			return new ReadOnlySpan<char>(ctx.str + startPos, ctx.pos - startPos);
			//return ctx.str.AsSpan().Slice(startPos, ctx.pos - startPos);
		}

		private unsafe static bool ParseKey(ref ParserContext ctx, out ReadOnlySpan<char> key) {
			int startPos = ctx.pos;
			do {
				switch (ctx.c) {
					case '=':
						//key = ctx.str.AsSpan().Slice(startPos, ctx.pos - startPos - 1);
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