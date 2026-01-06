namespace BaseProject.Application.Common.Abstract;

public interface ICacheableRequest<TData> : IRequest<TData>
{
    string CacheKey { get; }
    IEnumerable<string>? Tags { get; }
    TimeSpan? Duration { get; }
}