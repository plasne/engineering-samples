using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace tools
{

    public class SingleLineConsoleLoggerProvider : ILoggerProvider
    {
        private readonly SingleLineConsoleLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, SingleLineConsoleLogger> _loggers = new ConcurrentDictionary<string, SingleLineConsoleLogger>();

        public SingleLineConsoleLoggerProvider()
        {
            _config = new SingleLineConsoleLoggerConfiguration();
        }

        public SingleLineConsoleLoggerProvider(SingleLineConsoleLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new SingleLineConsoleLogger(name, _config));
        }

        public void Dispose()
        {
            foreach (var logger in _loggers)
            {
                logger.Value.Shutdown();
            }
            _loggers.Clear();
        }
    }

    public class SingleLineConsoleLoggerConfiguration
    {
        public bool DisableColors { get; set; } = false;
    }

    public class SingleLineConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly SingleLineConsoleLoggerConfiguration _config;
        private BlockingCollection<string> Queue { get; set; } = new BlockingCollection<string>();
        private CancellationTokenSource QueueTakeCts { get; set; } = new CancellationTokenSource();
        private Task Dispatcher { get; set; }
        private bool IsAcceptingMessages { get; set; } = true;
        private ManualResetEventSlim IsShutdown { get; set; } = new ManualResetEventSlim(false);

        public SingleLineConsoleLogger(string name, SingleLineConsoleLoggerConfiguration config)
        {
            _name = name;
            _config = config;
            Dispatcher = Task.Run(() =>
            {
                while (IsAcceptingMessages)
                {
                    try
                    {
                        Console.WriteLine(Queue.Take(QueueTakeCts.Token));
                    }
                    catch (OperationCanceledException)
                    {
                        // let the loop end
                    }
                }
                IsShutdown.Set();
            });
        }

        public void Shutdown()
        {
            IsAcceptingMessages = false;
            QueueTakeCts.Cancel();
            IsShutdown.Wait(5000);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            // write the message
            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message))
            {

                // write the message
                var sb = new StringBuilder();
                var logLevelColors = GetLogLevelConsoleColors(logLevel);
                if (!_config.DisableColors && logLevelColors.Foreground != null) sb.Append(logLevelColors.Foreground);
                if (!_config.DisableColors && logLevelColors.Background != null) sb.Append(logLevelColors.Background);
                var logLevelString = GetLogLevelString(logLevel);
                sb.Append(logLevelString);
                if (!_config.DisableColors) sb.Append("\u001b[0m"); // reset
                sb.Append($" {DateTime.UtcNow.ToString()} [{_name}] {message}");
                Queue.Add(sb.ToString());

            }

            // write the exception
            if (exception != null)
            {
                Console.WriteLine(exception.ToString());
            }

        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            if (_config.DisableColors)
            {
                return new ConsoleColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new ConsoleColors("\u001b[37m", "\u001b[41m"); // white on red
                case LogLevel.Error:
                    return new ConsoleColors("\u001b[30m", "\u001b[41m"); // black on red
                case LogLevel.Warning:
                    return new ConsoleColors("\u001b[33m", "\u001b[40m"); // yellow on black
                case LogLevel.Information:
                    return new ConsoleColors("\u001b[32m", "\u001b[40m"); // green on black
                case LogLevel.Debug:
                    return new ConsoleColors("\u001b[37m", "\u001b[40m"); // white on black
                case LogLevel.Trace:
                    return new ConsoleColors("\u001b[37m", "\u001b[40m"); // white on black
                default:
                    return new ConsoleColors(null, null);
            }
        }

        private readonly struct ConsoleColors
        {
            public ConsoleColors(string foreground, string background)
            {
                Foreground = foreground;
                Background = background;
            }

            public string Foreground { get; }

            public string Background { get; }
        }

    }

}