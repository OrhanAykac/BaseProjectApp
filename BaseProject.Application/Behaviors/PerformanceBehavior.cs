using System.Diagnostics;

namespace BaseProject.Application.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(Serilog.ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IBaseCommand
{
    private readonly Stopwatch _timer = new();
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Restart();

        var response = await next(message, cancellationToken);

        _timer.Stop();

        if (_timer.ElapsedMilliseconds > 5000)
        {
            var requestName = typeof(TRequest).Name;
            logger.Warning($"Performance alert. Request: {requestName} ({_timer.ElapsedMilliseconds} ms)");
        }

        return response;
    }
}