using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using common;

namespace aspnetcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            // add logging
            services.AddSingleLineConsoleLogger();

            // add telemetry
            services.AddHttpContextAccessor();
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryInitializer, CorrelationToTelemetry>();

            // add HttpClient (could be typed or named)
            services
                .AddHttpClient<AppConfig>()
                .ConfigurePrimaryHttpMessageHandler(() => new ProxyHandler());

            // add configuration
            services.AddSingleton<AppConfig, AppConfig>();

            // setup CORS policy
            string[] allowedOrigins = Program.AllowedOrigins;
            if (allowedOrigins != null)
            {
                services.AddCors(options =>
                   {
                       options.AddDefaultPolicy(builder =>
                       {
                           builder.WithOrigins(allowedOrigins)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                           if (!allowedOrigins.Contains("*")) builder.AllowCredentials();
                       });
                   });
            }

            // add the real weather service
            services.AddSingleton<IWeatherResolver, WeatherResolver>();

            // add controllers
            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {

            // load the configuration
            var logger = provider.GetService<ILogger<Startup>>();
            logger.LogInformation("Loading configuration...");
            var config = provider.GetService<AppConfig>();
            config.Apply().Wait();

            // confirm and log the configuration
            config.Optional("HOST_URL", Program.HostUrl);
            config.Optional("ALLOWED_ORIGINS", Program.AllowedOrigins);
            config.Optional("ASPNETCORE_ENVIRONMENT", Program.Environment);
            config.Optional("APPINSIGHTS_INSTRUMENTATIONKEY", hideValue: true);

            // allow the use of a developer exception page
            if (env.IsDevelopment())
            {
                logger.LogDebug("Developer exception page will be used.");
                app.UseDeveloperExceptionPage();
            }

            // enable as appropriate
            //app.UseHttpsRedirection();
            //app.UseDefaultFiles();
            //app.UseStaticFiles();

            // add routing and CORS
            app.UseRouting();
            app.UseCors();

            // enable as appropriate
            //app.UseAuthentication();
            //app.UseAuthorization();

            // add the endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }



    }
}
