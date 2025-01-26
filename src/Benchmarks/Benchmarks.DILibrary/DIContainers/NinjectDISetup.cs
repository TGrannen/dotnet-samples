using Ninject;

namespace Benchmarks.DILibrary.DIContainers;

public class NinjectDISetup
{
    public NinjectDISetup()
    {
        Kernel = new StandardKernel();
        Kernel.Load(typeof(NinjectBindings).Assembly);
    }

    public IKernel Kernel { get; set; }
}

public class NinjectBindings : Ninject.Modules.NinjectModule
{
    public override void Load()
    {
        Bind<ITestService>().To<TestService>();
    }
}