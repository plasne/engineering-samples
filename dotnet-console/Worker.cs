namespace console
{

    public class Worker
    {

        private readonly IDatabaseWriter writer;

        public Worker(IDatabaseWriter writer)
        {
            this.writer = writer;
        }

        public void DoWork()
        {
            this.writer.Write();
        }

    }

}