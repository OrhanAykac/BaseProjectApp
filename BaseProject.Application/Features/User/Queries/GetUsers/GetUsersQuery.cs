using BaseProject.Application.Common.Requests;

namespace BaseProject.Application.Features.User.Queries.GetUsers;

public class GetUsersQuery
    : BaseFilterRequest, ICacheableRequest<DataResult<List<GetUsersResponse>?>>
{
    public string CacheKey => $"User_{nameof(GetUsersQuery)}_{this.CacheKeyDefaultSuffix}";
    public IEnumerable<string>? Tags => ["User"];
    public TimeSpan? Duration => TimeSpan.FromMinutes(10);
}
