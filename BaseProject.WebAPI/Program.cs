using BaseProject.WebAPI;
using BaseProject.WebAPI.Endpoints;
using Swashbuckle.AspNetCore.SwaggerUI;

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

    if (!isProduction)
    {
        //app.MapOpenApi();

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api-document/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.DocExpansion(DocExpansion.None);
            options.RoutePrefix = "api-document";
            options.SwaggerEndpoint("/api-document/v1/swagger.json", "BaseProject API Endpoints");
        });
    }

    app.UseHealthChecks("/health");
}