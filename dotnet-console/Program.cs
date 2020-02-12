using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using dotenv.net;
using Microsoft.Extensions.Hosting;
using common;

namespace console
{
    class Program
    {

        private static string AppInsightsInstrumentationKey
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            }
        }

        static void Main(string[] args)
        {

            // load configuration (optional)
            var env = tools.FindFile.Up(".env");
            if (!string.IsNullOrEmpty(env)) DotEnv.Config(true, env);

            // configure telemetry
            TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
            telemetryConfig.InstrumentationKey = AppInsightsInstrumentationKey;
            telemetryConfig.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

            // create a generic host container
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {

                    // add logging
                    services.AddSingleLineConsoleLogger();

                    // add telemetry
                    services.AddSingleton<TelemetryClient>(provider => new TelemetryClient(telemetryConfig));

                    // add HttpClient (could be typed or named)
                    services
                        .AddHttpClient<AppConfig>()
                        .ConfigurePrimaryHttpMessageHandler(() => new ProxyHandler());

                    // add configuration
                    services.AddSingleton<AppConfig, AppConfig>();

                    // add components and workers
                    services.AddTransient<IDatabaseWriter, RealDatabaseWriter>();
                    services.AddTransient<Worker>();

                }).UseConsoleLifetime();
            var host = builder.Build();

            // main loop
            using (var scope = host.Services.CreateScope())
            {

                // load the configuration
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                logger.LogInformation("Loading configuration...");
                var config = scope.ServiceProvider.GetService<AppConfig>();
                config.Apply().Wait();
                var telemetry = scope.ServiceProvider.GetService<TelemetryClient>();

                // confirm and log the configuration
                config.Optional("APPINSIGHTS_INSTRUMENTATIONKEY", hideValue: true);

                // do work
                var op = telemetry.StartOperation(new RequestTelemetry() { Name = "DoWork" });
                logger.LogDebug("appinsights: StartOperation()");
                try
                {
                    var worker = scope.ServiceProvider.GetService<Worker>();
                    worker.DoWork();
                }
                catch (Exception e)
                {
                    logger.LogDebug("appinsights: TrackException()");
                    telemetry.TrackException(e);
                }
                finally
                {
                    logger.LogDebug("appinsights: StopOperation()");
                    telemetry.StopOperation(op);
                }

                // flush (not-blocking, so wait)
                logger.LogInformation("flushing telemetry (waiting for 5 seconds)...");
                telemetry.Flush();
                Task.Delay(5000).Wait();
                logger.LogInformation("flushed.");

            }

        }




    }
}
