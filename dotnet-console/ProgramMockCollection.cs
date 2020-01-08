using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using dotenv.net;

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

            // load configuration (optional)
            var env = tools.FindFile.Up(".env");
            if (!string.IsNullOrEmpty(env)) DotEnv.Config(true, env);

            // support dependency injection
            var services = new ServiceCollection();
            Program.AddLogging(services);
            services.AddSingleton<IDatabaseWriter>(provider =>
            {
                this.fixture.Setup(provider);
                return this.fixture.Writer;
            });
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
