using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace ECE461Project1
{
    class Program
    {
        static int Main(string[] args)
        {
            if(!Logger.CreateFile())
            {
                Console.WriteLine("$LOG_FILE environment variable not set");
                return 1; //Exit Failure
            }

            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect num of input arguments");
                return 1; //Exit Failure
            }

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

                    scoreText += "\"" + scoreMetric.metricName + "_SCORE\":" + unweightedScore + ", ";
                }

                scoreText += "\"CORRECTNESS_SCORE\":-1, \"RESPONSIVE_MAINTAINER_SCORE\":-1,  \"NET_SCORE\":" + netScore + "}\n";

                scoreSheets.Add(new ScoreSheet(netScore, scoreText));
            }

            while (scoreSheets.Count > 0)
            {
                float maxScore = -1;
                ScoreSheet scoreSheet = null;
                foreach (ScoreSheet scoreSheetInList in scoreSheets)
                {
                    if (scoreSheetInList.netScore > maxScore)
                    {
                        maxScore = scoreSheetInList.netScore;
                        scoreSheet = scoreSheetInList;
                    }
                }
                if (scoreSheet != null)
                {
                    Console.Write(scoreSheet.scoreText);
                    scoreSheets.Remove(scoreSheet);
                }
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
            string passedCases = StringSplitHelper(stdOut, ", Passed:    ", ",");
            string totalCases = StringSplitHelper(stdOut, ", Total:    ", ",");
            string totalCodeCoverage = StringSplitHelper(stdOut, "| ECE461Project1 | ", "%");

            Console.WriteLine("Total: " + totalCases);
            Console.WriteLine("Passed: " + passedCases);
            Console.WriteLine("Coverage: " + totalCodeCoverage + "%");
            Console.WriteLine(passedCases + "/" + totalCases + " test cases passed. " + totalCodeCoverage + "% line coverage achieved.");

            Logger.WriteLine(stdOut, 2);
            Logger.WriteLine(tests.StandardError.ReadToEnd(), 2);

        }

        static string StringSplitHelper(string input, string firstSplit, string secondSplit)
        {
            string[] split1Result = input.Split(firstSplit);
            if (split1Result.Length < 2) return string.Empty;
            return split1Result[1].Split(secondSplit)[0];
        }

        static void CompileCode()
        {
            Process builder = new Process();
            builder.StartInfo.RedirectStandardOutput = true;
            builder.StartInfo.RedirectStandardError = true;
            builder.StartInfo.FileName = "dotnet";
            builder.StartInfo.Arguments = "build";
            builder.StartInfo.WorkingDirectory = "./Code";
            builder.Start();

            while (!builder.HasExited) ;

            Logger.WriteLine(builder.StandardOutput.ReadToEnd(), 2);
            Logger.WriteLine(builder.StandardError.ReadToEnd(), 2);
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
