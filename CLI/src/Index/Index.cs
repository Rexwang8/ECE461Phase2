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
    public class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("---Validating inputs---");
            //validate inputs
            bool isValidated = ValidateInputs(args);
            if (!isValidated)
            {
                Environment.Exit(1);
            }

            //initialize args
            string ARGFILEPATH = args[0];
            int ARGLOGLEVEL = int.Parse(args[1]);
            string ARGLOGFILE = args[2];
            string ARGGITHUBTOKEN = args[3];


            //initialize logger
            Logger logger = new Logger(ARGLOGLEVEL, ARGLOGFILE, "Index: ");
            logger.Log("---Starting program -- All validated---", 1);


            //read file
            logger.Log("Reading file", 1);
            List<string> rawUrls = GetRawListFromFile(ARGFILEPATH);

            //convert list of strings to list of empty URLinfos
            List<URLInfo> urlInfos = GetURLList(rawUrls);

            URLClass AllPackages = new URLClass(urlInfos);
            Console.WriteLine("We have " + AllPackages.GetAllPackages().Count + " packages");
            AllPackages.setTotalPackages(AllPackages.GetAllPackages().Count);
            AllPackages.countGithub();
            AllPackages.countNPM();
            AllPackages.printNumPackages();

            Console.WriteLine("------ NPM -------");

            //npm pull
            //repeat three times for retry
            for (int i = 0; i < 3; i++)
            {
                if (AllPackages.getTotalNpmPulled() == AllPackages.getTotalNpm())
                {
                    break;
                }

                foreach (var pkg in AllPackages.GetAllPackages().Values)
                {
                    Console.WriteLine("Getting npm info for " + pkg.getName());
                    if ((pkg.getType() == "npm" || pkg.getType() == "both") && pkg.getNPMSuccess() == false)
                    {
                        callNPM(pkg, logger);
                        
                        //add built in delay to avoid rate limiting
                        System.Threading.Thread.Sleep(500);
                    }
                }
                //update total npm pulled
                AllPackages.countNpmPulled();
                if (AllPackages.getTotalNpmPulled() == AllPackages.getTotalNpm())
                {
                    Console.WriteLine("All npm packages pulled successfully( " + AllPackages.getTotalNpmPulled() + " out of " + AllPackages.getTotalNpm() + " )");
                    break;
                }


                Console.WriteLine("Retrying npm pulls(retry " + (i + 1) + " of 3)");
                System.Threading.Thread.Sleep(2000);


            }

            Console.WriteLine("We have " + AllPackages.getTotalPackages() + " packages, " + AllPackages.getTotalNpm() + " npm, " + AllPackages.getTotalGithub() + " github");
            Console.WriteLine(AllPackages.getTotalNpmPulled() + " npm packages pulled successfully out of " + AllPackages.getTotalNpm() + " npm packages");


            //print results for npm
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                if (pkg.getType() == "npm" || pkg.getType() == "both")
                {
                    Console.WriteLine(pkg.getNPMInfo());
                }
            }

             Console.WriteLine("------ GITHUB -------");
            //github pull
            for (int i = 0; i < 3; i++)
            {
                if (AllPackages.getTotalGithubPulled() == AllPackages.getTotalGithub())
                {

                    break;
                }

                foreach (var pkg in AllPackages.GetAllPackages().Values)
                {
                    Console.WriteLine("Getting github info for " + pkg.getName());
                    Console.WriteLine("pkg type is " + pkg.getType());
                    if ((pkg.getType() == "github" || pkg.getType() == "both") && pkg.getGHSuccess() == false)
                    {
                        callGithub(pkg, logger, ARGGITHUBTOKEN);

                        //add built in delay to avoid rate limiting
                        System.Threading.Thread.Sleep(500);
                    }
                }

                //update total github pulled
                AllPackages.countGithubPulled();
                if (AllPackages.getTotalGithubPulled() == AllPackages.getTotalGithub())
                {
                    Console.WriteLine("All github packages pulled successfully( " + AllPackages.getTotalGithubPulled() + " out of " + AllPackages.getTotalGithub() + " )");
                    break;
                }


                Console.WriteLine("Retrying github pulls(retry " + (i + 1) + " of 3)");
                System.Threading.Thread.Sleep(2000);
            }

            Console.WriteLine("We have " + AllPackages.getTotalPackages() + " packages, " + AllPackages.getTotalNpm() + " npm, " + AllPackages.getTotalGithub() + " github");
            Console.WriteLine(AllPackages.getTotalGithubPulled() + " github packages pulled successfully out of " + AllPackages.getTotalGithub() + " github packages");
 
            
            //Clone repositories
            Console.WriteLine("--- CLONE --- We have " + AllPackages.GetAllPackages().Count + " packages");

            
            CloneUrls(AllPackages);

            //Print Results
            logger.Log("Getting names and types", 1);
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine(pkg.getInfo());
            }

            Console.WriteLine("------ STATIC ANALYSIS -------");

            //Perfom Static Analysis
            StaticAnalysisLibrary StaticAnalysis = new StaticAnalysisLibrary();
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine("Performing StaticAnalysis for " + pkg.getName());
                if (pkg.getPath() != "none")
                {
                    StaticAnalysis.Analyze(pkg);
                }
            }

            Console.WriteLine("------ METRICS -------");

            //get each metric
            logger.Log("Getting each metric", 1);
            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine("!!!!!!Getting metrics for " + pkg.getName());

                //license
                //pkg.CalcValidLicense();

                //busfactor
                pkg.setBusFactor(BusFactor.GetScore(pkg));

                //ramptime
                pkg.setRampUpTime(RampUp.GetScore(pkg));

                //responsive maintainer
                pkg.setResponseMaintainerScore(Maintainer.GetScore(pkg));

                //license compatibility
                //pkg.setLicenseScore(License.GetScore(pkg));
                
                //correctness
                pkg.setCorrectnessScore(Correctness.GetScore(pkg));

                //NEW METRICS
                //code review ratio

                //version ratio
                pkg.setDependencyScore(Dependency.GetScore(pkg));
                //net score
            }

            //write to file

            //print score and license
            logger.Log("Printing out results", 1);
            Console.WriteLine("------ SCORES -------");

            foreach (var pkg in AllPackages.GetAllPackages().Values)
            {
                Console.WriteLine(pkg.getScoreInfo());
            }
            AllPackages.printAllPackages();
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
            //prevent double calls
            if (urlInfo.getNPMSuccess())
                return;

            //Execute the task and handle any errors
            Task<APIError> task = Task.Run(() => urlInfo.PullNpmInfo(logger));
            APIError err = await task;
            if (err.GetErrType() == APIError.errorType.none)
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
            //prevent double calls
            if (urlInfo.getGHSuccess())
                return;

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
        /* 
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
                } */

        public static bool ValidateInputs(string[] args)
        {
            //we expect 4 arguments: the filepath, the log level, and the log file path, and the github token
            if (args.Length != 4)
            {
                Console.WriteLine("Invalid number of arguments. Expected 5, got " + args.Length);
                return false;
            }

            string ARGFILEPATH = args[0];
            int ARGLOGLEVEL = 2;

            //check if filepath is an absolute path that exists
            if (!System.IO.Path.IsPathRooted(ARGFILEPATH))
            {
                Console.WriteLine("Filepath is not an absolute path, it is " + ARGFILEPATH);
                return false;
            }

            //check if file exists
            if (!System.IO.File.Exists(ARGFILEPATH))
            {
                Console.WriteLine("File does not exist");
                return false;
            }

            //check if log level is valid
            bool success = int.TryParse(args[1], out ARGLOGLEVEL);
            if (!success)
            {
                Console.WriteLine("Log level is not an integer");
                return false;
            }

            if (ARGLOGLEVEL < 0 || ARGLOGLEVEL > 2)
            {
                Console.WriteLine("Log level is not valid");
                return false;
            }

            string LOGFPATH = args[2];
            //check if log file path is an absolute path
            if (!System.IO.Path.IsPathRooted(LOGFPATH))
            {
                Console.WriteLine("Log file path is not an absolute path");
                return false;
            }

            //check if github token is valid
            string GHTOKEN = args[3];
            if (GHTOKEN.Length != 40)
            {
                Console.WriteLine("Github token is not valid. Length is " + GHTOKEN.Length);
                return false;
            }

            return true;
        }

        static private void CloneUrls(URLClass urlInfos)
        {
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            Console.WriteLine("Cloning " + urlInfos.GetAllPackages().Count + " packages");
            foreach (var urlInfo in urlInfos.GetAllPackages())
            {
                Console.WriteLine("Cloning " + urlInfo.Value.getName());
                if (urlInfo.Value.getGithubUrl() != "none")
                {
                    string abspath = System.IO.Path.GetFullPath("../../modules/" + urlInfo.Value.getName());
                    urlInfo.Value.setPath(abspath);
                    Cli.Wrap("python3")
                        .WithArguments("../Utility/gitPython.py " + urlInfo.Value.getName() + " " + urlInfo.Value.getGithubUrl())
                        .WithValidation(CommandResultValidation.None)
                        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                        .ExecuteAsync()
                        .GetAwaiter()
                        .GetResult();
                    Logger.WriteLine("stdout" + stdOutBuffer.ToString(), 2);
                    Console.WriteLine("stdout" + stdOutBuffer.ToString());
                    Logger.WriteLine("stderr" + stdErrBuffer.ToString(), 2);
                    Console.WriteLine("stderr" + stdErrBuffer.ToString());
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
