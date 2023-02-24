using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using StaticAnalysis;
using Utility;
using System.IO;

namespace UnitTesting
{
    [TestClass]
    public class PullNpmInforUnitTests
    {
        // Tests based on repo commits as of 2/12/23
        // Environment Variables for Windows:
        // Command Prompt -> setx GITHUB_TOKEN <GITHUB_TOKEN value>
        // Restart Visual Studio / code editor afterwards

        // Basic run
        [TestMethod]
        public void BasicRunTest()
        {
            // Stale for ~1 year
            string url = "https://github.com/browserify/browserify";
            Logger logger = new Logger();
            URLInfo urlinfo = new URLInfo(url);
            System.Threading.Tasks.Task pulled = urlinfo.PullNpmInfo(logger);
            String datapath = "";
            Assert.IsTrue(new FileInfo(datapath).Length != 0);
        }

        // Single committer run
        [TestMethod]
        public void InvalidURL()
        {
            // Stale for ~8 years
            string url = "https://github.com/12u8937984749502798345";
            Logger logger = new Logger();
            URLInfo urlinfo = new URLInfo(url);
            //System.Threading.Tasks.Task pulled = urlinfo.PullNpmInfo(logger);
            int result = urlinfo.PullNpmInfo(logger).Result;
            Assert.IsTrue(result == 1);
        }
    }
}
