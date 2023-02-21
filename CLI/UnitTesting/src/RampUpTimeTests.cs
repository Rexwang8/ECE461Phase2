﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Index;
using System.Collections.Generic;
using StaticAnalysis;

namespace UnitTesting;

[TestClass]
public class RampUpTimeTests
{
    [TestMethod]
    public void GetScoreTest()
    {
        string url = "https://github.com/nodejs/node-addon-api";
        RampUpTime rampUpTime = new RampUpTime();
        float result = rampUpTime.GetScore(url); ;
        float expectedValue = 0.47025f;
        float acceptableDeviation = 0.01f;
        Assert.IsTrue((expectedValue - acceptableDeviation <= result) && (result <= expectedValue + acceptableDeviation));
    }
}

