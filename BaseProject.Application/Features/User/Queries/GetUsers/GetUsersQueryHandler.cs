using BaseProject.Application.Common.Requests;
using BaseProject.Application.Common.Results;
using BaseProject.Application.Data;

namespace BaseProject.Application.Features.User.Queries.GetUsers;

internal sealed class GetUsersQueryHandler(IAppDbContextRO appDbContext)
    : IRequestHandler<GetUsersQuery, IDataResult<IEnumerable<GetUsersResponse>>>
{
    public async ValueTask<IDataResult<IEnumerable<GetUsersResponse>>> Handle(
        GetUsersQuery request, CancellationToken c)
    {
        var filter = request.Filter;

        var query = appDbContext.Users
            .ApplyOnlyActive(filter);

        if (!string.IsNullOrEmpty(filter.SearchParam))
        {
            query = query.Where(u =>
                u.FirstName.Contains(filter.SearchParam));
        }

        var totalCount = await query.CountAsync(c);

        var users = await query
             .ApplyPaginate(filter)
             .ProjectToType<GetUsersResponse>()
             .OrderByDescending(o => o.CreatedAt)
             .ToListAsync(c);

        return Result.Success(users, filter.PageIndex, filter.PageSize, totalCount);
    }
}
