using System;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace console
{
    public class ProgramFake
    {

        [Fact]
        public void DoWorkTest()
        {

            // load configuration (optional)
            var env = tools.FindFile.Up(".env");
            if (!string.IsNullOrEmpty(env)) DotEnv.Config(true, env);

            // support dependency injection
            var services = new ServiceCollection();
            Program.AddLogging(services);
            services.AddTransient<IDatabaseWriter, FakeDatabaseWriter>();
            services.AddTransient<Worker>();

            // do the work
            using (var provider = services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var worker = scope.ServiceProvider.GetService<Worker>();
                    worker.DoWork();
                }
            }

        }

    }
}
