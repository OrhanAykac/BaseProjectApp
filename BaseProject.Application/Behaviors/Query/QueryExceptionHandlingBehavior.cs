namespace BaseProject.Application.Behaviors.Query;
public class QueryExceptionHandlingBehavior<TRequest, TResponse>(Serilog.ILogger logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseRequest
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {

            logger.Error(ex, "An error occurred while processing {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}