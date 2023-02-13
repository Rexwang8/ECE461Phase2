using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ECE461Project1
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1) return 1; //Exit Failure

            //Command Line Inputs Begin Here
            if (args[0] == "install")
            {
                //Install Dependencies

                return 0; //Exit success
            }
            else if (args[0] == "build")
            {
                //compile code
                CompileCode();

                return 0; //Exit success
            }
            else if (args[0] == "test")
            {
                //run unit tests
                RunUnitTests();

                return 0; //Exit success
            }
            else
            {
                //                    /Users/ishaan/Desktop/ECE\ 461/ECE461TeamRepo/Code/ECE461Project1/URLTestFile.txt
                GetScores(args[0]);

                return 0; //Exit success
            }
        }

        static void GetScores(string urlFilePath)
        {
            string[] rawUrls = GithubURLRetriever.GetRawListFromFile(urlFilePath);
            List<string> githubUrlList = GithubURLRetriever.GetURLList(rawUrls);

            List<IScoreMetric> scoreMetrics = new List<IScoreMetric>();

            scoreMetrics.Add(new RampUpTime());
            scoreMetrics.Add(new License());
            scoreMetrics.Add(new BusFactor());

            List<ScoreSheet> scoreSheets = new();

            foreach (string url in githubUrlList)
            {
                string scoreText = string.Empty;
                float netScore = 0;
                scoreText += "{\"URL\":\"" + GithubURLRetriever.githubUrlToRawURL[url] + "\", ";
                foreach (IScoreMetric scoreMetric in scoreMetrics)
                {
                    float unweightedScore = scoreMetric.GetScore(url);
                    float weightedScore = unweightedScore * scoreMetric.metricWeight;
                    netScore += weightedScore;

                    scoreText += "\"" + scoreMetric.metricName + "_SCORE\":" + weightedScore + ", ";
                }

                scoreText += "\"CORRECTNESS_SCORE\":-1, \"RESPONSIVE_MAINTAINER_SCORE\":-1,  \"NET_SCORE\":" + netScore + "}\n";

                scoreSheets.Add(new ScoreSheet(netScore, scoreText));
            }

            while (scoreSheets.Count > 0)
            {
                float maxScore = -1;
                string scoreText = string.Empty;
                foreach (ScoreSheet scoreSheet in scoreSheets)
                {
                    if (scoreSheet.netScore > maxScore)
                    {
                        maxScore = scoreSheet.netScore;
                        scoreText = scoreSheet.scoreText;
                    }
                }
                Console.WriteLine(scoreText);
            }
        }

        static void RunUnitTests()
        {
            Process tests = new Process();
            tests.StartInfo.RedirectStandardOutput = true;
            tests.StartInfo.RedirectStandardError = true;
            tests.StartInfo.FileName = "dotnet";
            tests.StartInfo.Arguments = "test /p:CollectCoverage=true /p:CoverletOutputFormat=teamcity";
            tests.StartInfo.WorkingDirectory = "./Code";
            tests.Start();

            while (!tests.HasExited) ;

            string stdOut = tests.StandardOutput.ReadToEnd();
            string passedCases = stdOut.Split(", Passed:    ")[1].Split(',')[0];
            string totalCases = stdOut.Split(", Total:    ")[1].Split(',')[0];
            string totalCodeCoverage = stdOut.Split("| ECE461Project1 | ")[1].Split('%')[0];

            Console.WriteLine("Total: " + totalCases);
            Console.WriteLine("Passed: " + passedCases);
            Console.WriteLine("Coverage: " + totalCodeCoverage + "%");
            Console.WriteLine(passedCases + "/" + totalCases + " test cases passed. " + totalCodeCoverage + "% line coverage achieved.");

        }

        static void CompileCode()
        {
            Process builder = new Process();
            builder.StartInfo.FileName = "dotnet";
            builder.StartInfo.Arguments = "build";
            builder.StartInfo.WorkingDirectory = "./Code";
            builder.Start();

            while (!builder.HasExited) ;
        }

        class ScoreSheet
        {
            public float netScore;
            public string scoreText;

            public ScoreSheet(float netScore, string scoreSheet)
            {
                this.netScore = netScore;
                this.scoreText = scoreSheet;
            }
        }
    }
}
