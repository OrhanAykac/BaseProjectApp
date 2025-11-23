using BaseProject.WebAPI;
using BaseProject.WebAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

bool isProduction = builder.Environment.IsProduction();

builder.Services
    .Bootstrapper(builder.Configuration, isProduction);

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("AllowAll");

MapEndpoints(app, isProduction);

await app.RunAsync();

static void MapEndpoints(WebApplication app, bool isProduction)
{
    foreach (IEndpoint endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
    {
        endpoint.MapEndpoint(app);
    }

    app.UseHealthChecks("/health");
}