namespace StaticAnalysis;
public static class BusFactor
{
    /*
    BusFactor

    Metric   Weight
    ------------------
    Issues      30%
    Health      70%
    Forks       


    */
    public static float GetScore(URLInfo urlInfo)
    {   
        Console.WriteLine("-----\nCalculating BusFactor");

        float finalScore = 0;
        const float ISSUE_WEIGHT = .3f;
        const float HEALTH_WEIGHT = .7f;

        int health_score = 0;
        int fork_score = 0;

        //Calculate Issues
        if(urlInfo.githubIssues != 0 && urlInfo.githubOpenIssues != 0)
        {
            finalScore += ISSUE_WEIGHT * (urlInfo.githubIssues - urlInfo.githubOpenIssues)/urlInfo.githubIssues;
            Console.WriteLine($"There are {urlInfo.githubIssues} githubIssues");
            Console.WriteLine($"There are {urlInfo.githubOpenIssues} githubOpenIssues");
        }
        else
        {
            Console.WriteLine("No Issues");
        }
        
        //Health Percentage: ReadME or License presence
        if (urlInfo.licensePath != "")
        {
            health_score += 1;
        }
        if (urlInfo.readmePath != "")
        {
            health_score += 1;
        }
        finalScore += HEALTH_WEIGHT * health_score/2;

        //Forks
        /* if (urlInfo.githubForks == 0)
        {
            //Check if there is any open issues 
            
        }
        else if (urlInfo.githubForks <= 25)
        {
            
        }
        else if (urlInfo.githubForks <= 50)
        {

        }
        else if (urlInfo.githubForks <= 100)
        {

        }
        else if (urlInfo.githubForks <= 500)
        {

        }
        else
        {

        } */


        return finalScore;
    }
}