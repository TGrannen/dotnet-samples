using Autofac;
using BenchmarkDotNet.Attributes;
using Benchmarks.DILibrary;
using Benchmarks.DILibrary.DIContainers;
using Microsoft.Extensions.DependencyInjection;
using Ninject;

namespace Benchmarks.ConsoleApp
{
    [MemoryDiagnoser]
    public class DITransientResolverBenchmarks
    {
        private AutoFacDISetup _autoFacDI;
        private NetCoreDISetup _netCoreDi;
        private NinjectDISetup _ninjectDi;

        [GlobalSetup]
        public void Setup()
        {
            _autoFacDI = new AutoFacDISetup();
            _netCoreDi = new NetCoreDISetup();
            _ninjectDi = new NinjectDISetup();
        }

        [Benchmark]
        public void AutoFacDI()
        {
            var service = _autoFacDI.Container.Resolve<ITestService>();
        }

        [Benchmark]
        public void NetCoreDI()
        {
            var service = _netCoreDi.Provider.GetService<ITestService>();
        }

        [Benchmark]
        public void NinjectDI()
        {
            var service = _ninjectDi.Kernel.Get<ITestService>();
        }
    }
}