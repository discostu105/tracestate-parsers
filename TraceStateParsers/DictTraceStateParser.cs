using System.Collections.Generic;
using System.Linq;

namespace TraceStateParsers {
	/// <summary>
	/// Very simple string split by comma, then put into sorted dictionary
	/// 
	/// find entry and remove it from dictionary
	/// </summary>
	public static class DictTraceStateParser {
		public static void Parse(string traceState, string searchKey, out string foundTraceStateEntry, out string strippedTraceState) {
			if (string.IsNullOrEmpty(traceState)) {
				foundTraceStateEntry = null;
				strippedTraceState = string.Empty;
				return;
			}
			var splitted = traceState.Split(',');
			var dict = new SortedDictionary<string, string>();
			foreach (var entry in splitted) {
				var keyvalue = entry.Split('=');
				dict.Add(keyvalue[0].Trim(), keyvalue[1].Trim());
			}
			foundTraceStateEntry = null;
			if (dict.TryGetValue(searchKey, out string found)) {
				foundTraceStateEntry = dict[searchKey];
				dict.Remove(searchKey);
			}

			strippedTraceState = string.Join(", ", dict.Select(x => $"{x.Key}={x.Value}"));
		}
	}
}