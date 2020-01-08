using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using dotenv.net;
using Moq;

namespace aspnetcore.Tests
{

    public class WeatherForecastControllerTest
    {

        [Fact]
        public void GetTest()
        {

            // load configuration (optional)
            string path = AppDomain.CurrentDomain.BaseDirectory.Split("/bin/")[0];
            DotEnv.Config(false, path + "/.env");

            // support dependency injection
            var services = new ServiceCollection();
            Program.AddLogging(services);
            services.AddTransient<Controllers.WeatherForecastController>();
            services.AddSingleton<IWeatherService>(provider =>
            {

                // create a mock IWeatherService
                var service = new Mock<IWeatherService>();
                service.Setup(o => o.GetForecast()).Returns(new WeatherServiceData[] {
                    new WeatherServiceData() {
                        Date = DateTime.Now.AddDays(1),
                        TemperatureC = 0
                    },
                    new WeatherServiceData() {
                        Date = DateTime.Now.AddDays(2),
                        TemperatureC = 10
                    },
                    new WeatherServiceData() {
                        Date = DateTime.Now.AddDays(3),
                        TemperatureC = 20
                    },
                    new WeatherServiceData() {
                        Date = DateTime.Now.AddDays(4),
                        TemperatureC = 30
                    },
                    new WeatherServiceData() {
                        Date = DateTime.Now.AddDays(5),
                        TemperatureC = 40
                    }
                });
                return service.Object;

            });

            // validate that the function enriches the data appropriately
            using (var provider = services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var controller = scope.ServiceProvider.GetService<Controllers.WeatherForecastController>();
                    IEnumerable<WeatherForecast> list = controller.Get();
                    foreach (var forecast in list)
                    {
                        Assert.False(string.IsNullOrEmpty(forecast.Summary));
                    }
                }
            }

        }

    }
}