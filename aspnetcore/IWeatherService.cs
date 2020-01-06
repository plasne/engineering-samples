using System;
using System.Collections.Generic;

namespace aspnetcore
{

    public interface IWeatherService
    {

        IEnumerable<WeatherServiceData> GetForecast();

    }

    public class WeatherServiceData
    {

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

    }

}