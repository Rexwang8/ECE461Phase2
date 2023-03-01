using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Utility;
using StaticAnalysis;
using CliWrap;
using System.Text;

namespace Index
{
    class Program
    {
        static int Main(string[] args)
        {
            //validate inputs
            ValidateInputs(args);

            //initialize args
            string ARGFILEPATH = args[0];
            int ARGLOGLEVEL = int.Parse(args[1]);
            string ARGLOGFILE = args[2];
            string ARGGITHUBTOKEN = args[3];

            //initialize logger
            Logger logger = new Logger(ARGLOGLEVEL, ARGLOGFILE, "Index: ");
            logger.Log("Starting program -- All validated", 1);

            //read file
            logger.Log("Reading file", 1);
            List<string> rawUrls = GetRawListFromFile(ARGFILEPATH);

            //convert list of strings to list of empty URLinfos
            List<URLInfo> urlInfos = GetURLList(rawUrls);

            URLClass AllPackages = new URLClass(urlInfos);
            Console.WriteLine("We have " + AllPackages.GetAllPackages().Count + " packages");

            //npm pull
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine("Getting npm info for " + pkg.getName());
                if (pkg.getType() == "npm" || pkg.getType() == "both")
                {
                    callNPM(pkg, logger);

                    //add built in delay to avoid rate limiting
                    System.Threading.Thread.Sleep(500);
                }
            }
            System.Threading.Thread.Sleep(1000);
            //print results for npm
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                if (pkg.getType() == "npm" || pkg.getType() == "both")
                {
                    Console.WriteLine(pkg.getNPMInfo());
                }
            }
            

            //github pull
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine("Getting github info for " + pkg.getName());
                Console.WriteLine("pkg type is " + pkg.getType());
                if (pkg.getType() == "github" || pkg.getType() == "both")
                {
                    callGithub(pkg, logger, ARGGITHUBTOKEN);

                    //add built in delay to avoid rate limiting
                    System.Threading.Thread.Sleep(500);
                }
            }


            System.Threading.Thread.Sleep(15000);
/*
            //Clone repositories
            CloneUrls(AllPackages);

            //Print Results
            logger.Log("Getting names and types", 1);
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine(pkg.getInfo());
            }
            
            //Perfom Static Analysis
            StaticAnalysisLibrary StaticAnalysis = new StaticAnalysisLibrary();
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine("Peforming StaticAnalysis for " + pkg.getName());
                if (pkg.getPath() != "none")
                {
                    StaticAnalysis.Analyze(pkg);
                }
            }

            //Print Static Analysis Results only for cloned repo
            logger.Log("Getting names and types", 1);
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                if (pkg.getPath() != "none")
                {
                    Console.WriteLine(pkg.getStaticInfo());
                }
            }

            //get each metric
            logger.Log("Getting each metric", 1);
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine("Getting metrics for " + pkg.getName());
                //ramptime

                //license

                //busfactor

                //responsive maintainer

                //license compatibility

                //correctness

                //NEW METRICS
                //code review ratio

                //version ratio

                //net score
            }

            //write to file

            return 0;
            */
            return 0;
        }

        static List<string> GetRawListFromFile(string urlFilePath)
        {
            List<string> rawUrls = new();
            using (StreamReader sr = new StreamReader(urlFilePath))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    rawUrls.Add(line);
                }
            }
            return rawUrls;
        }


        //Void async function to call npm api
        public static async void callNPM(URLInfo urlInfo, Logger logger)
        {
            //Execute the task and handle any errors
            Task<APIError> task = Task.Run(() => urlInfo.PullNpmInfo(logger));
            APIError err = await task;
            if(err.GetErrType() == APIError.errorType.none)
            {
                Console.WriteLine("NPM Data Recieved for package: " + urlInfo.getName());
                logger.Log("NPM Data Recieved for package: " + urlInfo.getName(), 1);
            }

            else
            {
                Console.WriteLine("Error: " + err.ToString());
                logger.Log("Error: " + err.ToString(), 1);
            }
            return;
        }


        //void async to call github api
        public static async void callGithub(URLInfo urlInfo, Logger logger, string githubToken)
        {
            //Execute the task and handle any errors
            Task<APIError> task = Task.Run(() => urlInfo.PullGithubInfo(logger, githubToken));
            APIError err = await task;
            if (err.GetErrType() == APIError.errorType.none)
            {
                Console.WriteLine("Github Data Recieved for package: " + urlInfo.getName());
                logger.Log("Github Data Recieved for package: " + urlInfo.getName(), 1);
            }

            else
            {
                Console.WriteLine("Error: " + err.ToString());
                logger.Log("Error: " + err.ToString(), 1);
            }
            return;
        }

        static List<URLInfo> GetURLList(List<string> rawUrls)
        {
            List<URLInfo> urlInfos = new();
            foreach (string rawUrl in rawUrls)
            {
                urlInfos.Add(new URLInfo(rawUrl));
            }
            return urlInfos;
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
                ScoreSheet? scoreSheet = null;
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

        static void ValidateInputs(string[] args)
        {
            //we expect 4 arguments: the filepath, the log level, and the log file path, and the github token
            if (args.Length != 4)
            {
                Console.WriteLine("Invalid number of arguments. Expected 5, got " + args.Length);
                Environment.Exit(1);
            }

            string ARGFILEPATH = args[0];
            int ARGLOGLEVEL = 2;

            //check if filepath is an absolute path that exists
            if (!System.IO.Path.IsPathRooted(ARGFILEPATH))
            {
                Console.WriteLine("Filepath is not an absolute path, it is " + ARGFILEPATH);
                Environment.Exit(1);
            }

            //check if file exists
            if (!System.IO.File.Exists(ARGFILEPATH))
            {
                Console.WriteLine("File does not exist");
                Environment.Exit(1);
            }

            //check if log level is valid
            bool success = int.TryParse(args[1], out ARGLOGLEVEL);
            if (!success)
            {
                Console.WriteLine("Log level is not an integer");
                Environment.Exit(1);
            }

            if (ARGLOGLEVEL < 0 || ARGLOGLEVEL > 2)
            {
                Console.WriteLine("Log level is not valid");
                Environment.Exit(1);
            }

            string LOGFPATH = args[2];
            //check if log file path is an absolute path
            if (!System.IO.Path.IsPathRooted(LOGFPATH))
            {
                Console.WriteLine("Log file path is not an absolute path");
                Environment.Exit(1);
            }

            //check if github token is valid
            string GHTOKEN = args[3];
            if (GHTOKEN.Length != 40)
            {
                Console.WriteLine("Github token is not valid");
                Environment.Exit(1);
            }

            return;
        }

        static private void CloneUrls(URLClass urlInfos)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            foreach (var urlInfo in urlInfos.GetAllPackages())
            {
                if (urlInfo.Value.getGithubUrl() != "none")
                {
                    //urlInfo.Value.path = "../../modules/" + urlInfo.Value.getName();
                    urlInfo.Value.setPath("../../modules/" + urlInfo.Value.getName());
                    Cli.Wrap("python3")
                        .WithArguments("../Utility/gitPython.py " + urlInfo.Value.getName() + " " + urlInfo.Value.getGithubUrl())
                        .WithValidation(CommandResultValidation.None)
                        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                        .ExecuteAsync()
                        .GetAwaiter()
                        .GetResult();
                    Logger.WriteLine(stdOutBuffer.ToString(), 2);
                    Logger.WriteLine(stdErrBuffer.ToString(), 2);
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
    
    public class License : IScoreMetric
    {
        //dummy class to get vscode to stop yelling at me
        public string metricName { get; set; } = "LICENSE";
        public float metricWeight { get; set; } = 0.2f;

        public float GetScore(string url)
        {
            return 0.5f;
        }

        public bool unsuccesfullHTTPRequestFlag = true;
    }
}
