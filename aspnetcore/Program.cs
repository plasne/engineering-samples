using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using dotenv.net;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace aspnetcore
{

    public class Program
    {

        public static string LogLevel
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("LOG_LEVEL");
            }
        }

        public static bool DisableColors
        {
            get
            {
                string use = System.Environment.GetEnvironmentVariable("DISABLE_COLORS");
                if (string.IsNullOrEmpty(use)) return false;
                var positive = new string[] { "true", "1", "yes" };
                return (positive.Contains(use.ToLower()));
            }
        }

        public static string[] HostUrl
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("HOST_URL");
                if (string.IsNullOrEmpty(s)) return null;
                return s.Split(";");
            }
        }

        public static string[] AllowedOrigins
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
                if (string.IsNullOrEmpty(s)) return null;
                return s.Split(";");
            }
        }

        public static string Environment
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            }
        }

        public static string AppInsightsInstrumentationKey
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

        public static void Main(string[] args)
        {
            DotEnv.Config(false);
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    //logging.AddAzureWebAppDiagnostics();
                })
                .UseStartup<Startup>();
            if (HostUrl != null) builder.UseUrls(HostUrl);
            return builder;
        }

    }
}
