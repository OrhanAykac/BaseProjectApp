using BaseProject.Application.Common.Abstract;
using BaseProject.Application.Data;
using BaseProject.Domain.Enums;
using BaseProject.Infrastructure.Cache;
using BaseProject.Infrastructure.Logging.SerilogConfig;
using BaseProject.Infrastructure.Persistance.Contexts;
using BaseProject.Infrastructure.Storage;
using BaseProject.Infrastructure.Storage.Cloudflare.ImageService;
using BaseProject.Infrastructure.Storage.Cloudflare.StorageService;
using BaseProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Net.Http.Headers;

namespace BaseProject.Infrastructure;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction, AppService appService)
    {
        services
            .AddCache(configuration, appService, isProduction)
            .AddLogger(configuration, isProduction, appService)
            .AddDbContext(configuration, isProduction);

        services
            .AddHttpClient("CFImage", h =>
        {
            string token = configuration["CloudFlare:ImageStorage:Token"]!;
            string baseUrl = configuration["CloudFlare:ImageStorage:BaseUploadUrl"]!;

            h.BaseAddress = new Uri(baseUrl);
            h.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        })
            .AddStandardResilienceHandler();


        services.AddScoped<CloudflareImageHelper>();
        services.AddScoped<CloudflareObjectStorageHelper>();
        services.AddScoped<IStorageService, StorageService>();
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        string connStr = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<AppDbContextRO>(options =>
        {
            options.UseNpgsql(connStr)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (!isProduction)
                options.EnableSensitiveDataLogging();

            options.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                warnings.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
        });

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connStr);

            if (!isProduction)
                options.EnableSensitiveDataLogging();

            options.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                warnings.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
        });

        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<IAppDbContextRO, AppDbContextRO>();

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration, AppService appService, bool isProduction)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<CacheService>();

        if (isProduction)
        {
            var multiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = async () => await Task.FromResult(multiplexer));

            services.AddDataProtection()
            .PersistKeysToStackExchangeRedis(multiplexer, $"DataProtectionKey:{appService}");
        }


        services.AddHybridCache(config =>
        {
            config.MaximumPayloadBytes = 1024 * 1024;
            config.MaximumKeyLength = 1024;
            config.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            };
        });


        return services;
    }
}