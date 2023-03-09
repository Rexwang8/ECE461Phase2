namespace StaticAnalysis;
public static class RampUp
{
    /*
    Rampup

    Metric   Weight
    ------------------
    Issues                              20%
    Size of repo (larger is worse)      20%
    Ratio of comments to code           20%
    Presence of readme                  10%
    Number of stargazers/Maintainers    20%
    Health                              10%
    Forks       


    */
    public static float GetScore(URLInfo urlInfo)
    {   
        Console.WriteLine("-----\nCalculating Rampup Time");

        float finalScore = 0;
        const float ISSUE_WEIGHT = .2f;
        const float HEALTH_WEIGHT = .1f;
        const float SIZE_WEIGHT = .2f;
        const float COMMENT_WEIGHT = .2f;
        const float CONTRIBUTOR_WEIGHT = .2f;
        const float README_WEIGHT = .1f;




        int health_score = 0;
        int fork_score = 0;


        //Count issues
        if(urlInfo.getType() == "github" || urlInfo.getType() == "both")
        {
            if(urlInfo.githubIssues != 0)
            {
                //award less points for more issues
                int numissues = urlInfo.githubIssues;
                float scoreAwardedForIssues = ISSUE_WEIGHT * (1 - (numissues/100));
                finalScore += Math.Clamp(scoreAwardedForIssues, 0, ISSUE_WEIGHT);

                Console.WriteLine($"Awarded {scoreAwardedForIssues} for {numissues} issues");
            }
            else
            {
                Console.WriteLine("Awarding 0.2 points for no issues");
                finalScore += ISSUE_WEIGHT;
            }
        }
        else if (urlInfo.getType() == "npm")
        {
            //npm doesn't have issues, so instead, we check for size of readme as a patch
            int lengthnpmReadme = urlInfo.npmReadme.Length;
            float scoreAwardedForReadme = README_WEIGHT * (1 - (lengthnpmReadme/1000));
            finalScore += Math.Clamp(scoreAwardedForReadme, 0, README_WEIGHT);
            Console.WriteLine($"Awarded {scoreAwardedForReadme} for {lengthnpmReadme} characters in readme");
        }
        else
        {
            Console.WriteLine("No Type, Awarding no points for issues");
        }

        //Size of repo
        //We check with static analysis to see if the repo is large
        //If it is, we award less points
        int numLinesCode = urlInfo.codeLineCount;
        float scoreAwardedForSize = SIZE_WEIGHT * (1 - (numLinesCode/10000));
        finalScore += Math.Clamp(scoreAwardedForSize, 0, SIZE_WEIGHT);
        Console.WriteLine($"Awarded {scoreAwardedForSize} for {numLinesCode} lines of code");

        //Ratio of comments to code
        //We check with static analysis to see if the repo has a lot of comments
        //If it does, we award more points
        int numLinesComments = urlInfo.commentLineCount;
        float scoreAwardedForComments = COMMENT_WEIGHT * ((3.0f * numLinesComments)/numLinesCode);
        finalScore += Math.Clamp(scoreAwardedForComments, 0, COMMENT_WEIGHT);
        Console.WriteLine($"Awarded {scoreAwardedForComments} for {numLinesComments} lines of comments");

        //Number of stargazers
        if(urlInfo.getType() == "github" || urlInfo.getType() == "both")
        {
            if(urlInfo.githubStargazersCount != 0)
            {
                //award more points for more stargazers (more popular)
                int numStargazers = urlInfo.githubStargazers;
                float scoreAwardedForContributors = CONTRIBUTOR_WEIGHT * (2 * numStargazers/100);
                finalScore += Math.Clamp(scoreAwardedForContributors, 0, CONTRIBUTOR_WEIGHT);

                Console.WriteLine($"Awarded {scoreAwardedForContributors} for {numStargazers} Stargazers");
            }
            else
            {
                Console.WriteLine("No Stargazers, awarding no points for Stargazers.");
            }
        }
        else if (urlInfo.getType() == "npm")
        {
            //npm has maintainers instead of stargazers
            int numMaintainers = urlInfo.npmMaintainers.Length / 2;
            float scoreAwardedForMaintainers = CONTRIBUTOR_WEIGHT * (numMaintainers/30);
            finalScore += Math.Clamp(scoreAwardedForMaintainers, 0, CONTRIBUTOR_WEIGHT);
            Console.WriteLine($"Awarded {scoreAwardedForMaintainers} for {numMaintainers} Maintainers");
        }
        else
        {
            Console.WriteLine("No Type, Awarding no points for contributors");
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
        Console.WriteLine($"Awarded {HEALTH_WEIGHT * health_score/2} for health score of {health_score}/2");




        return finalScore;
    }
}