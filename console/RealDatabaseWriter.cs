using System;
using Microsoft.Extensions.Logging;

namespace console
{

    public class RealDatabaseWriter : IDatabaseWriter
    {

        public RealDatabaseWriter(ILogger<RealDatabaseWriter> logger)
        {
            this.Logger = logger;
        }

        private ILogger Logger { get; }

        public void Write()
        {
            this.Logger.LogInformation("real write to database.");
        }

        public void Dispose()
        {
            this.Logger.LogInformation("real dispose.");
        }

    }

}