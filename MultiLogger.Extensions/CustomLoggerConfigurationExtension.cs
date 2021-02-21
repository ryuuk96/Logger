using System;
using System.Collections.Generic;
using System.IO;

using AdvLogger;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomLogger.Extensions
{
    public static class CustomLoggerConfigurationExtension
    {

        static IServiceCollection Services;


        public static void AddCustomLoggerConfiguration ( IConfigurationRoot configuration ) => LoggerConfiguration = configuration;
        public static void AddCustomLoggerOptions<T> ( string property)
            where T: class
        {
            var something = Configure<T>(LoggerConfiguration.GetSection(property));
        }

        public static IServiceCollection AddCustomLoggerConfiguration(this IServiceCollection services)
        {
            foreach (var item in LoggerConfigs)
            {
                var configOption = item.Value.GetType().MakeGenericType();
                services.Configure(item.Key, configOption =>
                {
                    LoggerConfiguration.GetSection(item.Key);
                });
            }
            return services;
        }
    }
}
