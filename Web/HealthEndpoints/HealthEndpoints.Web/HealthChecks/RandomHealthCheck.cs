using Bogus;
using Bogus.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealthEndpoints.Web.HealthChecks
{
    public class RandomHealthCheck : IHealthCheck
    {
        private readonly ILogger<RandomHealthCheck> _logger;

        public RandomHealthCheck(ILogger<RandomHealthCheck> logger)
        {
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var faker = new Faker();
                var result = faker.Random.Number(0, 5).OrNull(faker, 0.2f);
                if (result == null)
                {
                    throw new Exception("Bad Response");
                }

                if (result > 4)
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Values are not looking good"));
                }
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Error during check: {Message}", exception.Message);
                return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
            }
        }
    }
}