using System;
using System.Text;

namespace TraceStateParsers {
	/// <summary>
	/// This implementation only tries to find search-entry.
	/// it then handles all the cases
	///  - not found
	///  - found at beginning
	///  - found at end
	///  - found in middle
	/// </summary>
	public static class StringSearchTraceStateParser {
		public static void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			if (string.IsNullOrEmpty(traceState)) {
				foundTraceStateEntry = null;
				strippedTraceState = string.Empty;
				return;
			}

			foundTraceStateEntry = null;
			string searchKeyWithEquals = searchKey + "=";
			var strippedTraceStateSb = new StringBuilder(traceState.Length);
			int startPos = traceState.IndexOf(searchKeyWithEquals, StringComparison.Ordinal);
			if (startPos != -1) {
				// found
				strippedTraceStateSb.Append(traceState.Substring(0, startPos));
				int endPos = traceState.IndexOf(",", startPos + 1, StringComparison.Ordinal);
				if (endPos != -1) {
					// found in middle
					foundTraceStateEntry = traceState.Substring(startPos + searchKeyWithEquals.Length, endPos - startPos - searchKeyWithEquals.Length).Trim();
					strippedTraceStateSb.Append(traceState.Substring(endPos + 1).Trim());
				} else {
					// found at end
					foundTraceStateEntry = traceState.Substring(startPos + searchKeyWithEquals.Length).Trim();
				}
				strippedTraceState = strippedTraceStateSb.ToString().Trim();
				if (strippedTraceState.StartsWith(",")) {
					strippedTraceState = strippedTraceState.Substring(1).Trim();
				}
				if (strippedTraceState.EndsWith(",")) {
					strippedTraceState = strippedTraceState.Substring(0, strippedTraceState.Length - 1).Trim();
				}
			} else {
				// not found
				strippedTraceState = traceState.Trim();
			}
		}
	}
}