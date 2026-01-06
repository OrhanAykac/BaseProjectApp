namespace BaseProject.Application.Features.User.Queries.GetUsers;

internal sealed class GetUsersQueryHandler()
    : IRequestHandler<GetUsersQuery, DataResult<List<GetUsersResponse>?>>
{
    public async ValueTask<DataResult<List<GetUsersResponse>?>> Handle(
        GetUsersQuery request, CancellationToken c)
    {
        var users = new List<GetUsersResponse>()
        {
            new()
            {
                UserId = Guid.NewGuid(),
                Email = ""
            }
        };
        return Result.Success(users);
    }
}
