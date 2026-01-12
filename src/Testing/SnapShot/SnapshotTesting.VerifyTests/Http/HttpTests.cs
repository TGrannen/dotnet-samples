using Microsoft.Extensions.DependencyInjection;
using VerifyTests.Http;

namespace SnapshotTesting.VerifyTests.Http;

public class HttpTests
{
    [Fact]
    public async Task RecordedCalls()
    {
        using var client = new MockHttpClient();

        await client.GetAsync("https://fake/get1");
        await client.GetAsync("https://fake/get2");

        await Verify(client.Calls);
    }

    [Fact]
    public async Task HttpResponse_ViaServiceDependencyInjection()
    {
        var collection = new ServiceCollection();
        collection.AddScoped<MyService>();

        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var httpBuilder = collection.AddHttpClient<MyService>();
        var recording = httpBuilder.AddRecording();

        await using var provider = collection.BuildServiceProvider();
        var myService = provider.GetRequiredService<MyService>();

        var sizeOfResponse = await myService.MethodThatDoesHttp();

        await Verify(new { sizeOfResponse, recording.Sends }).IgnoreMembers("Date");
    }

    public class MyService
    {
        private readonly HttpClient _client;

        // Resolve a HttpClient. All http calls done at any
        // resolved client will be added to `recording.Sends`
        public MyService(HttpClient client)
        {
            _client = client;
        }

        // Some code that does some http calls
        public async Task<int> MethodThatDoesHttp()
        {
            var jsonResult = await _client.GetStringAsync("https://httpbin.org/json");
            var xmlResult = await _client.GetStringAsync("https://httpbin.org/xml");
            return jsonResult.Length + xmlResult.Length;
        }
    }
}