using BaseProject.Application.Features.User.Command.AddUser;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseProject.WebAPI.Endpoints.User;

public class AddUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGroup("/users")
            .WithTags("User")
            .MapPost("add-user", [Authorize(AuthenticationSchemes = "Bearer,Basic")]
        async (ISender sender, ClaimsPrincipal claims, [FromBody] AddUserCommandModel req, CancellationToken c) =>
            {
                //Example of getting user info from claims. Can make extentions methods for this if needed often.
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var command = new AddUserCommand(
                    req.Email,
                    req.Password,
                    req.FirstName,
                    req.LastName,
                    req.IsActive,
                    Guid.Parse(userId!));

                var response = await sender.Send(command, c);

                return response;
            });
    }
}

public class AddUserCommandModel
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsActive { get; set; }
}
