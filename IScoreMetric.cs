using System;

namespace ECE461Project1
{
    public interface IScoreMetric
    {
        float metricWeight { get; }
        int GetScore(string githubUrl);
    }
}
