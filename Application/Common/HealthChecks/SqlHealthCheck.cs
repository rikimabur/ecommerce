using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Application.Common.HealthChecks
{
    public class SqlHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public SqlHealthCheck(string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var sqlConnection = new SqlConnection(_connectionString);

                await sqlConnection.OpenAsync(cancellationToken);

                using var command = sqlConnection.CreateCommand();
                command.CommandText = "SELECT 1";

                await command.ExecuteScalarAsync(cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Connection failed.", exception: ex);
            }
        }
    }
}
