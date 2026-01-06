using BaseProject.Domain.Abstract;

namespace BaseProject.Application.Common.Requests;

public abstract class BaseFilterRequest<T> : BaseFilterRequest
{
    public T FilterData { get; set; } = default!;
}
public class BaseFilterRequest
{
    public string SearchParam { get; set; } = default!;
    public bool OnlyActive { get; set; } = false;
    public bool UsePaginate { get; set; } = false;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public int Skip => (PageIndex - 1) * PageSize;
    /// <summary>
    /// Cache key for request please add suffix for unique key example: $"GetUserById:{CacheKeyDefaultPrefix}"
    /// </summary>
    public string CacheKeyDefaultSuffix => $"SearchParam={SearchParam}-OnlyActive={OnlyActive}-UsePaginate={UsePaginate}-PageIndex={PageIndex}-PageSize={PageSize}";
}

public static class BaseFilterRequestExtentions
{
    public static IQueryable<T> ApplyPaginate<T>(this IQueryable<T> query, BaseFilterRequest request)
    {
        if (request.UsePaginate)
            return query.Skip(request.Skip).Take(request.PageSize);

        return query;
    }

    public static IQueryable<T> ApplyOnlyActive<T>(this IQueryable<T> query, BaseFilterRequest request)
        where T : BaseEntity
    {
        if (request.OnlyActive)
            return query.Where(e => e.IsActive);

        return query;
    }
}
