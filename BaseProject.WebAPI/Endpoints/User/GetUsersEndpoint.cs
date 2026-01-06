using BaseProject.Application.Features.User.Queries.GetUsers;
using BaseProject.WebAPI.Extentions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.WebAPI.Endpoints.User;

public class GetUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app
            .MapGroup("/users")
            .WithTags("User")
            .MapPost("",
        async (ISender sender, [FromBody] GetUsersQuery query, CancellationToken c) =>
            {
                var response = await sender.Send(query, c);

                return response.ToResult();
            });
    }
}