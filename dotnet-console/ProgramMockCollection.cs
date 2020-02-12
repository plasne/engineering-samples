using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using dotenv.net;
using Microsoft.Extensions.Hosting;

namespace console
{

    public class MockDatabaseWriterFixture : IDisposable
    {

        public IDatabaseWriter Writer;

        public void Setup(IServiceProvider provider)
        {
            if (this.Writer == null)
            {
                var mock = new Mock<IDatabaseWriter>();
                var logger = provider.GetService<ILogger<ProgramMock>>();
                mock.Setup(o => o.Write()).Callback(() => logger.LogInformation("mock-collection write to database."));
                mock.Setup(o => o.Dispose()).Callback(() => logger.LogInformation("mock-collection dispose."));
                this.Writer = mock.Object;
            }
        }

        public void Dispose()
        {
            if (this.Writer != null) this.Writer.Dispose();
        }
    }

    [CollectionDefinition("MockDatabaseWriter")]
    public class MockDatabaseWriterCollection : ICollectionFixture<MockDatabaseWriterFixture> { }

    [Collection("MockDatabaseWriter")]
    public class ProgramMockCollection
    {

        private readonly MockDatabaseWriterFixture fixture;

        public ProgramMockCollection(MockDatabaseWriterFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void DoWorkTest()
        {

            // if you need specific settings, you should set those for the test
            //System.Environment.SetEnvironmentVariable("MY_VAR", "my_value");

            // create a generic host container
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IDatabaseWriter>(provider =>
                    {
                        this.fixture.Setup(provider);
                        return this.fixture.Writer;
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
