using BaseProject.Application;
using BaseProject.Domain.Enums;
using BaseProject.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BaseProject.WebAPI;

public static class RegisterServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration, bool isProd)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();


        services
        .AddApplicationServices(configuration)
        .AddInfrastructureServices(configuration, isProd, AppService.WebApi);

        return services;
    }

    private static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        ServiceDescriptor[] serviceDescriptors = typeof(IEndpoint).Assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false }
            && type.IsAssignableTo(typeof(IEndpoint))
            )
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (IEndpoint endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }


}

internal sealed class GlobalExceptionHandlerMiddleware(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandlerMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken c)
    {
        logger.LogError(ex, "Unhandled exception occurred");

        // Make sure to set the status code before writing to the response body
        httpContext.Response.StatusCode = ex switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = ex,
            ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Type = ex.GetType().Name,
                Title = "An error occurred",
                Detail = ex.Message,
            }
        })
            .ConfigureAwait(false);

        return true;
    }
}
