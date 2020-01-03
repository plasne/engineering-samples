using System;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace samples
{
    public class ProgramFake
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
            services.AddTransient<IDatabaseWriter, FakeDatabaseWriter>();
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
