namespace BaseProject.Application.Common.Abstract;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get; }
    IEnumerable<string>? Tags { get; }
    TimeSpan? Duration { get; }
}