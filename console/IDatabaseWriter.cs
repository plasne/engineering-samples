using System;

namespace console
{

    public interface IDatabaseWriter : IDisposable
    {

        void Write();

    }

}