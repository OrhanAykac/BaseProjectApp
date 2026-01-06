using BaseProject.Application.Behaviors;
using BaseProject.Application.Mapping.Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace BaseProject.Application;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _)
    {
        MapsterMappingConfig.Configure();

        services.AddValidatorsFromAssembly(typeof(ApplicationServices).Assembly);

        services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehaviour<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }
}
