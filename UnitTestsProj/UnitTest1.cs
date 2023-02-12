using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECE461Project1;

namespace UnitTestsProj
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void License()
        {
            string url = "https://github.com/nodejs/node-addon-api";
            License license = new License();
            float result = license.GetScore(url);
            Assert.AreEqual(0, result, "Incorrect Result");
        }
    }
}
