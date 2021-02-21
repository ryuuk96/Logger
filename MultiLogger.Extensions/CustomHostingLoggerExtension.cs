
using MultiLogger.Interfaces;
using MultiLogger.Model;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MultiLogger.Extensions
{
    public static class CustomHostingLoggerExtension
    {
        public static IHostBuilder AddWebhostCustomLogger ( this IHostBuilder host ) =>
            host
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();

                logging.AddCustomLogger();
            });

        public static ILoggingBuilder AddCustomLogger ( this ILoggingBuilder loggingBuilder )
        {
            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<Interfaces.ILogger<DetailedLogEntry>, LoggerFactory>());

            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MSLoggerProvider>());
            return loggingBuilder;
        }
    }
}
