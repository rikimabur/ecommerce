using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using System.Net.NetworkInformation;

namespace Application.Common.HealthChecks
{
    public class PingHealthCheck : IHealthCheck
    {
        public PingHealthCheck(string ipAddress)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress, nameof(ipAddress));

            if (!System.Net.IPAddress.TryParse(ipAddress, out var ip))
            {
                throw new ArgumentException($"Invalid IP address {ipAddress}", nameof(ipAddress));
            }

            this.IPAddress = ip;
        }

        public IPAddress IPAddress { get; }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(IPAddress);

            if (reply.Status == IPStatus.Success)
            {
                return HealthCheckResult.Healthy("The IP address is reachable.");
            }

            return HealthCheckResult.Unhealthy("The IP address is unreachable.");
        }
    }
}
