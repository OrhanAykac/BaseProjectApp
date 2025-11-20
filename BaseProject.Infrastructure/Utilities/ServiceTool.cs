using Microsoft.Extensions.DependencyInjection;

namespace BaseProject.Infrastructure.Utilities;
public static class ServiceTool
{
    public static IServiceProvider ServiceProvider { get; set; } = default!;
    public static void CreateInstance(IServiceCollection service)
    {
        ServiceProvider = service.BuildServiceProvider();
    }
}
