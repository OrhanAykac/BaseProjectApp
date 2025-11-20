using BaseProject.Application.Common.Abstract;

namespace BaseProject.Application.Behaviors.Query;

public class QueryCachingBehavior<TRequest, TResponse>(ICacheService cache) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken c)
    {
        var duration = message.Duration ?? TimeSpan.FromMinutes(10);

        return await cache.GetOrCreateAsync(
            key: message!.CacheKey!,
             (message, next, obj: this),
            static async (state, token) =>
             await state.next(state.message, token),
            tags: message.Tags,
         cancellationToken: c);
    }
}
