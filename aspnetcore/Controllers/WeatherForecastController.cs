using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnetcore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherResolver weatherResolver)
        {
            this.Logger = logger;
            this.WeatherResolver = weatherResolver;
        }

        private readonly ILogger<WeatherForecastController> Logger;

        private IWeatherResolver WeatherResolver { get; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            var list = new List<WeatherForecast>();

            // fetch the forecast
            var raw = await this.WeatherResolver.GetForecast();
            Logger.LogDebug($"{raw.Count()} forecasts obtained");

            // enrich with summaries
            var rng = new Random();
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

            return Ok(list);
        }

        [HttpGet("version")]
        public string GetVersion()
        {
            return "v1.0.0";
        }

    }
}
