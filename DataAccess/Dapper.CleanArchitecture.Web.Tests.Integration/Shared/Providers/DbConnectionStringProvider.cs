using Dapper.CleanArchitecture.Infrastructure.DataAccess.Interfaces;

namespace Dapper.CleanArchitecture.Web.Tests.Integration.Shared.Providers;

internal class DbConnectionStringProvider : IDbConnectionStringProvider
{
    internal string ConnectionString { get; set; }

    public string GetConnectionString()
    {
        return ConnectionString;
    }
}