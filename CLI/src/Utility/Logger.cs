using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility;
public static class Logger
{
    /* Useless Code 
    public static bool CreateFile()
    {
        
        string logFile = System.Environment.GetEnvironmentVariable("LOG_FILE");
        if (logFile != null)
        {
            File.Create(logFile);
            return true;
        }
        
        return false;
    } */

    public static bool CheckEnvironment()
    {
        if (System.Environment.GetEnvironmentVariable("LOG_FILE") == null)
        {
            Logger.WriteLine("LOG_FILE not set...", 1);
            return false;
        }
        if (System.Environment.GetEnvironmentVariable("LOG_LEVEL") == null)
        {
            Logger.WriteLine("LOG_LEVEL not set...", 1);
            return false;
        }
        if (System.Environment.GetEnvironmentVariable("GITHUB_TOKEN") == null)
        {
            Logger.WriteLine("GITHUB_TOKEN not set...",1);
            return false;
        }

        return true;
    }

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
