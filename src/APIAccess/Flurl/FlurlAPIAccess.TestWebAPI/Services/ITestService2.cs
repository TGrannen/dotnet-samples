namespace FlurlAPIAccess.TestWebAPI.Services;

public interface ITestService2
{
    Task<PostData> GetData(int id);
}

internal class TestService2 : ITestService2
{
    private readonly IFlurlRequestProvider _requestProvider;

    public TestService2(IFlurlRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public Task<PostData> GetData(int id)
    {
        return _requestProvider.GetAuthenticatedRequest()
            .AppendPathSegment($"posts/{id}")
            .GetJsonAsync<PostData>();
    }
}

public class PostData
{
    public int Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string PublishedAt { get; init; } = string.Empty;
    public string UpdatedAt { get; init; } = string.Empty;
    public int UserId { get; init; }
}