using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency
{
    public static class Logger
    {
        // ...existing code...
        // provide a lazy default factory so static classes can use logging without DI
        private static ILoggerFactory? _factory;
        internal static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    // creates a simple console logger if nobody configured a factory
                    _factory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Debug);
                    });
                }
                return _factory;
            }
            set => _factory = value;
        }

        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
        // ...existing code...
    }
}