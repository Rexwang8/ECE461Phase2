namespace StaticAnalysis;
public static class NetScore
{
    public static float GetNetScore(URLInfo urlInfo)
    {
        float netScore = 0;
        netScore = urlInfo.license_score + urlInfo.rampUp_score + urlInfo.busFactor_score + urlInfo.correctness_score + urlInfo.responseMaintainer_score + urlInfo.dependency_score + urlInfo.pullreview_score;
        netScore = netScore / 7;
        return netScore;
    }
}