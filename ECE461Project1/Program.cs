﻿using System;
using System.Collections.Generic;

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

                return 0; //Exit success
            }
            else if (args[0] == "test")
            {
                //run unit tests

                return 0; //Exit success
            }
            else
            {
                //                    /Users/ishaan/Desktop/ECE\ 461/ECE461TeamRepo/ECE461Project1/URLTestFile.txt
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
                Console.WriteLine(url);
                foreach (IScoreMetric scoreMetric in scoreMetrics)
                {
                    if (scoreMetric.metricName != "Bus Factor")
                    {
                        float unweightedScore = scoreMetric.GetScore(url);
                        float weightedScore = unweightedScore * scoreMetric.metricWeight;
                        Console.WriteLine(scoreMetric.metricName + " score = " + unweightedScore);
                    }
                }
            }
        }
    }
}
