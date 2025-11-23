namespace BaseProject.Application.Behaviors;

public class ValidatorBehaviour<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IBaseRequest
    where TResponse : IResult
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(message, cancellationToken);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(s => s.ErrorMessage).ToList();
                return (TResponse)Result.Fail(errors);
            }
        }

        return await next(message, cancellationToken);
    }
}

public class DataValidatorBehaviour<TRequest, TResponse>(IValidator<TRequest>? validator = null)
 : IPipelineBehavior<TRequest, IDataResult<TResponse>>
    where TRequest : IRequest<IDataResult<TResponse>>
    where TResponse : IDataResult<TResponse>
{
    public async ValueTask<IDataResult<TResponse>> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, IDataResult<TResponse>> next,
        CancellationToken c)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(message, c);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(s => s.ErrorMessage).ToList();
                return (TResponse)Result.Fail(errors);
            }
        }

        return await next(message, c);
    }
}
