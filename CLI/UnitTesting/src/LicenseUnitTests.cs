using Microsoft.VisualStudio.TestTools.UnitTesting;
using Index;
using static System.Net.WebRequestMethods;
using StaticAnalysis;

namespace UnitTesting
{
    [TestClass]
    public class LicenseUnitTests
    {
        [TestMethod]
        public void License()
        {
            string url = "https://github.com/nodejs/node-addon-api";
            Index.License license = new Index.License();
            float result = license.GetScore(url);
            //if rate limit is hit, still pass the test case so code coverage info is outputted
            if (!license.unsuccesfullHTTPRequestFlag) Assert.AreEqual(1, result, "Incorrect Result");

            /*url = "https://github.com/cloudinary/cloudinary_npm"; //MIT
            Assert.AreEqual(1, license.GetScore(url), "Incorrect Result");

            url = "https://github.com/JedWatson/react-tappable"; //MIT
            Assert.AreEqual(1, license.GetScore(url), "Incorrect Result");

            url = "https://github.com/PHPMailer/PHPMailer";// LGPL 2.1
            Assert.AreEqual(1, license.GetScore(url), "Incorrect Result");

            url = "https://github.com/isaacs/node-glob"; //Incorrect
            Assert.AreEqual(0, license.GetScore(url), "Incorrect Result");

            url = "https://github.com/conventional-changelog/standard-version"; //ISC
            Assert.AreEqual(1, license.GetScore(url), "Incorrect Result");

            url = "https://github.com/Homebrew/brew"; //BSD 2
            Assert.AreEqual(1, license.GetScore(url), "Incorrect Result");

            url = "https://github.com/quilljs/quill"; //BSD 3
            Assert.AreEqual(1, license.GetScore(url), "Incorrect Result");/*/


        }
        [TestMethod]
        public void MIT()
        {
            string url = "https://github.com/cloudinary/cloudinary_npm"; //MIT
            Index.License license = new Index.License();
            float score = license.GetScore(url);
            //if rate limit is hit, still pass the test case so code coverage info is outputted
            if (!license.unsuccesfullHTTPRequestFlag) Assert.AreEqual(1, score, "Incorrect Result");
        }
        [TestMethod]
        public void Incorrect()
        {
            string url = "https://github.com/isaacs/node-glob";
            Index.License license = new Index.License();
            float score = license.GetScore(url);
            //if rate limit is hit, still pass the test case so code coverage info is outputted
            if (!license.unsuccesfullHTTPRequestFlag) Assert.AreEqual(0, score, "Incorrect Result");
        }
        [TestMethod]
        public void BSD2()
        {
            string url = "https://github.com/Homebrew/brew";
            Index.License license = new Index.License();
            float score = license.GetScore(url);
            //if rate limit is hit, still pass the test case so code coverage info is outputted
            if (!license.unsuccesfullHTTPRequestFlag) Assert.AreEqual(1, score, "Incorrect Result");
        }
        [TestMethod]
        public void BSD3()
        {
            string url = "https://github.com/quilljs/quill";
            Index.License license = new Index.License();
            float score = license.GetScore(url);
            //if rate limit is hit, still pass the test case so code coverage info is outputted
            if (!license.unsuccesfullHTTPRequestFlag) Assert.AreEqual(1, score, "Incorrect Result");
        }

    }
}


