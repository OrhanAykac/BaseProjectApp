using BaseProject.Application.Common.Results;
using FluentValidation;

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
                return (TResponse)(IResult)Result.Fail(errors);
            }
        }

        return await next(message, cancellationToken);
    }
}

public class DataValidatorBehaviour<TRequest, TResponse, TData>(IValidator<TRequest>? validator = null)
 : IPipelineBehavior<TRequest, IDataResult<TData>>
    where TRequest : IRequest<IDataResult<TData>>
    where TResponse : IDataResult<TData>
{
    public async ValueTask<IDataResult<TData>> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, IDataResult<TData>> next,
        CancellationToken c)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(message, c);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(s => s.ErrorMessage).ToList();
                return (TResponse)(IResult)Result.Fail(errors);
            }
        }

        return await next(message, c);
    }
}
