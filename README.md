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
|               Method |                input |         Mean |      Error |     StdDev |
|--------------------- |--------------------- |-------------:|-----------:|-----------:|
|    **StringSplitParser** |                **emtpy** |    **52.172 ns** |  **0.6401 ns** |  **0.5674 ns** |
|           DictParser |                emtpy |     9.476 ns |  0.2206 ns |  0.2360 ns |
|   StringSearchParser |                emtpy |    49.513 ns |  0.9575 ns |  0.8956 ns |
|     RecDescentParser |                emtpy |     2.820 ns |  0.0241 ns |  0.0202 ns |
| FastRecDescentParser |                emtpy |     9.928 ns |  0.2249 ns |  0.2844 ns |
|    **StringSplitParser** |       **multi_firstpos** |   **285.023 ns** |  **4.2339 ns** |  **3.9604 ns** |
|           DictParser |       multi_firstpos | 1,927.313 ns | 37.1288 ns | 54.4229 ns |
|   StringSearchParser |       multi_firstpos |   145.048 ns |  1.6561 ns |  1.4681 ns |
|     RecDescentParser |       multi_firstpos |   573.453 ns |  7.1702 ns |  6.3562 ns |
| FastRecDescentParser |       multi_firstpos |   512.426 ns | 10.2724 ns | 11.4177 ns |
|    **StringSplitParser** | **multi_firstpos_long_blanks** |   **988.491 ns** | **19.7951 ns** | **50.0246 ns** |
|           DictParser | multi_firstpos_long_blanks |           NA |         NA |         NA |
|   StringSearchParser | multi_firstpos_long_blanks |   854.733 ns | 16.5753 ns | 14.6936 ns |
|     RecDescentParser | multi_firstpos_long_blanks | 1,668.290 ns | 10.5050 ns |  9.3124 ns |
| FastRecDescentParser | multi_firstpos_long_blanks | 1,448.193 ns | 28.1988 ns | 41.3335 ns |
|    **StringSplitParser** |        **multi_lastpos** |   **390.155 ns** |  **2.0882 ns** |  **1.9533 ns** |
|           DictParser |        multi_lastpos | 2,088.397 ns | 24.0163 ns | 20.0547 ns |
|   StringSearchParser |        multi_lastpos |   524.008 ns | 10.4911 ns | 17.5283 ns |
|     RecDescentParser |        multi_lastpos |   551.520 ns |  7.3240 ns |  6.8509 ns |
| FastRecDescentParser |        multi_lastpos |   538.264 ns | 10.6928 ns | 16.6473 ns |
|    **StringSplitParser** |   **multi_lastpos_long** |   **565.382 ns** |  **7.9333 ns** |  **7.0326 ns** |
|           DictParser |   multi_lastpos_long | 2,449.264 ns | 47.1726 ns | 46.3298 ns |
|   StringSearchParser |   multi_lastpos_long |   702.139 ns | 13.6216 ns | 15.6866 ns |
|     RecDescentParser |   multi_lastpos_long |   906.737 ns |  3.3409 ns |  3.1251 ns |
| FastRecDescentParser |   multi_lastpos_long |   839.809 ns | 16.6002 ns | 20.9940 ns |
|    **StringSplitParser** | **multi_lastpos_long_blanks** |   **905.630 ns** | **18.1223 ns** | **31.2601 ns** |
|           DictParser | multi_lastpos_long_blanks | 2,995.895 ns | 22.1482 ns | 20.7174 ns |
|   StringSearchParser | multi_lastpos_long_blanks | 1,130.306 ns | 22.3865 ns | 19.8451 ns |
|     RecDescentParser | multi_lastpos_long_blanks | 1,753.201 ns | 35.0448 ns | 52.4535 ns |
| FastRecDescentParser | multi_lastpos_long_blanks | 1,386.218 ns | 20.2429 ns | 18.9353 ns |
|    **StringSplitParser** |   **multi_lastpos_miss** |   **281.339 ns** |  **5.2483 ns** |  **4.9092 ns** |
|           DictParser |   multi_lastpos_miss | 1,769.532 ns | 34.8304 ns | 45.2893 ns |
|   StringSearchParser |   multi_lastpos_miss |   140.760 ns |  2.7340 ns |  3.3576 ns |
|     RecDescentParser |   multi_lastpos_miss |   527.197 ns | 10.1297 ns |  9.4753 ns |
| FastRecDescentParser |   multi_lastpos_miss |   519.434 ns | 10.3093 ns | 22.8447 ns |
|    **StringSplitParser** |               **single** |   **165.174 ns** |  **3.0592 ns** |  **2.8616 ns** |
|           DictParser |               single |   917.996 ns | 11.4160 ns |  9.5329 ns |
|   StringSearchParser |               single |   217.148 ns |  2.6367 ns |  2.4663 ns |
|     RecDescentParser |               single |   175.726 ns |  4.5182 ns |  4.4374 ns |
| FastRecDescentParser |               single |   160.637 ns |  2.3593 ns |  2.2068 ns |

