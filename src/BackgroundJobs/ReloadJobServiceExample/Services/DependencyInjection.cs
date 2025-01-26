using ReloadJobServiceExample.Services.Jobs;

namespace ReloadJobServiceExample.Services;

public static class DependencyInjection
{
    public static void AddJobs<T>(this IServiceCollection services)
    {
        var assembly = typeof(T).Assembly;
        var types = assembly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IReloadJob))).ToList();
        var helper = new DependencyInjectionHelper();
        foreach (var type in types)
        {
            services.AddTransient(type);
            var generic = typeof(ReloadJobService<>);
            var constructed = generic.MakeGenericType(type);
            services.AddSingleton(constructed);
            services.AddSingleton(provider => (provider.GetRequiredService(constructed) as IReloadJobService)!);
            helper.AddHostedService(services, constructed, provider => provider.GetRequiredService(constructed));
        }
    }

    /// <summary>
    /// AddHostedService does not have an overload that just takes a Type object. This class serves to provide that ability by
    /// using reflection to call a Generic Typed wrapper method around the provided method
    /// <para>
    /// Source Idea https://stackoverflow.com/a/69121064
    /// </para>
    /// </summary>
    private class DependencyInjectionHelper
    {
        public void AddHostedService(IServiceCollection services, Type type, Func<IServiceProvider, object> factoryMethod)
        {
            var thisClass = typeof(DependencyInjectionHelper);
            var methodInfo = thisClass.GetMethod("AddHostedServiceGen");
            var genericMethod = methodInfo!.MakeGenericMethod(type);
            genericMethod.Invoke(this, new object[] { services, factoryMethod });
        }

        // Needed as a work-around because we can't call the extension method with reflection.
        // ReSharper disable once UnusedMember.Local
        public static IServiceCollection AddHostedServiceGen<THostedService>(IServiceCollection services,
            Func<IServiceProvider, object> factoryMethod)
            where THostedService : class, IHostedService =>
            services.AddHostedService(provider => factoryMethod(provider) as THostedService);
    }
}