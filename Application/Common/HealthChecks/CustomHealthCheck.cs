using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Application.Common.HealthChecks
{
    public class CustomHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Custom health check logic (e.g., ping an external service)
            bool isServiceHealthy = CheckCustomServiceHealth(); // Custom logic here

            if (isServiceHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Service is running normally."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Service is down or unavailable."));
        }

        private bool CheckCustomServiceHealth()
        {
            // Simulate a health check (e.g., ping an API or check a file)
            return true; // or false if unhealthy
        }
    }
}
