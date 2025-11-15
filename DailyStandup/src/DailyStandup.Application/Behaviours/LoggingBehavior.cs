using MediatR;
using Serilog;

namespace DailyStandup.Application.Behaviours
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Log.Information("Handling {RequestName} {@Request}", typeof(TRequest).Name, request);
            var response = await next();
            Log.Information("Handled {RequestName} {@Response}", typeof(TRequest).Name, response);
            return response;
        }
    }
}
