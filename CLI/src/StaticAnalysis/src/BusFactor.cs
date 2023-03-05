
namespace StaticAnalysis;
public static class BusFactor
{
    //metrics used for check if it has a readme
    public static float GetScore(URLInfo urlInfo)
    {   
        float finalScore = 0;
        float totalScore = 0;
        //Calculate Issues
        if (urlInfo.githubIssues == 0)
        {
            
        }
        Console.WriteLine($"There are {urlInfo.githubIssues} githubIssues");
        Console.WriteLine($"There are {urlInfo.githubOpenIssues} githubOpenIssues");
        return finalScore;
    }
}