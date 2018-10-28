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
|               Method |              input |         Mean |      Error |     StdDev |
|--------------------- |------------------- |-------------:|-----------:|-----------:|
|    **StringSplitParser** |              **emtpy** |    **52.812 ns** |  **0.4608 ns** |  **0.4085 ns** |
|           DictParser |              emtpy |     9.457 ns |  0.3694 ns |  0.3456 ns |
|   StringSearchParser |              emtpy |    52.702 ns |  1.0847 ns |  2.4261 ns |
|     RecDescentParser |              emtpy |     2.834 ns |  0.0447 ns |  0.0397 ns |
| FastRecDescentParser |              emtpy |     9.861 ns |  0.2254 ns |  0.3160 ns |
|    **StringSplitParser** |      **many_firstpos** |   **569.023 ns** | **10.9426 ns** | **10.7471 ns** |
|           DictParser |      many_firstpos | 8,328.487 ns | 20.5243 ns | 17.1387 ns |
|   StringSearchParser |      many_firstpos |   316.607 ns |  6.5940 ns | 11.1971 ns |
|     RecDescentParser |      many_firstpos | 1,150.061 ns | 22.4338 ns | 26.7058 ns |
| FastRecDescentParser |      many_firstpos | 1,059.628 ns | 16.6044 ns | 13.8654 ns |
|    **StringSplitParser** |       **many_lastpos** |   **646.834 ns** | **11.9678 ns** | **10.6091 ns** |
|           DictParser |       many_lastpos | 9,205.349 ns | 45.1800 ns | 40.0509 ns |
|   StringSearchParser |       many_lastpos |   370.021 ns |  4.0623 ns |  3.6011 ns |
|     RecDescentParser |       many_lastpos | 1,172.275 ns | 23.1548 ns | 41.1577 ns |
| FastRecDescentParser |       many_lastpos | 1,045.296 ns | 20.4747 ns | 20.1089 ns |
|    **StringSplitParser** |     **multi_firstpos** |   **364.999 ns** |  **5.2163 ns** |  **4.3559 ns** |
|           DictParser |     multi_firstpos | 2,286.776 ns | 45.7449 ns | 65.6059 ns |
|   StringSearchParser |     multi_firstpos |   399.827 ns |  7.9201 ns |  9.7266 ns |
|     RecDescentParser |     multi_firstpos |   542.975 ns |  6.4670 ns |  5.7328 ns |
| FastRecDescentParser |     multi_firstpos |   515.257 ns |  3.9700 ns |  3.7135 ns |
|    **StringSplitParser** |      **multi_lastpos** |   **384.420 ns** |  **7.2502 ns** | **12.3115 ns** |
|           DictParser |      multi_lastpos | 2,140.306 ns | 41.9171 ns | 43.0458 ns |
|   StringSearchParser |      multi_lastpos |   519.219 ns | 10.3507 ns | 19.9423 ns |
|     RecDescentParser |      multi_lastpos |   537.764 ns | 10.6558 ns | 14.5857 ns |
| FastRecDescentParser |      multi_lastpos |   502.456 ns |  4.7637 ns |  4.4560 ns |
|    **StringSplitParser** | **multi_lastpos_long** |   **947.326 ns** |  **3.0243 ns** |  **2.6810 ns** |
|           DictParser | multi_lastpos_long | 3,148.891 ns | 61.5690 ns | 82.1928 ns |
|   StringSearchParser | multi_lastpos_long | 1,244.379 ns | 19.2770 ns | 17.0885 ns |
|     RecDescentParser | multi_lastpos_long | 1,682.071 ns | 23.6840 ns | 22.1540 ns |
| FastRecDescentParser | multi_lastpos_long | 1,509.594 ns | 29.8859 ns | 45.6389 ns |
|    **StringSplitParser** |         **multi_miss** |   **294.175 ns** |  **4.6758 ns** |  **4.3738 ns** |
|           DictParser |         multi_miss | 1,715.938 ns | 33.7224 ns | 43.8487 ns |
|   StringSearchParser |         multi_miss |   144.956 ns |  2.8985 ns |  6.6015 ns |
|     RecDescentParser |         multi_miss |   535.030 ns | 10.6048 ns | 10.8904 ns |
| FastRecDescentParser |         multi_miss |   535.551 ns | 10.5131 ns | 13.2956 ns |
|    **StringSplitParser** |             **single** |   **161.258 ns** |  **1.7321 ns** |  **1.5355 ns** |
|           DictParser |             single |   951.644 ns | 18.2171 ns | 17.0403 ns |
|   StringSearchParser |             single |   206.358 ns |  0.8279 ns |  0.7744 ns |
|     RecDescentParser |             single |   178.980 ns |  1.0601 ns |  0.9397 ns |
| FastRecDescentParser |             single |   160.022 ns |  3.1741 ns |  2.8137 ns |
