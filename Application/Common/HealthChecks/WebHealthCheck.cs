using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace Application.Common.HealthChecks
{
    public class WebHealthCheck : IHealthCheck
    {
        public WebHealthCheck(string url)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"Invalid URL {url}", nameof(url));
            }

            this.Url = uri;
        }

        public Uri Url { get; }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(this.Url);

            if (response.StatusCode < HttpStatusCode.BadRequest)
            {
                return HealthCheckResult.Healthy("The URL is accessible.");
            }

            return HealthCheckResult.Unhealthy("The URL is inaccessible.");
        }
    }
}
