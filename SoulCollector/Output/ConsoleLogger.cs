using System;

namespace SoulCollector.Output
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            //Console.WriteLine(message);
        }

        public void Flush()
        {
        }
    }
}