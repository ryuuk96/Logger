using System;
using System.Collections.Generic;
using System.IO;

using MultiLogger.Interfaces;
using MultiLogger.Model;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MultiLogger.Extensions
{
    public static class LoggerProcessor
    {
        public static List<IMultiLogger<DetailedLogEntry>> RegisteredLoggers = new List<IMultiLogger<DetailedLogEntry>>();
        public static bool ExcludeBuiltInLogger = false;


        private static IConfigurationRoot configuration = null;
        public static IConfigurationRoot LoggerConfiguration
        {
            get
            {
                if (configuration == null)
                    return new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();
                return configuration;
            }
            private set { configuration = value; }
        }


        /// <summary>
        /// Add custom object logger that logs an object of type T1, and where T2 is a class that implements ICustomObjLogger<typeparamref name="T1"/>
        /// </summary>
        /// <typeparam name="T1">Log model that stores log messages and corresponding properties</typeparam>
        /// <typeparam name="T2">Type that implements ICustomObjLogger<T1></typeparam>
        public static void AddCustomObjLogger<T1, T2> ( this IServiceCollection services) 
            where T2: class, IMultiLogger<T1>
        {
            services.AddSingleton<IMultiLogger<T1>, T2>();
        }
    }
}
