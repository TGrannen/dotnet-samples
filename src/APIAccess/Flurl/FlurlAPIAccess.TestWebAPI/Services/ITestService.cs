namespace FlurlAPIAccess.TestWebAPI.Services;

public interface ITestService
{
    Task<TodoData> GetData(int id);
}

internal class TestService : ITestService
{
    private readonly IOptions<APIConfig> _options;

    public TestService(IOptions<APIConfig> options)
    {
        _options = options;
    }

    public Task<TodoData> GetData(int id)
    {
        return _options.Value.BaseUrl.AppendPathSegment($"todos/{id}").GetJsonAsync<TodoData>();
    }
}

public class TodoData
{
    public int UserId { get; init; }
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool Completed { get; init; }
}