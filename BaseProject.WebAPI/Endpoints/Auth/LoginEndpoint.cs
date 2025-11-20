using BaseProject.Application.Features.Auth.Commands.Login;
using Mediator;

namespace BaseProject.WebAPI.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGroup("/auth")
            .WithTags("Auth")
            .MapPost("login", async (ISender sender, LoginCommand command) =>
            {
                var response = await sender.Send(command);

                if (response is null)
                    return Results.NotFound("User not found");

                return Results.Ok(response);
            });
    }
}