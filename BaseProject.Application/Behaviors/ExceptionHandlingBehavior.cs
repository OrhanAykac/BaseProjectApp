using BaseProject.Application.Constants;

namespace BaseProject.Application.Behaviors;

public sealed class ExceptionHandlingBehavior<TRequest, TResponse>(Serilog.ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToArray();
            return CreateFailResult(errors);
        }
        catch (Exception ex)
        {
            logger.Error(ex, Messages.Log.ErrorHandlingMessage, typeof(TRequest).Name);
            return CreateFailResult([Messages.Error.UnexpectedError]);
        }
    }

    private static TResponse CreateFailResult(string[] errors)
    {
        var responseType = typeof(TResponse);

        // Handle plain Result
        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Fail(errors);
        }

        // Handle DataResult<T>
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(DataResult<>))
        {
            var dataType = responseType.GetGenericArguments()[0];
            var failMethod = typeof(Result)
                .GetMethods()
                .First(m => m.Name == "Fail" && m.IsGenericMethod && m.GetParameters().Length == 1 
                    && m.GetParameters()[0].ParameterType == typeof(IReadOnlyList<string>));
            
            var genericFailMethod = failMethod.MakeGenericMethod(dataType);
            return (TResponse)genericFailMethod.Invoke(null, [errors.ToList().AsReadOnly()])!;
        }

        // Handle PagedDataResult<T>
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(PagedDataResult<>))
        {
            var dataType = responseType.GetGenericArguments()[0];
            var failPagedMethod = typeof(Result)
                .GetMethods()
                .First(m => m.Name == "FailPaged" && m.IsGenericMethod && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(IReadOnlyList<string>));
            
            var genericFailPagedMethod = failPagedMethod.MakeGenericMethod(dataType);
            return (TResponse)genericFailPagedMethod.Invoke(null, [errors.ToList().AsReadOnly()])!;
        }

        throw new InvalidOperationException($"Unsupported response type: {responseType.Name}");
    }
}
