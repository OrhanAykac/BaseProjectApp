namespace BaseProject.Application.Behaviors;

public class ValidatorBehaviour<TMessage, TResponse>(IValidator<TMessage>? validator = null)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : class, IBaseRequest
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(message, cancellationToken);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        return await next(message, cancellationToken);
    }
}