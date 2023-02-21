using Microsoft.VisualStudio.TestTools.UnitTesting;
using Index;
using System.Collections.Generic;
using Utility;

namespace UnitTesting
{
    [TestClass]
    public class GetGitUrlsUnitTests
    {
        [TestMethod]
        public void GitUrlTest()
        {
            string[] url = { "https://github.com/nodejs/node-addon-api" };
            List<string> result = GithubURLRetriever.GetURLList(url);
            Assert.AreEqual(url[0], result[0]);
        }

        [TestMethod]
        public void NpmjsUrlTest()
        {
            string[] url = { "https://www.npmjs.com/package/express" };
            List<string> result = GithubURLRetriever.GetURLList(url);
            Assert.AreEqual("https://github.com/expressjs/express", result[0]);
        }
    }
}
