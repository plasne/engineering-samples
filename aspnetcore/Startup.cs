using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace aspnetcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // enable as appropriate
            //services.AddHttpContextAccessor();

            // add logging
            Program.AddLogging(services);
            services.AddApplicationInsightsTelemetry();

            // setup CORS policy
            string[] allowedOrigins = Program.AllowedOrigins;
            if (allowedOrigins != null)
            {
                services.AddCors(options =>
                   {
                       options.AddPolicy("AllowedOrigins", builder =>
                       {
                           builder.WithOrigins(allowedOrigins)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                           if (!allowedOrigins.Contains("*")) builder.AllowCredentials();
                       });
                   });
            }

            // add the real weather service
            services.AddSingleton<IWeatherService, RealWeatherService>();

            // add controllers
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {

            // log the parameters
            var logger = provider.GetService<ILogger<Startup>>();
            logger.LogInformation($"LOG_LEVEL = '{Program.LogLevel}'");
            logger.LogInformation($"DISABLE_COLORS = '{Program.DisableColors}'");
            logger.LogInformation($"HOST_URL = '{Program.HostUrl}'");
            string[] allowedOrigins = Program.AllowedOrigins;
            if (allowedOrigins != null) logger.LogInformation($"ALLOWED_ORIGINS = '{string.Join(";", allowedOrigins)}'");
            logger.LogInformation($"ASPNETCORE_ENVIRONMENT = '{Program.Environment}'");
            logger.LogInformation($"APPINSIGHTS_INSTRUMENTATIONKEY = '{(string.IsNullOrEmpty(Program.AppInsightsInstrumentationKey) ? "(not-set)" : "(set)")}'");

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
            app.UseCors("AllowedOrigins");

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
