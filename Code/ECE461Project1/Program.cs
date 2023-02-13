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

            foreach (string url in githubUrlList)
            {
                float netScore = 0;
                Console.Write("{\"URL\":\"" + GithubURLRetriever.githubUrlToRawURL[url] + "\", ");
                foreach (IScoreMetric scoreMetric in scoreMetrics)
                {
                    float unweightedScore = scoreMetric.GetScore(url);
                    float weightedScore = unweightedScore * scoreMetric.metricWeight;
                    netScore += weightedScore;

                    Console.Write("\"" + scoreMetric.metricName + "_SCORE\":" + weightedScore + ", ");
                }

                Console.Write("\"CORRECTNESS_SCORE\":-1, \"RESPONSIVE_MAINTAINER_SCORE\":-1,  \"NET_SCORE\":" + netScore + "}\n");
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

            Console.Write(tests.StandardOutput.ReadToEnd());
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n" + tests.StandardError.ReadToEnd());
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
    }
}
