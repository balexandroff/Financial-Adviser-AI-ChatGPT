using Serilog;

namespace FinancialAdviserAI.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(x => x.AddSerilog(GetSerilogLogger(configuration)));

            return services;
        }

        private static Serilog.ILogger GetSerilogLogger(IConfiguration configuration, bool overrideEfLog = false)
        {
            var log = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .Enrich.WithMachineName();

            if (overrideEfLog)
            {
                log = log.MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Debug);
            }

            return log.WriteTo.Debug().CreateLogger();
        }
    }
}
