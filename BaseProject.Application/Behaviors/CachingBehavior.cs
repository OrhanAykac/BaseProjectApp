namespace BaseProject.Application.Behaviors;

public class CachingBehavior<TRequest, TData>(ICacheService cache)
    : IPipelineBehavior<TRequest, TData>
    where TRequest : ICacheableRequest<TData>
{
    public async ValueTask<TData> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TData> next,
        CancellationToken c)
    {
        var cached = await cache.GetOrCreateAsync(
            message.CacheKey,
            (message, next),
            static async (state, token) =>
            {
                var result = await state.next(state.message, token);
                return result;
            },
            message.Duration,
            message.Tags,
            c);

        return cached;
    }
}