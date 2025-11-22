using BaseProject.Application.Common.Abstract;
using BaseProject.Application.Common.Requests;
using BaseProject.Application.Common.Results;

namespace BaseProject.Application.Features.User.Queries.GetUsers;

public record GetUsersQuery(BaseFilterRequest Filter)
    : ICacheableRequest<IDataResult<IEnumerable<GetUsersResponse>>>
{
    public string CacheKey => $"User_GetUsers_{Filter.CacheKeyDefaultSuffix}";
    public IEnumerable<string>? Tags => ["User"];
    public TimeSpan? Duration => TimeSpan.FromMinutes(10);
}
