namespace StaticAnalysis;
public static class Maintainer
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
        Console.WriteLine("-----\nCalculating Response Maintainer");

        float finalScore = 0;
        float STATUS_WEIGHT = 0;
        //float UPDATE_WEIGHT = 0;
        float ISSUES_WEIGHT = 0;
        float MAINTAINER_WEIGHT = 0;


        //Count issues
        if(urlInfo.getType() == "github" || urlInfo.getType() == "both")
        {
            //award less points for more issues
            int numissues = urlInfo.githubIssues;
            int openissues = urlInfo.githubOpenIssues;
            float issueratio = 0;
            if (numissues != 0){
                issueratio = openissues/numissues;
            }
            if (issueratio <= 0.1){
                ISSUES_WEIGHT += .50f;
            }else if (issueratio <= 0.2){
                ISSUES_WEIGHT += .40f;
            }else if (issueratio <= 0.3){
                ISSUES_WEIGHT += .30f;
            }else if (issueratio <= 0.4){
                ISSUES_WEIGHT += .20f;
            }else if (issueratio <= 0.5){
                ISSUES_WEIGHT += .10f;
            }else{
                ISSUES_WEIGHT += 0;
            }


            //Checking Status
            if (urlInfo.githubIsEmpty){
                STATUS_WEIGHT += 0;
            } else if (urlInfo.githubIsDisabled){
                STATUS_WEIGHT += 0;
            } else if (urlInfo.githubIsPrivate){
                STATUS_WEIGHT += .10f;
            } else if (urlInfo.githubIsFork){
                STATUS_WEIGHT += .15f;
            } else {
                STATUS_WEIGHT += .30f;
            }


            /*
            //Checking Updates
            if (urlInfo.githubUpdatedAt >= DateTime.Now - DateTime.Now.AddDays(-90)){
                UPDATE_WEIGHT += .20f;
            } else if (urlInfo.githubUpdatedAt >= DateTime.Now - DateTime.Now.AddDays(-80)){
                UPDATE_WEIGHT += .15f;
            } else if (urlInfo.githubUpdatedAt >= DateTime.Now - DateTime.Now.AddDays(-365)){
                UPDATE_WEIGHT += .10f;
            } else if (urlInfo.githubUpdatedAt >= DateTime.Now - DateTime.Now.AddDays(-730)){
                UPDATE_WEIGHT += .5f;
            } else {
                UPDATE_WEIGHT += 0;
            }*/

        }
        else if (urlInfo.getType() == "npm")
        {
            /*if (urlInfo.npmTimes >= DateTime.Now - DateTime.Now.AddDays(-90)){
                UPDATE_WEIGHT += .50f;
            } else if (urlInfo.npmTimes >= DateTime.Now - DateTime.Now.AddDays(-80)){
                UPDATE_WEIGHT += .40f;
            } else if (urlInfo.npmTimes >= DateTime.Now - DateTime.Now.AddDays(-365)){
                UPDATE_WEIGHT += .30f;
            } else if (urlInfo.npmTimes >= DateTime.Now - DateTime.Now.AddDays(-730)){
                UPDATE_WEIGHT += .20f;
            } else {
                UPDATE_WEIGHT += 0;
            }*/
        
            if ((urlInfo.npmMaintainers).Count() == 0){
                MAINTAINER_WEIGHT += 0;
            } else if ((urlInfo.npmMaintainers).Count() == 10){
                MAINTAINER_WEIGHT += .10f;
            } else if ((urlInfo.npmMaintainers).Count() == 20){
                MAINTAINER_WEIGHT += .20f;
            } else if ((urlInfo.npmMaintainers).Count()== 30){
                MAINTAINER_WEIGHT += .30f;
            } else if ((urlInfo.npmMaintainers).Count() == 50){
                MAINTAINER_WEIGHT += .40f;
            } else if ((urlInfo.npmMaintainers).Count() >= 51){
                MAINTAINER_WEIGHT += .50f;
            }
        }
        else
        {
            Console.WriteLine("No Type, Awarding no points");
        }
        finalScore = MAINTAINER_WEIGHT + STATUS_WEIGHT + ISSUES_WEIGHT;
        urlInfo.responseMaintainer_score = finalScore;
        return finalScore;
    }
}