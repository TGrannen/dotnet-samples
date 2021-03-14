using BenchmarkDotNet.Running;

namespace Benchmarks.ConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<ListVsSetLookupSpeedBenchmark>();
            BenchmarkRunner.Run<DITransientResolverBenchmarks>();
        }
    }
}