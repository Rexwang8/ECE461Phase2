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
        /*
        args[0] = "/home/shay/a/wang5009/461/ECE461Phase2/CLI/example/git.txt";
        args[1] = "2";
        args[2] = "/home/shay/a/wang5009/461/logoutput.log";
        args[3] = "ghp_123412341234123412341234m12343123412";
        */
        args[0] = "/home/shay/a/ma562/461/CLI/example/git.txt";
        args[1] = "0";
        args[2] = "/home/shay/a/ma562/461/logoutput.log";
        args[3] = "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";
        Console.WriteLine("TestCheckARGS");
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