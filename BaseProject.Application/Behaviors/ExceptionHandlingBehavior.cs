namespace BaseProject.Application.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse>(Serilog.ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IBaseRequest
        where TResponse : IResult
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
            return (TResponse)Result.Fail($"An error occurred while processing {typeof(TRequest).Name}");
        }
    }
}
public class DataExceptionHandlingBehavior<TRequest, TResponse>(Serilog.ILogger logger)
    : IPipelineBehavior<TRequest, IDataResult<TResponse>>
        where TRequest : class, IBaseRequest
        where TResponse : IDataResult<TResponse>
{
    public async ValueTask<IDataResult<TResponse>> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, IDataResult<TResponse>> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while processing {RequestName}", typeof(TRequest).Name);
            return (TResponse)Result.Fail($"An error occurred while processing {typeof(TRequest).Name}");
        }
    }
}