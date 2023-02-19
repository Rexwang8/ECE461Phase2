namespace StaticAnalysis;
public interface IScoreMetric
{
    float metricWeight { get; }
    string metricName { get; }
    float GetScore(string githubUrl);
}
