using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

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

	public class TestDataProvider : IEnumerable<object[]> {
		public IEnumerator<object[]> GetEnumerator() {
			// multi entry, first pos
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\", bbb=\"222\", ccc=\"333\"",
				searchKey:                    "aaa",
				expectedFoundTraceStateEntry: "\"111\"",
				expectedStrippedTraceState:   "bbb=\"222\", ccc=\"333\"") };

			// multi entry, middle pos
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\", bbb=\"222\", ccc=\"333\"",
				searchKey:                    "bbb",
				expectedFoundTraceStateEntry: "\"222\"",
				expectedStrippedTraceState:   "aaa=\"111\", ccc=\"333\"") };

			// multi entry, last pos
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\", bbb=\"222\", ccc=\"333\"",
				searchKey:                    "ccc",
				expectedFoundTraceStateEntry: "\"333\"",
				expectedStrippedTraceState:   "aaa=\"111\", bbb=\"222\"") };

			// multi entry, not found
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\", bbb=\"222\", ccc=\"333\"",
				searchKey:                    "xxx",
				expectedFoundTraceStateEntry: null,
				expectedStrippedTraceState:   "aaa=\"111\", bbb=\"222\", ccc=\"333\"") };

			// empty tracestate
			yield return new object[] { new TestData (
				traceState:                   "",
				searchKey:                    "yyy",
				expectedFoundTraceStateEntry: null,
				expectedStrippedTraceState:   "") };

			//// single entry, found
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\"",
				searchKey:                    "aaa",
				expectedFoundTraceStateEntry: "\"111\"",
				expectedStrippedTraceState:   "") };

			// single entry, not found
			yield return new object[] { new TestData(
				traceState:                   "aaa=\"111\"",
				searchKey:                    "xxx",
				expectedFoundTraceStateEntry: null,
				expectedStrippedTraceState:   "aaa=\"111\"") };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class TraceStateParserTests {
		[Theory]
		[ClassData(typeof(TestDataProvider))]
		public void DictTraceStateParserTest(TestData testData) {
			DictTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		public void StringSearchTraceStateParserTest(TestData testData) {
			StringSearchTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		public void StringSplitTraceStateParserTest(TestData testData) {
			StringSplitTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		public void RecDescentTraceStateParserTest(TestData testData) {
			RecDescentTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		public void FastRecDescentTraceStateParserTest(TestData testData) {
			FastRecDescentTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}
	}
}
