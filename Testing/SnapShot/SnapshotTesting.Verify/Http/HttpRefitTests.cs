using Microsoft.Extensions.DependencyInjection;
using Refit;
using VerifyTests.Http;

namespace SnapshotTesting.Verify;

[UsesVerify]
[Collection("Http Collection")]
public class HttpRefitTests
{
    [Fact]
    public async Task HttpResponse_ViaServiceDependencyInjection()
    {
        var collection = new ServiceCollection();
        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var recording = collection
            .AddRefitClient<IHttpBinApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://httpbin.org"))
            .AddRecording();

        await using var provider = collection.BuildServiceProvider();
        var api = provider.GetRequiredService<IHttpBinApi>();

        await api.GetJson();

        await Verifier.Verify(recording.Sends).IgnoreMembers("Date");
    }

    [Fact]
    public async Task HttpResponse_ViaServiceDependencyInjection_WithMock()
    {
        using var client = new MockHttpClient { BaseAddress = new Uri("https://localhost:1234") };
        var collection = new ServiceCollection();
        collection.AddSingleton(_ => RestService.For<IHttpBinApi>(client));
        await using var provider = collection.BuildServiceProvider();
        var api = provider.GetRequiredService<IHttpBinApi>();

        await api.GetJson();
        await api.Remove(4561);

        await Verifier.Verify(client).IgnoreMembers("Date");
    }

    public interface IHttpBinApi
    {
        [Get("/json")]
        Task<string> GetJson();

        [Delete("/json/{number}")]
        Task Remove(int number);
    }
}