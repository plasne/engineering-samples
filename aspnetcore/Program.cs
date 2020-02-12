using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging.ApplicationInsights;
using dotenv.net;
using common;

namespace aspnetcore
{

    public class Program
    {

        public static string[] HostUrl
        {
            get
            {
                return AppConfig.GetArrayOnce("HOST_URL");
            }
        }

        public static string[] AllowedOrigins
        {
            get
            {
                return AppConfig.GetArrayOnce("ALLOWED_ORIGINS");
            }
        }

        public static string Environment
        {
            get
            {
                // can inform: env.IsDevelopment()
                return AppConfig.GetStringOnce("ASPNETCORE_ENVIRONMENT");
            }
        }

        public static string AppInsightsInstrumentationKey
        {
            get
            {
                return AppConfig.GetStringOnce("APPINSIGHTS_INSTRUMENTATIONKEY");
            }
        }

        public static void Main(string[] args)
        {
            var env = FindFile.Up(".env");
            if (!string.IsNullOrEmpty(env)) DotEnv.Config(false, env);
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddFilter<ApplicationInsightsLoggerProvider>("", AddSingleLineConsoleLoggerConfiguration.LogLevel);
                })
                .UseStartup<Startup>();
            if (HostUrl != null) builder.UseUrls(HostUrl);
            return builder;
        }

    }
}
