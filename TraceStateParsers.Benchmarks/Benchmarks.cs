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
				traceState: "",
				searchKey: "fw5-29a-3039/dt");
			yield return new Input("single",
				traceState: "fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"",
				searchKey: "fw5-29a-3039/dt");
			yield return new Input("multi_firstpos",
				traceState: "fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\", msappid=\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIJdkjfsijeiSJKFJKSFA==*",
				searchKey: "fw5-29a-3039/dt");
			yield return new Input("multi_lastpos",
				traceState: "msappid=\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIJdkjfsijeiSJKFJKSFA==*, fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"",
				searchKey: "fw5-29a-3039/dt");
			yield return new Input("multi_miss",
				traceState: "msappid=\"abcdef-12334567-fw5-xyzasdf-292929\", nr=*XHFUSAKJFIJdkjfsijeiSJKFJKSFA==*, congo=\"1;FEC354CD6;1;2;727c\"",
				searchKey: "fw5-29a-3039/dt");
			yield return new Input("multi_lastpos_long",
				traceState: "msappid=\"abcdef-12334567-fw5-xyzasdf-292929\",                                 nr=*XHFUSAKJFIxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxJdkjfsijeiSJKFJKSFA==*,                                                                                                    fw5-29a-3039/dt=\"1;FEC354CD6;1;2;727c\"",
				searchKey: "fw5-29a-3039/dt");
			yield return new Input("many_firstpos",
				traceState: "a=b,c=d,e=f,g=h,i=j,k=l,m=n,p=r,s=t,u=v,w=x,y=z",
				searchKey: "a");
			yield return new Input("many_lastpos",
				traceState: "a=b,c=d,e=f,g=h,i=j,k=l,m=n,p=r,s=t,u=v,w=x,y=z",
				searchKey: "y");
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
