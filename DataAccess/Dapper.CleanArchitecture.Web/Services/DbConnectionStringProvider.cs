using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;

namespace Dapper.CleanArchitecture.Web.Services;

internal class DbConnectionStringProvider : IDbConnectionStringProvider
{
    private readonly IContainerService _containerService;

    public DbConnectionStringProvider(IContainerService containerService)
    {
        _containerService = containerService;
    }

    public string GetConnectionString()
    {
        return _containerService.Container?.ConnectionString;
    }
}