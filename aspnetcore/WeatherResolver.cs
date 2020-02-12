using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore
{

    public class WeatherResolver : IWeatherResolver
    {

        public Task<IEnumerable<WeatherServiceData>> GetForecast()
        {
            // 10-day forecast
            var rng = new Random();
            return Task.FromResult(
                Enumerable.Range(1, 10).Select(index => new WeatherServiceData
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55)
                })
                .ToArray() as IEnumerable<WeatherServiceData>
            );
        }

    }

}