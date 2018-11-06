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
|               Method |              input |         Mean |      Error |     StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|--------------------- |------------------- |-------------:|-----------:|-----------:|------------:|------------:|------------:|--------------------:|
|    **StringSplitParser** |              **emtpy** |    **52.087 ns** |  **0.3905 ns** |  **0.3261 ns** |      **0.0216** |           **-** |           **-** |               **136 B** |
|           DictParser |              emtpy |     9.838 ns |  0.2250 ns |  0.3004 ns |           - |           - |           - |                   - |
|   StringSearchParser |              emtpy |    52.408 ns |  1.0596 ns |  1.0406 ns |      0.0266 |           - |           - |               168 B |
|     RecDescentParser |              emtpy |     2.885 ns |  0.0200 ns |  0.0177 ns |           - |           - |           - |                   - |
| FastRecDescentParser |              emtpy |     9.911 ns |  0.1624 ns |  0.1519 ns |           - |           - |           - |                   - |
|    **StringSplitParser** |      **many_firstpos** |   **541.530 ns** | **10.8440 ns** | **11.6029 ns** |      **0.1707** |           **-** |           **-** |              **1080 B** |
|           DictParser |      many_firstpos | 7,631.660 ns | 62.3363 ns | 55.2595 ns |      0.5188 |           - |           - |              3272 B |
|   StringSearchParser |      many_firstpos |   323.356 ns |  6.4685 ns |  8.1805 ns |      0.0720 |           - |           - |               456 B |
|     RecDescentParser |      many_firstpos | 1,092.079 ns | 11.8969 ns | 11.1284 ns |      0.1049 |           - |           - |               672 B |
| FastRecDescentParser |      many_firstpos | 1,057.344 ns | 12.7302 ns | 11.9078 ns |      0.0896 |           - |           - |               568 B |
|    **StringSplitParser** |       **many_lastpos** |   **675.842 ns** | **13.1627 ns** | **19.2937 ns** |      **0.1707** |           **-** |           **-** |              **1080 B** |
|           DictParser |       many_lastpos | 8,392.330 ns | 76.1775 ns | 71.2564 ns |      0.5188 |           - |           - |              3272 B |
|   StringSearchParser |       many_lastpos |   366.460 ns |  5.2793 ns |  4.9383 ns |      0.0925 |           - |           - |               584 B |
|     RecDescentParser |       many_lastpos | 1,109.195 ns | 21.9613 ns | 21.5690 ns |      0.1049 |           - |           - |               672 B |
| FastRecDescentParser |       many_lastpos | 1,083.954 ns | 20.8697 ns | 19.5216 ns |      0.0896 |           - |           - |               568 B |
|    **StringSplitParser** |     **multi_firstpos** |   **362.120 ns** |  **2.6283 ns** |  **2.1947 ns** |      **0.2031** |           **-** |           **-** |              **1280 B** |
|           DictParser |     multi_firstpos | 2,067.667 ns | 38.8063 ns | 51.8053 ns |      0.3014 |           - |           - |              1912 B |
|   StringSearchParser |     multi_firstpos |   420.374 ns |  7.4590 ns |  6.9771 ns |      0.1636 |           - |           - |              1032 B |
|     RecDescentParser |     multi_firstpos |   562.079 ns |  7.6319 ns |  6.7655 ns |      0.1192 |           - |           - |               752 B |
| FastRecDescentParser |     multi_firstpos |   516.077 ns | 10.2128 ns | 10.9276 ns |      0.1020 |           - |           - |               648 B |
|    **StringSplitParser** |      **multi_lastpos** |   **388.309 ns** |  **4.7480 ns** |  **4.4413 ns** |      **0.2007** |           **-** |           **-** |              **1264 B** |
|           DictParser |      multi_lastpos | 1,913.890 ns | 22.4936 ns | 18.7832 ns |      0.3052 |           - |           - |              1928 B |
|   StringSearchParser |      multi_lastpos |   496.600 ns |  9.5956 ns | 13.1345 ns |      0.1965 |           - |           - |              1240 B |
|     RecDescentParser |      multi_lastpos |   573.820 ns |  5.7078 ns |  5.3390 ns |      0.1192 |           - |           - |               752 B |
| FastRecDescentParser |      multi_lastpos |   518.213 ns | 16.6467 ns | 19.1703 ns |      0.1020 |           - |           - |               648 B |
|    **StringSplitParser** | **multi_lastpos_long** |   **946.675 ns** |  **8.2985 ns** |  **6.9296 ns** |      **0.6914** |      **0.0038** |           **-** |              **4352 B** |
|           DictParser | multi_lastpos_long | 2,973.322 ns | 46.8982 ns | 43.8686 ns |      0.9651 |      0.0076 |           - |              6096 B |
|   StringSearchParser | multi_lastpos_long | 1,220.134 ns | 23.9911 ns | 25.6702 ns |      0.8488 |      0.0019 |           - |              5344 B |
|     RecDescentParser | multi_lastpos_long | 1,766.899 ns |  8.9943 ns |  7.9732 ns |      0.3643 |           - |           - |              2296 B |
| FastRecDescentParser | multi_lastpos_long | 1,484.939 ns | 30.5851 ns | 28.6093 ns |      0.3471 |           - |           - |              2192 B |
|    **StringSplitParser** |         **multi_miss** |   **294.332 ns** |  **5.9053 ns** |  **6.0643 ns** |      **0.1345** |           **-** |           **-** |               **848 B** |
|           DictParser |         multi_miss | 1,757.902 ns | 35.1105 ns | 43.1189 ns |      0.3223 |           - |           - |              2040 B |
|   StringSearchParser |         multi_miss |   139.866 ns |  1.1352 ns |  1.0618 ns |      0.0570 |           - |           - |               360 B |
|     RecDescentParser |         multi_miss |   558.143 ns | 11.0340 ns | 12.7068 ns |      0.1135 |           - |           - |               720 B |
| FastRecDescentParser |         multi_miss |   516.521 ns |  9.9914 ns |  9.3459 ns |      0.0973 |           - |           - |               616 B |
|    **StringSplitParser** |             **single** |   **165.805 ns** |  **3.3145 ns** |  **4.1918 ns** |      **0.0558** |           **-** |           **-** |               **352 B** |
|           DictParser |             single |   806.858 ns | 15.4614 ns | 15.1852 ns |      0.0896 |           - |           - |               568 B |
|   StringSearchParser |             single |   205.768 ns |  2.3911 ns |  1.9967 ns |      0.0455 |           - |           - |               288 B |
|     RecDescentParser |             single |   181.992 ns |  3.5134 ns |  3.1145 ns |      0.0622 |           - |           - |               392 B |
| FastRecDescentParser |             single |   168.468 ns |  3.3429 ns |  3.8497 ns |      0.0455 |           - |           - |               288 B |
