namespace TraceStateParsers.Tests {
	public class TestData {
		public string TraceState { get; set; }
		public string SearchKey { get; set; }
		public string ExpectedFoundTraceStateEntry { get; set; }
		public string ExpectedStrippedTraceState { get; set; }

		public TestData(string traceState, string searchKey, string expectedFoundTraceStateEntry, string expectedStrippedTraceState) {
			TraceState = traceState;
			SearchKey = searchKey;
			ExpectedFoundTraceStateEntry = expectedFoundTraceStateEntry;
			ExpectedStrippedTraceState = expectedStrippedTraceState;
		}

	}
}
