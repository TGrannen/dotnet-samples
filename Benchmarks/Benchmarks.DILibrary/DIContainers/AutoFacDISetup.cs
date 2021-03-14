using Autofac;

namespace Benchmarks.DILibrary.DIContainers
{
    public class AutoFacDISetup
    {
        public AutoFacDISetup()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<TestService>().As<ITestService>();

            Container = builder.Build();
        }

        public IContainer Container { get; set; }
    }
}