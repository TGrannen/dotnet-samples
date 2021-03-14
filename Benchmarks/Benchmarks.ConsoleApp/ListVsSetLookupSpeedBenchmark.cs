using BenchmarkDotNet.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks.ConsoleApp
{
    [MinColumn]
    [MaxColumn]
    [MemoryDiagnoser]
    public class ListVsSetLookupSpeedBenchmark
    {
        private readonly IList _list;
        private readonly int _lookup;
        private readonly ISet<int> _set;

        public ListVsSetLookupSpeedBenchmark()
        {
            _list = Enumerable.Range(0, ListCount).ToList();
            _set = Enumerable.Range(0, ListCount).ToHashSet();
            _lookup = (int)(ListCount * 0.7);
        }

        [Params(1000, 2000000)]
        public int ListCount { get; set; }

        public int NumberOfLookups { get; set; } = 1000;

        [Benchmark]
        public void ListLookup()
        {
            for (int i = 0; i < NumberOfLookups; i++)
            {
                var contains = _list.Contains(_lookup);
            }
        }

        [Benchmark]
        public void SetLookup()
        {
            for (int i = 0; i < NumberOfLookups; i++)
            {
                var contains = _set.Contains(_lookup);
            }
        }
    }
}