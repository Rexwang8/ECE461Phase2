namespace StaticAnalysis;
public static class PullRequests
{
    /*
    Responsive Maintainer


    FOR GITHUB
    Metric   Weight
    ------------------
    Issues                              50%
    Status                              30%
    Updates                             20%

    FOR NPM
    Metric   Weight
    ------------------
    Issues                              50%
    Status                              50%

    */
    public static float GetScore(URLInfo urlInfo)
    {   
        int totalChanges = urlInfo.githubTotalChanges;
        Console.WriteLine(totalChanges);
        int prChanges = urlInfo.githubPRChanges;
        Console.WriteLine(prChanges);
        double fraction = (double)prChanges / totalChanges;
        fraction = CalculateScore((float)fraction);
        return (float)fraction;
    } 

    static float CalculateScore(float fraction)
    {
        // Define the mapping function here. This example uses a simple linear mapping.
        float slope = 0.5f;
        float intercept = 0.25f;

        float score = slope * fraction + intercept;

        // Ensure the score is between 0 and 1
        if (score < 0) score = 0;
        if (score > 1) score = 1;

        return score;
    }
}