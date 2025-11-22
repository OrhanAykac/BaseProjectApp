using BaseProject.Application.Common.Results;

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
            return (TResponse)(IResult)Result.Fail($"An error occurred while processing {typeof(TRequest).Name}");
        }
    }
}
public class DataExceptionHandlingBehavior<TRequest, TResponse, TData>(Serilog.ILogger logger)
    : IPipelineBehavior<TRequest, IDataResult<TData>>
        where TRequest : class, IBaseRequest
        where TResponse : IDataResult<TData>
{
    public async ValueTask<IDataResult<TData>> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, IDataResult<TData>> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while processing {RequestName}", typeof(TRequest).Name);
            return (TResponse)(IResult)Result.Fail($"An error occurred while processing {typeof(TRequest).Name}");
        }
    }
}