using Microsoft.Extensions.Configuration;

namespace Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;

public interface ISeedService
{
    Task CreateDatabase();
}

public class SeedService : ISeedService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedService> _logger;

    public SeedService(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger<SeedService> logger)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task CreateDatabase()
    {
        var basePath = _configuration.GetValue("Seed:BasePath", "./DataAccess/Seed/Scripts/");
        var path = Path.Combine(basePath, "DBCreationScript.sql");
        var sql = await File.ReadAllTextAsync(path);
        using (var conn = _connectionFactory.CreateDbConnection())
        {
            await conn.ExecuteReaderAsync(sql);
        }

        _logger.LogInformation("Database has been created and seeded");
    }
}