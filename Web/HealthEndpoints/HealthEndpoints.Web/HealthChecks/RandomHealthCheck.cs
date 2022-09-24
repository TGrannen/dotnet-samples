using Bogus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthEndpoints.Web.HealthChecks;

public class RandomHealthCheck : IHealthCheck
{
    private readonly ILogger<RandomHealthCheck> _logger;
    private readonly Faker _faker;

    public RandomHealthCheck(ILogger<RandomHealthCheck> logger)
    {
        _logger = logger;
        _faker = new Faker();
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var result = _faker.Random.Number(0, 5).OrNull(_faker, 0.2f);
            return result switch
            {
                null => throw new Exception("Bad Response"),
                > 4 => Task.FromResult(HealthCheckResult.Degraded("Values are not looking good")),
                _ => Task.FromResult(HealthCheckResult.Healthy())
            };
        }
        catch (Exception exception)
        {
            _logger.LogWarning("Error during check: {Message}", exception.Message);
            return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
        }
    }
}