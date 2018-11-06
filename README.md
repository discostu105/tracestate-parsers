This repository hosts sample-implementations for parsing w3c TraceContext `tracestate` header. Read more about it here: https://github.com/w3c/distributed-tracing

This is an attempt to get a grasp of complexity and performance of different implementation-approaches to parse this HTTP header. What the parsers currently are able to achive:
 * Find given tracestate entry key and return corresponding key.
 * Produce a new tracestate string, which has the entry removed that was searched.

 ## Features currently *not* implemented:
  * Detection of violations of TraceContext spec (e.g. size limits, count limits)
    * some implementations could easily add these checks (`StringSplitParser`, `RecDescentParser`, `FastRecDescentParser`, `DictParser`, `FastTokenizerParser`), while others could not so easily (`StringSearchParser`)
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
|               Method |              input |         Mean |       Error |      StdDev |       Median | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|--------------------- |------------------- |-------------:|------------:|------------:|-------------:|------------:|------------:|------------:|--------------------:|
|    **StringSplitParser** |              **emtpy** |     **3.323 ns** |   **0.0457 ns** |   **0.0382 ns** |     **3.329 ns** |           **-** |           **-** |           **-** |                   **-** |
|  FastTokenizerParser |              emtpy |    11.430 ns |   0.2865 ns |   0.2539 ns |    11.334 ns |           - |           - |           - |                   - |
|           DictParser |              emtpy |     9.854 ns |   0.1653 ns |   0.1546 ns |     9.924 ns |           - |           - |           - |                   - |
|   StringSearchParser |              emtpy |     3.004 ns |   0.0485 ns |   0.0405 ns |     3.015 ns |           - |           - |           - |                   - |
|     RecDescentParser |              emtpy |     2.943 ns |   0.0887 ns |   0.1021 ns |     2.953 ns |           - |           - |           - |                   - |
| FastRecDescentParser |              emtpy |     9.940 ns |   0.1895 ns |   0.1680 ns |     9.913 ns |           - |           - |           - |                   - |
|    **StringSplitParser** |      **many_firstpos** |   **543.614 ns** |  **10.8165 ns** |  **14.0644 ns** |   **547.632 ns** |      **0.1707** |           **-** |           **-** |              **1080 B** |
|  FastTokenizerParser |      many_firstpos |   439.297 ns |   8.2214 ns |   7.6903 ns |   443.011 ns |      0.0796 |           - |           - |               504 B |
|           DictParser |      many_firstpos | 8,031.257 ns | 155.9357 ns | 160.1345 ns | 8,038.960 ns |      0.5188 |           - |           - |              3272 B |
|   StringSearchParser |      many_firstpos |   331.962 ns |   6.3836 ns |   5.9713 ns |   333.734 ns |      0.0720 |           - |           - |               456 B |
|     RecDescentParser |      many_firstpos | 1,122.029 ns |  21.9447 ns |  27.7530 ns | 1,117.481 ns |      0.1049 |           - |           - |               672 B |
| FastRecDescentParser |      many_firstpos | 1,110.045 ns |  22.2129 ns |  46.8545 ns | 1,083.862 ns |      0.0896 |           - |           - |               568 B |
|    **StringSplitParser** |       **many_lastpos** |   **683.421 ns** |  **13.0785 ns** |  **14.5367 ns** |   **690.749 ns** |      **0.1707** |           **-** |           **-** |              **1080 B** |
|  FastTokenizerParser |       many_lastpos |   587.243 ns |  14.0411 ns |  12.4471 ns |   582.499 ns |      0.0792 |           - |           - |               504 B |
|           DictParser |       many_lastpos | 8,470.123 ns | 162.3215 ns | 159.4214 ns | 8,519.748 ns |      0.5188 |           - |           - |              3272 B |
|   StringSearchParser |       many_lastpos |   376.675 ns |   7.1448 ns |   7.3372 ns |   375.036 ns |      0.0925 |           - |           - |               584 B |
|     RecDescentParser |       many_lastpos | 1,136.823 ns |   5.2006 ns |   4.8646 ns | 1,137.906 ns |      0.1049 |           - |           - |               672 B |
| FastRecDescentParser |       many_lastpos | 1,083.648 ns |  21.2762 ns |  21.8491 ns | 1,078.840 ns |      0.0896 |           - |           - |               568 B |
|    **StringSplitParser** |     **multi_firstpos** |   **363.497 ns** |   **4.3829 ns** |   **3.8853 ns** |   **364.246 ns** |      **0.2031** |           **-** |           **-** |              **1280 B** |
|  FastTokenizerParser |     multi_firstpos |   204.643 ns |   3.0073 ns |   2.5112 ns |   204.497 ns |      0.0927 |           - |           - |               584 B |
|           DictParser |     multi_firstpos | 2,154.059 ns |  42.3387 ns |  55.0523 ns | 2,159.355 ns |      0.3014 |           - |           - |              1912 B |
|   StringSearchParser |     multi_firstpos |   420.797 ns |   8.1319 ns |   7.6066 ns |   418.751 ns |      0.1636 |           - |           - |              1032 B |
|     RecDescentParser |     multi_firstpos |   573.980 ns |  11.3640 ns |  13.9561 ns |   569.854 ns |      0.1192 |           - |           - |               752 B |
| FastRecDescentParser |     multi_firstpos |   543.322 ns |  10.7534 ns |  10.0588 ns |   542.390 ns |      0.1020 |           - |           - |               648 B |
|    **StringSplitParser** |      **multi_lastpos** |   **392.995 ns** |   **3.3029 ns** |   **3.0895 ns** |   **393.395 ns** |      **0.2007** |           **-** |           **-** |              **1264 B** |
|  FastTokenizerParser |      multi_lastpos |   231.850 ns |   4.6007 ns |   4.7246 ns |   230.011 ns |      0.0925 |           - |           - |               584 B |
|           DictParser |      multi_lastpos | 2,019.799 ns |  39.0226 ns |  41.7537 ns | 2,039.792 ns |      0.3052 |           - |           - |              1928 B |
|   StringSearchParser |      multi_lastpos |   505.975 ns |   7.4934 ns |   7.0093 ns |   507.431 ns |      0.1965 |           - |           - |              1240 B |
|     RecDescentParser |      multi_lastpos |   575.738 ns |  11.3765 ns |  15.1873 ns |   570.869 ns |      0.1192 |           - |           - |               752 B |
| FastRecDescentParser |      multi_lastpos |   523.728 ns |  10.0856 ns |  10.3571 ns |   522.717 ns |      0.1020 |           - |           - |               648 B |
|    **StringSplitParser** | **multi_lastpos_long** |   **981.027 ns** |   **6.4671 ns** |   **6.0493 ns** |   **981.351 ns** |      **0.6905** |      **0.0038** |           **-** |              **4352 B** |
|  FastTokenizerParser | multi_lastpos_long |   527.447 ns |  10.2112 ns |  11.3498 ns |   526.995 ns |      0.3376 |      0.0010 |           - |              2128 B |
|           DictParser | multi_lastpos_long | 3,120.024 ns |   8.3450 ns |   7.8059 ns | 3,120.726 ns |      0.9651 |      0.0076 |           - |              6096 B |
|   StringSearchParser | multi_lastpos_long | 1,265.748 ns |  22.4421 ns |  20.9923 ns | 1,275.494 ns |      0.8488 |      0.0019 |           - |              5344 B |
|     RecDescentParser | multi_lastpos_long | 1,812.775 ns |  36.2049 ns |  33.8661 ns | 1,820.597 ns |      0.3643 |           - |           - |              2296 B |
| FastRecDescentParser | multi_lastpos_long | 1,513.545 ns |  14.4326 ns |  12.0519 ns | 1,515.989 ns |      0.3471 |           - |           - |              2192 B |
|    **StringSplitParser** |         **multi_miss** |   **306.005 ns** |   **5.9000 ns** |   **7.8764 ns** |   **307.615 ns** |      **0.1345** |           **-** |           **-** |               **848 B** |
|  FastTokenizerParser |         multi_miss |   185.856 ns |   1.5458 ns |   1.4459 ns |   186.016 ns |      0.0470 |           - |           - |               296 B |
|           DictParser |         multi_miss | 1,804.888 ns |  35.7199 ns |  53.4639 ns | 1,804.821 ns |      0.3223 |           - |           - |              2040 B |
|   StringSearchParser |         multi_miss |   142.700 ns |   2.7435 ns |   2.8173 ns |   143.377 ns |      0.0570 |           - |           - |               360 B |
|     RecDescentParser |         multi_miss |   557.364 ns |   6.9408 ns |   6.4925 ns |   557.836 ns |      0.1135 |           - |           - |               720 B |
| FastRecDescentParser |         multi_miss |   519.086 ns |  10.0477 ns |  13.0648 ns |   520.112 ns |      0.0973 |           - |           - |               616 B |
|    **StringSplitParser** |             **single** |   **166.044 ns** |   **1.7288 ns** |   **1.6171 ns** |   **165.721 ns** |      **0.0558** |           **-** |           **-** |               **352 B** |
|  FastTokenizerParser |             single |   116.260 ns |   1.6278 ns |   1.3593 ns |   116.029 ns |      0.0355 |           - |           - |               224 B |
|           DictParser |             single |   805.195 ns |   9.8640 ns |   8.7442 ns |   807.922 ns |      0.0896 |           - |           - |               568 B |
|   StringSearchParser |             single |   210.452 ns |   4.1501 ns |   3.8820 ns |   209.881 ns |      0.0455 |           - |           - |               288 B |
|     RecDescentParser |             single |   180.473 ns |   2.9507 ns |   2.6157 ns |   181.073 ns |      0.0622 |           - |           - |               392 B |
| FastRecDescentParser |             single |   167.261 ns |   2.1592 ns |   1.8031 ns |   167.499 ns |      0.0455 |           - |           - |               288 B |
