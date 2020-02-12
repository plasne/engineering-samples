using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Xunit;
using Moq;

namespace console
{

    public class ProgramMock
    {

        [Fact]
        public void DoWorkTest()
        {

            // if you need specific settings, you should set those for the test
            //System.Environment.SetEnvironmentVariable("MY_VAR", "my_value");

            // create a generic host container
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IDatabaseWriter>(provider =>
                    {

                        // create a mock DatabaseWriter
                        var writer = new Mock<IDatabaseWriter>();
                        var logger = provider.GetService<ILogger<ProgramMock>>();
                        writer.Setup(o => o.Write()).Callback(() => logger.LogInformation("mock write to database."));
                        writer.Setup(o => o.Dispose()).Callback(() => logger.LogInformation("mock dispose."));
                        return writer.Object;

                    });
                    services.AddTransient<Worker>();
                }).UseConsoleLifetime();
            var host = builder.Build();

            // main loop
            using (var scope = host.Services.CreateScope())
            {
                var worker = scope.ServiceProvider.GetService<Worker>();
                worker.DoWork();
            }

        }

    }
}
