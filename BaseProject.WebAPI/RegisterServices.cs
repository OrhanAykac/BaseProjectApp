using BaseProject.Application;
using BaseProject.Domain.Enums;
using BaseProject.Infrastructure;
using BaseProject.WebAPI.Authentication;
using BaseProject.WebAPI.Endpoints;
using BaseProject.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BaseProject.WebAPI;

public static class RegisterServices
{
    public static IServiceCollection Bootstrapper(this IServiceCollection services, IConfiguration configuration, bool isProd)
    {
        services
        .AddHealthChecks();

        services
        .AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        })
        .AddExceptionHandler<GlobalExceptionHandlerMiddleware>()
        .AddApplicationServices(configuration)
        .AddInfrastructureServices(configuration, isProd, AppService.WebApi)
        .AddEndpoints()
        .ConfigureAuthentication(configuration)
        .ConfigureCors(configuration)
        .ConfigureRateLimiting(configuration);

        return services;
    }

    private static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[] allowedOrigins = (configuration["AllowedOrigins"] ?? "").Split(',');

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