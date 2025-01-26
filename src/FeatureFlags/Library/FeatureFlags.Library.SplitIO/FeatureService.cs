using FeatureFlags.Library.Core;
using FeatureFlags.Library.Core.Context;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Splitio.Services.Client.Interfaces;

namespace FeatureFlags.Library.SplitIO;

class FeatureService : IFeatureService, IJsonFeatureService
{
    private readonly IServiceProvider _provider;
    private readonly ISplitClient _client;
    private static readonly Converter Converter = new Converter();

    public FeatureService(IServiceProvider provider, ISplitClient client)
    {
        _provider = provider;
        _client = client;
    }

    public async Task<bool> IsEnabledAsync(string key)
    {
        var context = await GetProvider().GetUserAsync();
        return await IsEnabledAsync(key, context);
    }

    public async Task<bool> IsEnabledAsync(string key, bool defaultValue)
    {
        var context = await GetProvider().GetUserAsync();
        return await IsEnabledAsync(key, context, defaultValue);
    }

    public Task<bool> IsEnabledAsync(string key, IFeatureContext context, bool defaultValue = false)
    {
        var config = Converter.Convert(context);
        var treatment = _client.GetTreatment(config.Key, key, config.Attributes);
        return Task.FromResult(treatment == "on");
    }

    public async Task<T> GetConfiguration<T>(string key) where T : class
    {
        var context = await GetProvider().GetUserAsync();
        return await GetConfiguration(key, context, default(T));
    }

    public async Task<T> GetConfiguration<T>(string key, T defaultValue) where T : class
    {
        var context = await GetProvider().GetUserAsync();
        return await GetConfiguration(key, context, defaultValue);
    }

    public Task<T> GetConfiguration<T>(string key, IFeatureContext context, T defaultValue = default) where T : class
    {
        var config = Converter.Convert(context);
        var treatment = _client.GetTreatmentWithConfig(config.Key, key, config.Attributes);
        var result = JsonConvert.DeserializeObject<T>(treatment.Config);
        return Task.FromResult(result ?? defaultValue);
    }

    private IContextProvider GetProvider()
    {
        var contextProvider = _provider.GetService<IContextProvider>();
        if (contextProvider == null)
        {
            throw new MissingContextProviderException();
        }

        return contextProvider;
    }
}