﻿using System;
using System.Text;

namespace TraceStateParsers {
	/// <summary>
	/// string split by comma, iterate over entries, split each by '='
	/// put non-search results into array and use it to string.join again
	/// </summary>
	public static class StringSplitTraceStateParser {
		public static void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			foundTraceStateEntry = null;
			var splitted = traceState.Split(',');
			var nonSearchedEntriesSb = new StringBuilder(traceState.Length);
			int entryCount = splitted.Length;
			int strippedEntryCount = 0;
			for (int i = 0; i < entryCount; i++) {
				var entry = splitted[i];
				var trimmedEntry = entry.Trim();
				if (foundTraceStateEntry == null && trimmedEntry.StartsWith(searchKey, StringComparison.Ordinal)) {
					foundTraceStateEntry = trimmedEntry.Split('=')[1].Trim();
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
}