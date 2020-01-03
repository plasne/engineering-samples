using System;
using Microsoft.Extensions.Logging;

namespace samples
{

    public class FakeDatabaseWriter : IDatabaseWriter
    {

        public FakeDatabaseWriter(ILogger<FakeDatabaseWriter> logger)
        {
            this.Logger = logger;
        }

        private ILogger Logger { get; }

        public void Write()
        {
            this.Logger.LogInformation("fake write to database.");
        }

        public void Dispose()
        {
            this.Logger.LogInformation("fake dispose.");
        }

    }

}