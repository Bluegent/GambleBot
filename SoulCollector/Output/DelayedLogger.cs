using System;
using System.Collections.Generic;

namespace SoulCollector.Output
{ 
    public class DelayedLogger : ILogger
    {
        private List<string> _messages;

        public void Clear()
        {
            _messages.Clear(); ;
        }
        DelayedLogger()
        {
            _messages = new List<string>();
        }

        public void Log(string message)
        {
            _messages.Add(message);
        }

        public void Flush()
        {

            foreach (string str in _messages)
            {
                Console.Write(str);
            }
            Clear();

        }
    }

}