# Benchmarks

This solution illustrates how to use [BenchmarkDotNet](https://benchmarkdotnet.org/) to run in-depth benchmarking on .NET code.

Concepts that are shown in the solution are listed below.

* Multipe Export Formats (in `Release\net5.0\BenchmarkDotNet.Artifacts\results`)
* Parameterized Tests
* Example DI Benchmarking

## Example

```csharp

using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MyBenchmarks
{
    public class Md5VsSha256
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Md5VsSha256()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
```

### Resulting Benchmarks

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i5-6600K CPU 3.50GHz (Skylake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  DefaultJob : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT

```

| Method |     Mean |    Error |   StdDev |
| ------ | -------: | -------: | -------: |
| Sha256 | 45.68 μs | 0.452 μs | 0.423 μs |
| Md5    | 19.82 μs | 0.216 μs | 0.202 μs |
