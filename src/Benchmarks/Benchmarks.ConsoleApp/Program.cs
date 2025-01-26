using BenchmarkDotNet.Running;
using Benchmarks.ConsoleApp;

BenchmarkRunner.Run<ListVsSetLookupSpeedBenchmark>();
BenchmarkRunner.Run<DITransientResolverBenchmarks>();