using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace StaticAnalysis;
public class RampUpTime : IScoreMetric
{
    const int maxNumLines = 400000;
    const int minNumLines = 100;

    public float metricWeight { get; } = 0.25f;
    public string metricName { get; } = "RAMP_UP";

    public float GetScore(string githubUrl)
    {
        DeleteDownloadedRepo();

        Process gitClone = new Process();
        gitClone.StartInfo.RedirectStandardOutput = true;
        gitClone.StartInfo.RedirectStandardError = true;
        gitClone.StartInfo.FileName = "git";
        gitClone.StartInfo.Arguments = "clone " + githubUrl + " ./githubRepoClone";
        gitClone.Start();

        while (!gitClone.HasExited) ;

        int numLines = CountNewLines(GetFilesPaths("./githubRepoClone"));

        DeleteDownloadedRepo();

        if (numLines > maxNumLines) numLines = maxNumLines;
        if (numLines <= minNumLines) numLines = 0;
        float score = 1f - ((float)numLines / maxNumLines);

        return score;
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
