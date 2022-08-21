namespace Dapper.CleanArchitecture.Infrastructure.DataAccess.Seed;

public class SeedService
{
    private readonly IDbConnectionProvider _context;
    private readonly ILogger<SeedService> _logger;

    public SeedService(IDbConnectionProvider context, ILogger<SeedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateDatabase()
    {
        var path = "../Dapper.CleanArchitecture.Infrastructure/DataAccess/Seed/Scripts/DBCreationScript.sql";
        var sql = await File.ReadAllTextAsync(path);
        await _context.Connection.ExecuteReaderAsync(sql);
        _logger.LogInformation("Database has been created and seeded");
    }
}