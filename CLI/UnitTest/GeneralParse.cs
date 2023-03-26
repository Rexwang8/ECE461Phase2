using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using StaticAnalysis;
using Utility;
using System.IO;
using Index;

namespace UnitTest;
[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestENVSet()
    {
        Console.WriteLine("TestENVSet");
        bool isLogFileSet = Environment.GetEnvironmentVariable("LOG_FILE") != null;
        bool isLevelSet = Environment.GetEnvironmentVariable("LOG_LEVEL") != null;
        bool isTokenSet = Environment.GetEnvironmentVariable("GITHUB_TOKEN") != null;
        Assert.IsTrue(isLogFileSet && isLevelSet && isTokenSet);
    }

    //Verify Checker for environment variables works
    [TestMethod]
    public void TestCheckARGS()
    {
        string[] args = new string[4];
        
        args[0] = "/home/shay/a/wang5009/461/ECE461Phase2/CLI/example/git.txt";
        args[1] = "2";
        args[2] = "/home/shay/a/wang5009/461/logoutput.log";
        args[3] = "ghp_123412341234123412341234m12343123412";
        /*
        args[0] = "/home/shay/a/ma562/461/CLI/example/git.txt";
        args[1] = "0";
        args[2] = "/home/shay/a/ma562/461/logoutput.log";
        args[3] = "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";       //DON'T EVEN THINK ABOUT STEALING MY TOKEN (i already revoked it)
        Console.WriteLine("TestCheckARGS");
        */
        bool isValid = Index.Program.ValidateInputs(args);
        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void GetScore_ReturnsOne_WhenLicenseNoMatch()
    {
        StaticAnalysis.License license = new StaticAnalysis.License();
        string githubUrl = "https://github.com/Rexwang8/ECE461SoftwareEngineeringProject/blob/main/README.md";

        float score = license.GetScore(githubUrl);

        Assert.AreEqual(0, score);
    }

    [TestMethod]
    public void GetScore_WhenLicenseFail()
    {
        StaticAnalysis.License license = new StaticAnalysis.License();
        string githubUrl = "https://github.com/nonexistentusername/nonexistentrepository";

        float score = license.GetScore(githubUrl);

        Assert.AreEqual(0, score);
    }

}

[TestClass]
public class ScoreMetricTests
{
    private class MockScoreMetric : IScoreMetric
    {
        public float metricWeight => 1f;

        public string metricName => "Mock Score Metric";

        public float GetScore(string githubUrl)
        {
            return 0.5f;
        }
    }

    [TestMethod]
    public void MockScoreMetric_GetScore_ReturnsExpectedValue()
    {
        var scoreMetric = new MockScoreMetric();
        var githubUrl = "https://github.com/example/example-repo";

        var actualScore = scoreMetric.GetScore(githubUrl);
        var expectedScore = 0.5f;

        Assert.AreEqual(expectedScore, actualScore);
    }
}

[TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void Log_WithLogLevelGreaterThanMessagePriority_LogsMessageToFile()
        {
            string logFile = "test_log.txt";
            int logLevel = 2;
            string prefix = "TESTPREFIX: ";
            Logger logger = new Logger(logLevel, logFile, prefix);

            string message = "Test log message";
            int messagePriority = 1;

            logger.Log(message, messagePriority);

            string[] logLines = File.ReadAllLines(logFile);

            Assert.IsTrue(logLines[0].Contains(prefix));
        }

        [TestCleanup]
        public void Cleanup()
        {
            string logFile = "test_log.txt";
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
