using FluentValidation;

namespace BaseProject.Application.Behaviors.Query;

public class QueryValidatorBehaviour<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IBaseRequest
    where TResponse : BaseResponse
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(message, cancellationToken);

            if (!result.IsValid)
            {
                return (TResponse)new BaseResponse(false, string.Join(',', result.Errors.Select(s => s.ErrorMessage)));
            }
        }

        return await next(message, cancellationToken);
    }
}
