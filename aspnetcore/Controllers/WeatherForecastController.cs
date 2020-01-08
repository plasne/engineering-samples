using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService weatherService)
        {
            this.Logger = logger;
            this.WeatherService = weatherService;
        }

        private readonly ILogger<WeatherForecastController> Logger;

        private IWeatherService WeatherService { get; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var list = new List<WeatherForecast>();
            var rng = new Random();
            var raw = this.WeatherService.GetForecast();
            var len = Math.Min(raw.Count(), 5);
            for (int i = 0; i < len; i++)
            {
                var oldFormat = raw.ElementAt(i);
                var newFormat = new WeatherForecast()
                {
                    Date = oldFormat.Date,
                    TemperatureC = oldFormat.TemperatureC,
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
                list.Add(newFormat);
            }
            return list;
        }

        [HttpGet("version")]
        public string GetVersion()
        {
            return "v1.0.0";
        }

    }
}
