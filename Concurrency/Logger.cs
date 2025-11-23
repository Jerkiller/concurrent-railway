using Microsoft.Extensions.Logging;

namespace Concurrency
{
    public static class Logger
    {
        private static readonly DateTime startTime = DateTime.UtcNow;

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

        internal static ILogger CreateLogger<T>() => new PrefixLogger(LoggerFactory.CreateLogger<T>());
        internal static ILogger CreateLogger(string categoryName) => new PrefixLogger(LoggerFactory.CreateLogger(categoryName));
                
        private sealed class PrefixLogger : ILogger
        {
            private readonly ILogger _inner;
            public PrefixLogger(ILogger inner) => _inner = inner;

            public IDisposable? BeginScope<TState>(TState state) => _inner.BeginScope(state);

            public bool IsEnabled(LogLevel logLevel) => _inner.IsEnabled(logLevel);

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                    return;

                int ms = (DateTime.UtcNow - startTime).Milliseconds;
                int s = (int)(DateTime.UtcNow - startTime).TotalSeconds;
                string prefix = $"[{s.ToString("000")}.{ms.ToString("000")}] ";

                // ensure we always pass a formatter to the inner logger; preserve structured state & arguments
                Func<TState, Exception?, string> wrappedFormatter = (s, e) =>
                {
                    string body = formatter != null ? formatter(s, e) : s?.ToString() ?? string.Empty;
                    return prefix + body;
                };

                _inner.Log(logLevel, eventId, state, exception, wrappedFormatter);
            }
        }
    }
}