using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECE461Project1
{
    public static class Logger
    {
        public static bool CreateFile()
        {
            string logFile = System.Environment.GetEnvironmentVariable("LOG_FILE");
            if (logFile != null)
            {
                File.Create(logFile);
                return true;
            }
            
            return false;
        }
        public static void WriteLine(string logText, int logLevel)
        {
            string systemLogLevelString = System.Environment.GetEnvironmentVariable("LOG_LEVEL");
            string logFile = System.Environment.GetEnvironmentVariable("LOG_FILE");
            int systemLogLevel = 0;

            if (systemLogLevelString != null) systemLogLevel = int.Parse(systemLogLevelString);

            if (systemLogLevel == 0 || (logLevel == 2 && systemLogLevel == 1) || logFile == null) return;

            File.WriteAllText(logFile, logText + "\n");
        }
    }
}
