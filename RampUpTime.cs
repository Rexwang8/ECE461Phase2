using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ECE461Project1
{
    public class RampUpTime : IScoreMetric
    {
        public float metricWeight { get; } = 0.25f;

        public float GetScore(string githubUrl)
        {
            DeleteDownloadedRepo();

            Process gitClone = new Process();
            gitClone.StartInfo.RedirectStandardOutput = true;
            gitClone.StartInfo.RedirectStandardError = true;
            gitClone.StartInfo.FileName = "git";
            gitClone.StartInfo.Arguments = "clone " + githubUrl + " ./githubRepoClone";
            gitClone.Start();

            //Process gitClone = Process.Start("git", "clone " + githubUrl + " ./githubRepoClone");
            while (!gitClone.HasExited) ;

            Console.WriteLine(CountNewLines(GetFilesPaths("./githubRepoClone")));

            DeleteDownloadedRepo();
            return 1;
        }

        List<string> GetFilesPaths(string parentDir)
        {
            List<string> filePaths = new List<string>();
            filePaths.AddRange(Directory.GetFiles(parentDir));
            foreach (string folder in Directory.GetDirectories(parentDir))
            {
                filePaths.AddRange(GetFilesPaths(folder));
            }

            return filePaths;
        }

        int CountNewLines(List<string> filePaths)
        {
            int newLines = 0;
            foreach (string filePath in filePaths)
            {
                try
                {
                    string[] fileLines = File.ReadAllLines(filePath);
                    newLines += fileLines.Length;
                }
                catch { }
            }
            return newLines;
        }

        void DeleteDownloadedRepo()
        {
            if (Directory.Exists("./githubRepoClone"))
            {
                Directory.Delete("./githubRepoClone", true);
            }
        }
    }
}
