using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly int DefaultMillisecondsProcessing = 500;

        public PerformanceBehaviour(
            ILogger<TRequest> logger)
        {
            _timer = new Stopwatch();

            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > DefaultMillisecondsProcessing)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "Api Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds){@Request}",
                    requestName,
                    elapsedMilliseconds,
                    request);
            }

            return response;
        }
    }
}
