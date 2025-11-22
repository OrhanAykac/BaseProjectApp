using BaseProject.Application.Behaviors;
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

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DataValidatorBehaviour<,,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DataExceptionHandlingBehavior<,,>));

        return services;
    }
}
