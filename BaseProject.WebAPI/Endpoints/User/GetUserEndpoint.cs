using BaseProject.Application.Features.User.Queries.GetUsers;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseProject.WebAPI.Endpoints.User;

public class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGroup("/users")
            .WithTags("User")
            .MapPost("", [Authorize(AuthenticationSchemes = "Bearer,Basic")]
        async (ISender sender, ClaimsPrincipal claims, [FromBody] GetUsersQuery query, CancellationToken c) =>
            {
                //Example of getting user info from claims. Can make extentions methods for this if needed often.
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var response = await sender.Send(query, c);

                return response;
            });
    }
}
