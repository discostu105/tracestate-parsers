This repository hosts sample-implementations for parsing w3c TraceContext `tracestate` header. Read more about it here: https://github.com/w3c/distributed-tracing

This is an attempt to get a grasp of complexity and performance of different implementation-approaches to parse this HTTP header. What the parsers currently are able to achive:
 * Find given tracestate entry key and return corresponding key.
 * Produce a new tracestate string, which has the entry removed that was searched.

 ## Features currently *not* implemented:
  * Detection of violations of TraceContext spec (e.g. size limits, count limits)
    * some implementations could easily add these checks (`StringSplitParser`, `RecDescentParser`, `FastRecDescentParser`, `DictParser`), while others could not so easily (`StringSearchParser`)
  * Handling of different datatypes (string, binary)
    * just plain strings are currently returned

Take a look at the implementations [here](TraceStateParsers).

The various `input` definitions can be found [here](TraceStateParsers.Benchmarks/Benchmarks.cs#L37).

## Preliminary benchmark results:
``` ini

BenchmarkDotNet=v0.11.1.822-nightly, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i9-8950HK CPU 2.90GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|               Method |                input |        Mean |      Error |      StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|--------------------- |--------------------- |------------:|-----------:|------------:|------------:|------------:|------------:|--------------------:|
|    **StringSplitParser** |                **emtpy** |    **51.83 ns** |  **1.0379 ns** |   **1.6760 ns** |      **0.0216** |           **-** |           **-** |               **136 B** |
|           DictParser |                emtpy |    10.26 ns |  0.2354 ns |   0.3060 ns |           - |           - |           - |                   - |
|   StringSearchParser |                emtpy |    47.78 ns |  0.9759 ns |   1.1985 ns |      0.0266 |           - |           - |               168 B |
|     RecDescentParser |                emtpy |    54.23 ns |  1.1109 ns |   1.7939 ns |      0.0432 |           - |           - |               272 B |
| FastRecDescentParser |                emtpy |    52.54 ns |  1.0762 ns |   2.1984 ns |      0.0266 |           - |           - |               168 B |
|    **StringSplitParser** |       **multi_firstpos** |   **309.63 ns** |  **6.2079 ns** |  **12.1080 ns** |      **0.1459** |           **-** |           **-** |               **920 B** |
|           DictParser |       multi_firstpos | 2,012.27 ns | 39.7903 ns |  61.9486 ns |      0.3376 |           - |           - |              2128 B |
|   StringSearchParser |       multi_firstpos |   155.45 ns |  3.3672 ns |   7.1757 ns |      0.0608 |           - |           - |               384 B |
|     RecDescentParser |       multi_firstpos |   572.31 ns | 11.4553 ns |  18.8213 ns |      0.1202 |           - |           - |               760 B |
| FastRecDescentParser |       multi_firstpos |   501.13 ns |  9.9523 ns |  17.9461 ns |      0.1040 |           - |           - |               656 B |
|    **StringSplitParser** | **multi(...)lanks [26]** |   **964.36 ns** | **18.9412 ns** |  **17.7176 ns** |      **0.6332** |      **0.0038** |           **-** |              **3992 B** |
|           DictParser | multi(...)lanks [26] |          NA |         NA |          NA |           - |           - |           - |                   - |
|   StringSearchParser | multi(...)lanks [26] |   899.24 ns | 20.8068 ns |  36.9840 ns |      0.7334 |      0.0019 |           - |              4616 B |
|     RecDescentParser | multi(...)lanks [26] | 1,532.44 ns | 30.5116 ns |  44.7235 ns |      0.3262 |           - |           - |              2056 B |
| FastRecDescentParser | multi(...)lanks [26] | 1,475.41 ns | 29.2373 ns |  56.3304 ns |      0.3090 |           - |           - |              1952 B |
|    **StringSplitParser** |        **multi_lastpos** |   **390.67 ns** |  **8.4200 ns** |  **11.8036 ns** |      **0.2007** |           **-** |           **-** |              **1264 B** |
|           DictParser |        multi_lastpos | 2,214.86 ns | 41.8619 ns |  44.7917 ns |      0.3128 |           - |           - |              1976 B |
|   StringSearchParser |        multi_lastpos |   503.17 ns | 10.0451 ns |  13.0615 ns |      0.1979 |           - |           - |              1248 B |
|     RecDescentParser |        multi_lastpos |   558.68 ns | 11.5701 ns |  23.8942 ns |      0.1192 |           - |           - |               752 B |
| FastRecDescentParser |        multi_lastpos |   491.97 ns | 10.2272 ns |  18.1788 ns |      0.1020 |           - |           - |               648 B |
|    **StringSplitParser** |   **multi_lastpos_long** |   **599.18 ns** | **11.8750 ns** |  **25.5622 ns** |      **0.4139** |      **0.0010** |           **-** |              **2608 B** |
|           DictParser |   multi_lastpos_long | 2,591.67 ns | 50.4162 ns |  69.0103 ns |      0.5188 |           - |           - |              3280 B |
|   StringSearchParser |   multi_lastpos_long |   785.17 ns | 15.6192 ns |  24.7737 ns |      0.4644 |      0.0010 |           - |              2928 B |
|     RecDescentParser |   multi_lastpos_long |   941.21 ns | 11.6909 ns |  10.9357 ns |      0.2251 |           - |           - |              1424 B |
| FastRecDescentParser |   multi_lastpos_long |   792.69 ns | 15.5997 ns |  26.4894 ns |      0.2089 |           - |           - |              1320 B |
|    **StringSplitParser** | **multi(...)lanks [25]** |   **948.19 ns** | **18.1270 ns** |  **20.1481 ns** |      **0.6237** |      **0.0038** |           **-** |              **3928 B** |
|           DictParser | multi(...)lanks [25] | 3,289.53 ns | 85.4219 ns | 242.3278 ns |      0.7172 |      0.0038 |           - |              4520 B |
|   StringSearchParser | multi(...)lanks [25] | 1,176.90 ns | 22.9023 ns |  28.1261 ns |      0.7572 |      0.0019 |           - |              4768 B |
|     RecDescentParser | multi(...)lanks [25] | 1,503.37 ns | 25.0415 ns |  23.4238 ns |      0.3262 |           - |           - |              2064 B |
| FastRecDescentParser | multi(...)lanks [25] | 1,402.30 ns | 26.8426 ns |  25.1085 ns |      0.3109 |           - |           - |              1960 B |
|    **StringSplitParser** |   **multi_lastpos_miss** |   **295.30 ns** |  **5.7484 ns** |   **5.9032 ns** |      **0.1359** |           **-** |           **-** |               **856 B** |
|           DictParser |   multi_lastpos_miss | 1,725.53 ns | 33.1676 ns |  41.9464 ns |      0.3300 |           - |           - |              2088 B |
|   StringSearchParser |   multi_lastpos_miss |   137.42 ns |  2.7757 ns |   4.4026 ns |      0.0584 |           - |           - |               368 B |
|     RecDescentParser |   multi_lastpos_miss |   557.88 ns | 12.2267 ns |  33.6757 ns |      0.1154 |           - |           - |               728 B |
| FastRecDescentParser |   multi_lastpos_miss |   495.46 ns |  9.8510 ns |  18.9795 ns |      0.0987 |           - |           - |               624 B |
|    **StringSplitParser** |               **single** |   **172.11 ns** |  **3.4642 ns** |   **3.4023 ns** |      **0.0558** |           **-** |           **-** |               **352 B** |
|           DictParser |               single |   953.13 ns | 18.7455 ns |  24.3745 ns |      0.0896 |           - |           - |               568 B |
|   StringSearchParser |               single |   201.29 ns |  3.9686 ns |   5.0191 ns |      0.0455 |           - |           - |               288 B |
|     RecDescentParser |               single |   188.34 ns |  3.7968 ns |   5.0686 ns |      0.0622 |           - |           - |               392 B |
| FastRecDescentParser |               single |   167.02 ns |  3.3435 ns |   4.7951 ns |      0.0455 |           - |           - |               288 B |
