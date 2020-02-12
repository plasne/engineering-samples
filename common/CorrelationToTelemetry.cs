using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace common
{

    public class CorrelationToTelemetry : ITelemetryInitializer
    {

        public CorrelationToTelemetry(IHttpContextAccessor httpContextAccessor = null)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        private IHttpContextAccessor HttpContextAccessor { get; }

        public void Initialize(ITelemetry telemetry)
        {
            // use the correlation header
            string correlation = this.HttpContextAccessor?.HttpContext?.Request?.Headers["x-correlation"];
            if (!string.IsNullOrEmpty(correlation)) telemetry.Context.Operation.Id = correlation;
        }
    }

}