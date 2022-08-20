namespace Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;

public class SeedService
{
    private readonly IDbConnection _connection;
    private readonly ILogger<SeedService> _logger;

    public SeedService(IDbConnection connection, ILogger<SeedService> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task CreateDatabase()
    {
        var path = "../Dapper.CleanArchitecture.Infrastructure/DataAccess/Seed/Scripts/DBCreationScript.sql";
        var sql = await File.ReadAllTextAsync(path);
        await _connection.ExecuteReaderAsync(sql);
        _logger.LogInformation("Database has been created and seeded");
    }
}