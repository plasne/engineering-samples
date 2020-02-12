using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace console
{
    public class ProgramFake
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
                    services.AddTransient<IDatabaseWriter, FakeDatabaseWriter>();
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
