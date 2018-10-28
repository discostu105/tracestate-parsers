using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace TraceStateParsers.Tests {
	public class TraceStateParserTests {
		[Theory]
		[ClassData(typeof(TestDataProvider))]
		[ClassData(typeof(BlankFixingTestDataProvider))]
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

		// Since StringSearch parser does not fixup blanks, it is hard to assert for the actual strippedTraceState. For now, just test for foundTraceStateEntry in these test-cases
		[Theory]
		[ClassData(typeof(BlankFixingTestDataProvider))]
		public void StringSearchTraceStateParserBlanksTest(TestData testData) {
			StringSearchTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			//Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		[ClassData(typeof(BlankFixingTestDataProvider))]
		public void StringSplitTraceStateParserTest(TestData testData) {
			StringSplitTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		[ClassData(typeof(BlankFixingTestDataProvider))]
		public void RecDescentTraceStateParserTest(TestData testData) {
			RecDescentTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}

		[Theory]
		[ClassData(typeof(TestDataProvider))]
		[ClassData(typeof(BlankFixingTestDataProvider))]
		public void FastRecDescentTraceStateParserTest(TestData testData) {
			FastRecDescentTraceStateParser.Parse(testData.TraceState, testData.SearchKey, out string foundTraceStateEntry, out string strippedTraceState);
			Assert.Equal(testData.ExpectedFoundTraceStateEntry, foundTraceStateEntry);
			Assert.Equal(testData.ExpectedStrippedTraceState, strippedTraceState);
		}
	}
}
