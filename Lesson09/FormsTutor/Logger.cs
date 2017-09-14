using Splat;
using System.Diagnostics;

namespace FormsTutor
{
    public class Logger : ILogger
    {
        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level) return;

            Debug.WriteLine(message);
        }

        public LogLevel Level { get; set; }
    }
}
