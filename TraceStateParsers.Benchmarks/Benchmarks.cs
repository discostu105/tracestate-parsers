using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using System;
using System.Collections.Generic;
using TraceStateParsers;

namespace TraceStateParsers.Benchmarks {

	[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
	//[MemoryDiagnoser]
	//[HardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.BranchInstructions)]

	//[ShortRunJob] // will yield quicker results, but less accurate. 
	public class TraceStateParserBenchmarks {

		public struct Input {
			public string Description { get; set; }
			public string TraceState { get; set; }
			public string SearchKey { get; set; }

			public Input(string description, string traceState, string searchKey) : this() {
				Description = description;
				TraceState = traceState;
				SearchKey = searchKey;
			}

			public override string ToString() {
				return Description;
			}
		}

		public IEnumerable<Input> TraceStateParams() {
			yield return new Input("emtpy",
				"", "fw5-29a-3039/dt");
			yield return new Input("single",
				"fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"", "fw5-29a-3039/dt");
			yield return new Input("multi_firstpos",
				"fw5 -29a-3039/dt=\"1;FEC354CD6;1;2;727c\", msappid=\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIJdkjfsijeiSJKFJKSFA==*", "fw5-29a-3039/dt");
			yield return new Input("multi_lastpos",
				"msappid =\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIJdkjfsijeiSJKFJKSFA==*, fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"", "fw5-29a-3039/dt");
			yield return new Input("multi_lastpos_miss",
				"msappid =\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIJdkjfsijeiSJKFJKSFA==*, congo=\"1;FEC354CD6;1;2;727c\"", "fw5-29a-3039/dt");
			yield return new Input("multi_lastpos_long", 
				"msappid=\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxJdkjfsijeiSJKFJKSFA==*, fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"", "fw5-29a-3039/dt");
			yield return new Input("multi_lastpos_long_blanks",
				"                   msappid=\"abcdef-12334567-fw5-xyzasdf-292929\",                                                   nr=*XHFUSAKJFIxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxJdkjfsijeiSJKFJKSFA==*,                                                                                     fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"", "fw5-29a-3039/dt");
			yield return new Input("multi_firstpos_long_blanks",
				"        fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"           msappid=\"abcdef-12334567-fw5-xyzasdf-292929\",                                                   nr=*XHFUSAKJFIxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxJdkjfsijeiSJKFJKSFA==*,                                                                                     ", "fw5-29a-3039/dt");

		}


		[Benchmark]
		[ArgumentsSource(nameof(TraceStateParams))]
		public void StringSplitParser(Input input) {
			StringSplitTraceStateParser.Parse(input.TraceState, input.SearchKey, out string found, out string rest);
		}

		[Benchmark]
		[ArgumentsSource(nameof(TraceStateParams))]
		public void DictParser(Input input) {
			DictTraceStateParser.Parse(input.TraceState, input.SearchKey, out string found, out string rest);
		}

		[Benchmark]
		[ArgumentsSource(nameof(TraceStateParams))]
		public void StringSearchParser(Input input) {
			StringSearchTraceStateParser.Parse(input.TraceState, input.SearchKey, out string found, out string rest);
		}

		[Benchmark]
		[ArgumentsSource(nameof(TraceStateParams))]
		public void RecDescentParser(Input input) {
			RecDescentTraceStateParser.Parse(input.TraceState, input.SearchKey, out string found, out string rest);
		}

		[Benchmark]
		[ArgumentsSource(nameof(TraceStateParams))]
		public void FastRecDescentParser(Input input) {
			FastRecDescentTraceStateParser.Parse(input.TraceState, input.SearchKey, out string found, out string rest);
		}
	}


	public class Benchmarks {
		public static void Main(string[] args) {
			var summary = BenchmarkRunner.Run<TraceStateParserBenchmarks>();
		}
	}
}
