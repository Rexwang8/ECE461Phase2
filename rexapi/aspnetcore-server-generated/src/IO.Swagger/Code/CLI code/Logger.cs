using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IO.Swagger.CLI
{
    public class Logger
    {
        //store the initialized values
        private string logFile { get; set; } = "cache/log.txt";
        private int logLevel { get; set; } = 2;

        private string prefix { get; set; } = "LOGGERDEFAULT: ";

        public Logger(int logLevel = 2, string logFile = "cache/log.txt", string prefix = "LOGGERDEFAULT: ")
        {
            this.logLevel = logLevel;
            this.logFile = logFile;
            this.prefix = prefix;
        }

        public void Log(string message, int messagePriority)
        {
            if (this.logLevel == 0 || this.logFile == null) return;
            if(this.logLevel == 1 & messagePriority == 2) return;
            if(messagePriority <= this.logLevel)
            {
                using (StreamWriter sr = new StreamWriter(this.logFile, true))
                {
                    sr.WriteLine(prefix + " " + message);
                }
            }
        }

        //deprecated
        public static void WriteLine(string logText, int level)
        {
            string systemLogLevelString = System.Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "0";
            string logFile = System.Environment.GetEnvironmentVariable("LOG_FILE") ?? "0";
            int systemLogLevel = 0;
            if (systemLogLevelString != null) systemLogLevel = int.Parse(systemLogLevelString);

            if (systemLogLevel == 0 || logFile == null) return;

            if (systemLogLevel == 1 & level == 2) return;

            using (StreamWriter sr = new StreamWriter(logFile, true))
            {
                sr.WriteLine(logText);
            }
        }
    }
}
