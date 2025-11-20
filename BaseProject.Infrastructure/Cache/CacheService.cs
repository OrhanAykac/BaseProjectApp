using BaseProject.Application.Common.Abstract;
using Microsoft.Extensions.Caching.Hybrid;

namespace BaseProject.Infrastructure.Cache;

public class CacheService(HybridCache _hybridCache) : ICacheService
{
    public async ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = expiration.HasValue
           ? new HybridCacheEntryOptions { Expiration = expiration.Value }
           : null;

        return await _hybridCache.GetOrCreateAsync(key, factory, options, tags, cancellationToken);
    }

    public async ValueTask<T> GetOrCreateAsync<TState, T>(string key, TState state, Func<TState, CancellationToken, ValueTask<T>> factory, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = expiration.HasValue
           ? new HybridCacheEntryOptions { Expiration = expiration.Value }
           : null;

        return await _hybridCache.GetOrCreateAsync(key, state, factory, options, tags, cancellationToken);
    }

    public async ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await _hybridCache.RemoveByTagAsync(tag, cancellationToken);
    }
}