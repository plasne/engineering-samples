using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using dotenv.net;

namespace console
{

    public class ProgramMock
    {

        [Fact]
        public void DoWorkTest()
        {

            // load configuration (optional)
            string path = AppDomain.CurrentDomain.BaseDirectory.Split("/bin/")[0];
            DotEnv.Config(false, path + "/.env");

            // support dependency injection
            var services = new ServiceCollection();
            Program.AddLogging(services);
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
            var provider = services.BuildServiceProvider();

            // do the work
            using (var scope = provider.CreateScope())
            {
                var worker = scope.ServiceProvider.GetService<Worker>();
                worker.DoWork();
            }

        }

    }
}
