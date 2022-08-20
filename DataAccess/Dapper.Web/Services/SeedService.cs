namespace Dapper.Web.Services;

public class SeedService
{
    private readonly IConnectionProvider _connectionProvider;
    private readonly ILogger<SeedService> _logger;

    public SeedService(IConnectionProvider connectionProvider, ILogger<SeedService> logger)
    {
        _connectionProvider = connectionProvider;
        _logger = logger;
    }

    public async Task CreateDatabase()
    {
        var path = "./Services/Scripts/DBCreationScript.sql";
        var sql = await File.ReadAllTextAsync(path);
        await _connectionProvider.Connection.ExecuteReaderAsync(sql);
        _logger.LogInformation("Database has been created and seeded");
    }
}