using BaseProject.Application.Features.Auth.Commands.Login;
using BaseProject.WebAPI.Extentions;
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

                return response.ToResult();
            });
    }
}