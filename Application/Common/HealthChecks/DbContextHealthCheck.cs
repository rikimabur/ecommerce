using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Application.Common.HealthChecks
{
    public class DbContextHealthCheck<TContext> : IHealthCheck where TContext : DbContext
    {
        public DbContextHealthCheck(TContext context, Func<TContext, bool> condition)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(condition, nameof(condition));

            Context = context;
            Condition = condition;
        }

        public TContext Context { get; }
        public Func<TContext, bool> Condition { get; }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (Condition(Context))
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Query succeeded."));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Query failed."));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Query failed.", ex));
            }
        }
    }
}