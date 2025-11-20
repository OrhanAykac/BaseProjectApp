using BaseProject.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.SystemConsole.Themes;

namespace BaseProject.Infrastructure.Logging.SerilogConfig;

public static class SerilogBuilder
{
    public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration, bool isProduction, AppService appServiceName)
    {

        LogEventLevel level = isProduction ? LogEventLevel.Warning : LogEventLevel.Information;

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("AppService", (byte)appServiceName)
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", level)
            .MinimumLevel.Override("Microsoft.Extensions.Http", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", level)
        .WriteTo.Console(theme: SystemConsoleTheme.Colored)
        .WriteTo.PostgreSQL(
            connectionString: connectionString,
            tableName: "public.logs",
            columnOptions: new Dictionary<string, ColumnWriterBase>
                {
                    { "timestamp", new TimestampColumnWriter() },
                    { "level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar) },
                    { "message", new RenderedMessageColumnWriter() },
                    { "exception", new ExceptionColumnWriter() },
                    { "log_event", new LogEventSerializedColumnWriter() },
                    { "app_service", new SinglePropertyColumnWriter("AppService", PropertyWriteMethod.Raw, NpgsqlTypes.NpgsqlDbType.Smallint) }
                },
            restrictedToMinimumLevel: LogEventLevel.Warning,
            needAutoCreateTable: true)
        .CreateLogger();

        services.AddLogging(cfg =>
        {
            cfg.ClearProviders();
            cfg.AddSerilog(Log.Logger);
        });

        services.AddSingleton(Log.Logger);

        return services;
    }

}
