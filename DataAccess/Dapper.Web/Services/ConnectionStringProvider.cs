namespace Dapper.Web.Services;

/// <summary>
/// Leveraging this method of retrieving the connection string instead of from configuration due to the TestContainer set up of this project
/// </summary>
public interface IConnectionStringProvider
{
    public string? ConnectionString { get; }
}

class ConnectionStringProvider : IConnectionStringProvider
{
    private readonly IContainerService _service;
    public string? ConnectionString => _service?.Container?.ConnectionString;

    public ConnectionStringProvider(IContainerService service)
    {
        _service = service;
    }
}