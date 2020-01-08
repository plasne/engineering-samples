using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using dotenv.net;

namespace console
{
    class Program
    {

        private static string LogLevel
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("LOG_LEVEL");
            }
        }

        private static bool DisableColors
        {
            get
            {
                string use = System.Environment.GetEnvironmentVariable("DISABLE_COLORS");
                if (string.IsNullOrEmpty(use)) return false;
                var positive = new string[] { "true", "1", "yes" };
                return (positive.Contains(use.ToLower()));
            }
        }

        private static string AppInsightsInstrumentationKey
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            }
        }

        public static void AddLogging(IServiceCollection services)
        {

            // add logging
            services
                .AddLogging(configure =>
                {
                    services.AddSingleton<ILoggerProvider>(p => new tools.SingleLineConsoleLoggerProvider(
                        new tools.SingleLineConsoleLoggerConfiguration()
                        {
                            DisableColors = DisableColors
                        }
                    ));
                })
                .Configure<LoggerFilterOptions>(options =>
                {
                    if (Enum.TryParse(LogLevel, out Microsoft.Extensions.Logging.LogLevel level))
                    {
                        options.MinLevel = level;
                    }
                    else
                    {
                        options.MinLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                    }
                });

        }

        static void Main(string[] args)
        {

            // load configuration (optional)
            var env = tools.FindFile.Up(".env");
            if (!string.IsNullOrEmpty(env)) DotEnv.Config(true, env);

            // setup telemetry
            TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
            telemetryConfig.InstrumentationKey = AppInsightsInstrumentationKey;
            telemetryConfig.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            var telemetryClient = new TelemetryClient(telemetryConfig);

            // support dependency injection
            var services = new ServiceCollection();
            AddLogging(services);
            services.AddTransient<IDatabaseWriter, RealDatabaseWriter>();
            services.AddTransient<Worker>();

            Console.WriteLine("sutff");

            // main loop
            using (var provider = services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                    logger.LogInformation($"LOG_LEVEL = '{Program.LogLevel}'");
                    logger.LogInformation($"DISABLE_COLORS = '{Program.DisableColors}'");
                    logger.LogInformation($"APPINSIGHTS_INSTRUMENTATIONKEY = '{(string.IsNullOrEmpty(Program.AppInsightsInstrumentationKey) ? "(not-set)" : "(set)")}'");

                    // do work
                    var op = telemetryClient.StartOperation(new RequestTelemetry() { Name = "DoWork" });
                    logger.LogDebug("appinsights: StartOperation()");
                    try
                    {
                        var worker = scope.ServiceProvider.GetService<Worker>();
                        worker.DoWork();
                    }
                    catch (Exception e)
                    {
                        logger.LogDebug("appinsights: TrackException()");
                        telemetryClient.TrackException(e);
                    }
                    finally
                    {
                        logger.LogDebug("appinsights: StopOperation()");
                        telemetryClient.StopOperation(op);
                    }

                    // flush (not-blocking, so wait)
                    logger.LogInformation("flushing telemetry (waiting for 5 seconds)...");
                    telemetryClient.Flush();
                    Task.Delay(5000).Wait();
                    logger.LogInformation("flushed.");

                }
            }

            // dispose of the logging provider
            //loggingProvider.Dispose();

        }

    }
}
