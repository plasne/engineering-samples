using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using dotenv.net;
using Moq;
using common;
using System.Threading.Tasks;

namespace aspnetcore.Tests
{

    public class WeatherForecastControllerTest
    {

        [Fact]
        public async void GetTest()
        {

            // if you need specific settings, you should set those for the test
            //System.Environment.SetEnvironmentVariable("MY_VAR", "my_value");

            // support dependency injection
            var services = new ServiceCollection();
            services.AddSingleLineConsoleLogger();
            services.AddTransient<Controllers.WeatherForecastController>();
            services.AddSingleton<IWeatherResolver>(provider =>
            {

                // create a mock IWeatherService
                var service = new Mock<IWeatherResolver>();
                service.Setup(o => o.GetForecast()).Returns(
                    Task.FromResult((IEnumerable<WeatherServiceData>)
                        new WeatherServiceData[] {
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
                        }
                    )
                );
                return service.Object;

            });

            // validate that the function enriches the data appropriately
            using (var provider = services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var controller = scope.ServiceProvider.GetService<Controllers.WeatherForecastController>();
                    var result = await controller.Get();
                    Assert.Equal(200, result.StatusCode());
                    var list = result.Body();
                    Assert.Equal(5, list.Count());
                    foreach (var forecast in list)
                    {
                        Assert.False(string.IsNullOrEmpty(forecast.Summary));
                    }
                }
            }

        }

    }
}