using BaseProject.Application;
using BaseProject.Domain.Enums;
using BaseProject.Infrastructure;
using BaseProject.WebAPI.Authentication;
using BaseProject.WebAPI.Endpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BaseProject.WebAPI;

public static class RegisterServices
{
    public static IServiceCollection Bootstrapper(this IServiceCollection services, IConfiguration configuration, bool isProd)
    {
        services
        .AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        })
        .AddOpenApi()
        .AddExceptionHandler<GlobalExceptionHandlerMiddleware>()
        .AddApplicationServices(configuration)
        .AddInfrastructureServices(configuration, isProd, AppService.WebApi)
        .ConfigureApiDocument()
        .ConfigureAuthentication(configuration)
        .ConfigureRateLimiting(configuration);

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        string allowedOrigins = configuration["AllowedOrigins"] ?? "";

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
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

    private static IServiceCollection ConfigureAuthentication(
    this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
                };
            })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
                "Basic", _ => { });

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection ConfigureApiDocument(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BaseProject API",
                Version = "v1",
                Description = "BaseProject Web API Documentation"
            });
            options.CustomSchemaIds(type => type.FullName);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Description = "Basic Authentication",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "basic"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Basic"
                        }
                    },
                    []
                }
            });

        });

        services.AddEndpoints();

        return services;
    }

    private static IServiceCollection ConfigureRateLimiting(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Rate Limiting
        int windowSeconds = configuration.GetValue("RateLimiting:WindowSeconds", 10);
        int permitLimit = configuration.GetValue("RateLimiting:PermitLimit", 5);
        int queueLimit = configuration.GetValue("RateLimiting:QueueLimit", 2);
        int rejectionStatusCode = configuration.GetValue("RateLimiting:RejectionStatusCode", 429);

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(windowSeconds);
                opt.PermitLimit = permitLimit;
                opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = queueLimit;
            });
            options.RejectionStatusCode = rejectionStatusCode;
        });

        return services;
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
