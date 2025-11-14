using Serilog;

namespace API.Extensions;

public static class LoggingExtensions
{
    public static void AddSerilogLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        hostBuilder.UseSerilog();
    }
}