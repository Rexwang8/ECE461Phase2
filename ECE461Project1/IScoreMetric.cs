using System;

namespace ECE461Project1
{
    public interface IScoreMetric
    {
        float metricWeight { get; }
        string metricName { get; }
        float GetScore(string githubUrl);
    }
}
