namespace BaseProject.Application.Common.Abstract;

public interface ICacheService
{
    ValueTask<T> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, ValueTask<T>> factory,
        TimeSpan? expiration = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    ValueTask<T> GetOrCreateAsync<TState, T>(string key,
        TState state,
        Func<TState, CancellationToken, ValueTask<T>> factory,
        TimeSpan? expiration = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    ValueTask RemoveByTagAsync(string tag,
        CancellationToken cancellationToken = default);
}
