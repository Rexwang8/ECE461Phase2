using System;

namespace ECE461ProjectPart1
{
    public interface IScoreMetric
    {
        float metricWeight { get; }
        int GetScore(string githubUrl);
    }
}
