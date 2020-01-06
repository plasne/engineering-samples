using System;
using System.Collections.Generic;
using System.Linq;

namespace aspnetcore
{

    public class RealWeatherService : IWeatherService
    {

        public IEnumerable<WeatherServiceData> GetForecast()
        {
            // 10-day forecast
            var rng = new Random();
            return Enumerable.Range(1, 10).Select(index => new WeatherServiceData
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55)
            })
            .ToArray();
        }

    }

}