using Dapper.CleanArchitecture.Application.Employees.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(GetAllEmployeesQuery));
        return services;
    }
}