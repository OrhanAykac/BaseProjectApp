using BaseProject.Application.Features.User.Queries.GetUsers;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.WebAPI.Endpoints.User;

public class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGroup("/users")
            .WithTags("User")
            .MapPost("", [Authorize(AuthenticationSchemes = "Bearer,Basic")]
        async (ISender sender, [FromBody] GetUsersQuery query, CancellationToken c) =>
            {
                var response = await sender.Send(query, c);

                return response;
            });
    }
}