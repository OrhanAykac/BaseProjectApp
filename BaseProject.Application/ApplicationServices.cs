using BaseProject.Application.Behaviors.Command;
using BaseProject.Application.Behaviors.Query;
using BaseProject.Application.Mapping.Mapster;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BaseProject.Application;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        MapsterMappingConfig.Configure();

        services.AddValidatorsFromAssembly(typeof(ApplicationServices).Assembly);

        services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandPerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandExceptionHandlingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidatorBehaviour<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(QueryCachingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(QueryPerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(QueryExceptionHandlingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(QueryValidatorBehaviour<,>));
        //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // Add application services here
        return services;
    }
}
