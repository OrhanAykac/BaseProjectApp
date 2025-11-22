using BaseProject.WebAPI;
using BaseProject.WebAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

bool isProduction = builder.Environment.IsProduction();

builder.Services
    .Bootstrapper(builder.Configuration, isProduction);

var app = builder.Build();

MapEndpoints(app, isProduction);

await app.RunAsync();


static void MapEndpoints(WebApplication app, bool isProduction)
{
    if (!isProduction)
        app.MapOpenApi();

    app.UseHealthChecks("/health");

    foreach (IEndpoint endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
    {
        endpoint.MapEndpoint(app);
    }
}