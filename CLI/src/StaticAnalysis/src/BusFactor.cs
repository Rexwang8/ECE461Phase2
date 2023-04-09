namespace StaticAnalysis;
public static class BusFactor
{
    public static float GetScore(URLInfo urlInfo)
    {   
        Console.WriteLine("-----\nCalculating BusFactor");

        float ISSUE_WEIGHT = 0;
        float HEALTH_WEIGHT = 0;
        float FORK_WEIGHT = 0;
        float STATE_WEIGHT = 0;
        float DISCUSSION_WEIGHT = 0;
        float WATCHER_WEIGHT = 0;

        float finalScore = 0;
        int health_score = 0;
        int fork_score = 0;
        int state_score = 0;
        int discussion_score = 0;
        int watcher_score = 0;

        //Check Repo State
        if (urlInfo.githubIsArchived || urlInfo.githubIsDisabled || urlInfo.githubIsEmpty || urlInfo.githubIsLocked)
        {
            ISSUE_WEIGHT = .22f;
            HEALTH_WEIGHT = .03f;
            FORK_WEIGHT = .22f;
            STATE_WEIGHT = .3f;
            DISCUSSION_WEIGHT = .1f;
            WATCHER_WEIGHT = .13f;

            state_score += 0;  
        }
        else
        {
            ISSUE_WEIGHT = .10f;
            HEALTH_WEIGHT = .05f;
            FORK_WEIGHT = .35f;
            STATE_WEIGHT = .05f;
            DISCUSSION_WEIGHT = .25f;
            WATCHER_WEIGHT = .2f;

            state_score += 1;
        }
        finalScore += STATE_WEIGHT * state_score;
        
        //Calculate Issues
        if(urlInfo.githubIssues != 0)
        {
            finalScore += ISSUE_WEIGHT * (urlInfo.githubIssues - urlInfo.githubOpenIssues)/urlInfo.githubIssues;
        }
        else
        {
            finalScore += ISSUE_WEIGHT * 1;
        }
        Console.WriteLine("Final Score [56]: " + finalScore);
        //Check if Discussions
        if (urlInfo.githubDiscussions == 0)
        {
            discussion_score += 0;
        }
        else if (urlInfo.githubDiscussions <= 5)
        {
            discussion_score += 5;
        }
        else if (urlInfo.githubDiscussions <= 15)
        {
            discussion_score += 10;
        }
        else if (urlInfo.githubDiscussions <= 25)
        {
            discussion_score += 15;
        }
        else
        {
            discussion_score += 20;
        }
        finalScore += DISCUSSION_WEIGHT * discussion_score/20;

        Console.WriteLine("Final Score [80]: " + finalScore);
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

        Console.WriteLine("Final Score [92]: " + finalScore);
        //Forks
        if (urlInfo.githubForks == 0)
        {
            fork_score += 5;
        }
        else if (urlInfo.githubForks <= 25)
        {
            fork_score += 8;
        }
        else if (urlInfo.githubForks <= 50)
        {
            fork_score += 12;
        }
        else if (urlInfo.githubForks <= 100)
        {
            fork_score += 15;
        }
        else if (urlInfo.githubForks <= 500)
        {
            fork_score += 18;
        }
        else
        {
            fork_score += 20;
        }
        finalScore += fork_score/20 * FORK_WEIGHT;
        Console.WriteLine("Final Score [119]: " + finalScore);
        //Watchers
        if (urlInfo.githubWatchers == 0)
        {
            watcher_score += 0;
        }
        else if (urlInfo.githubWatchers <= 25)
        {
            watcher_score += 5;
        }
        else if (urlInfo.githubWatchers <= 50)
        {
            watcher_score += 8;
        }
        else
        {
            watcher_score += 10;
        }
        finalScore += watcher_score/10 * WATCHER_WEIGHT;

        return finalScore;
    }
}