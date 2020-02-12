using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aspnetcore
{

    public interface IWeatherResolver
    {

        Task<IEnumerable<WeatherServiceData>> GetForecast();

    }

    public class WeatherServiceData
    {

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

    }

}