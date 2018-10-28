using System.Collections;
using System.Collections.Generic;

namespace TraceStateParsers.Tests {
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

			// blanks, not found
			yield return new object[] { new TestData(
				traceState:                   " aaa=\"111\" ",
				searchKey:                    "xxx",
				expectedFoundTraceStateEntry: null,
				expectedStrippedTraceState:   "aaa=\"111\"") };

			// blanks, found
			yield return new object[] { new TestData(
				traceState:                   " aaa=\"111\" ",
				searchKey:                    "aaa",
				expectedFoundTraceStateEntry: "\"111\"",
				expectedStrippedTraceState:   "") };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <summary>
	/// Put tests in here, where amount of spaces in input and output are not equal
	/// </summary>
	public class BlankFixingTestDataProvider : IEnumerable<object[]> {
		public IEnumerator<object[]> GetEnumerator() {
			// multi entry, blanks, first pos
			yield return new object[] { new TestData (
				traceState:                   "   aaa=\"111\",    bbb=\"222\",    ccc=\"333\"   ",
				searchKey:                    "aaa",
				expectedFoundTraceStateEntry: "\"111\"",
				expectedStrippedTraceState:   "bbb=\"222\", ccc=\"333\"") };

			// multi entry, blanks, middle pos
			yield return new object[] { new TestData (
				traceState:                   "   aaa=\"111\",    bbb=\"222\",    ccc=\"333\"   ",
				searchKey:                    "bbb",
				expectedFoundTraceStateEntry: "\"222\"",
				expectedStrippedTraceState:   "aaa=\"111\", ccc=\"333\"") };

			// multi entry, blanks, last pos
			yield return new object[] { new TestData (
				traceState:                   "   aaa=\"111\",    bbb=\"222\",    ccc=\"333\"   ",
				searchKey:                    "ccc",
				expectedFoundTraceStateEntry: "\"333\"",
				expectedStrippedTraceState:   "aaa=\"111\", bbb=\"222\"") };

			/// -> Non-blank tests CANNOT be passed by StringSearchTraceState (it simply does not care about other entries), 
			// multi entry, no blanks, first pos
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\",bbb=\"222\",ccc=\"333\"",
				searchKey:                    "aaa",
				expectedFoundTraceStateEntry: "\"111\"",
				expectedStrippedTraceState:   "bbb=\"222\", ccc=\"333\"") };

			// multi entry, no blanks, middle pos
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\",bbb=\"222\",ccc=\"333\"",
				searchKey:                    "bbb",
				expectedFoundTraceStateEntry: "\"222\"",
				expectedStrippedTraceState:   "aaa=\"111\", ccc=\"333\"") };

			// multi entry, no blanks, last pos
			yield return new object[] { new TestData (
				traceState:                   "aaa=\"111\",bbb=\"222\",ccc=\"333\"",
				searchKey:                    "ccc",
				expectedFoundTraceStateEntry: "\"333\"",
				expectedStrippedTraceState:   "aaa=\"111\", bbb=\"222\"") };

		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

}
