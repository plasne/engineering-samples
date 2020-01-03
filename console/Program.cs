﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using dotenv.net;

namespace samples
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

        private static string AppInsightsKey
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("APPINSIGHTS_KEY");
            }
        }

        public static void AddLogging(IServiceCollection services)
        {

            // add logging
            services
                .AddLogging(configure =>
                {
                    configure.AddProvider(new SingleLineConsoleLoggerProvider(
                        new SingleLineConsoleLoggerConfiguration()
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

            // load configuration
            DotEnv.Config();

            // setup telemetry
            TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
            telemetryConfig.InstrumentationKey = System.Environment.GetEnvironmentVariable("APPINSIGHTS_KEY");
            telemetryConfig.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            var telemetryClient = new TelemetryClient(telemetryConfig);

            // support dependency injection
            var services = new ServiceCollection();
            AddLogging(services);
            services.AddTransient<IDatabaseWriter, RealDatabaseWriter>();
            services.AddTransient<Worker>();
            var provider = services.BuildServiceProvider();

            // main loop
            using (var scope = provider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

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
                logger.LogInformation("flushing (waiting for 5 seconds)...");
                telemetryClient.Flush();
                Task.Delay(5000).Wait();
                logger.LogInformation("flushed.");

            }

        }

    }
}