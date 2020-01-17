namespace SoulCollector.GameControllers
{
    using SoulCollector.Output;

    public class Dependencies
    {
        public ILogger Log { get; }

        public Dependencies(ILogger log)
        {
            Log = log;
        }
    }
}