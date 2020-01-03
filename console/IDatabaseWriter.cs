using System;

namespace samples
{

    public interface IDatabaseWriter : IDisposable
    {

        void Write();

    }

}