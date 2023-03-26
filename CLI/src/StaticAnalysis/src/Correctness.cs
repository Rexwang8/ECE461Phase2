namespace StaticAnalysis;
public class Maintainer
{
    /*
    Responsive Maintainer


    FOR GITHUB
    Metric   Weight
    ------------------
    Issues                              50%
    Status                              30%
    Stargazers                          20%

    FOR NPM
    Metric   Weight
    ------------------
    Maintainers                         50%
    Version Count                       50%
    */
    public static void GetScore(URLInfo urlInfo)
    {   
        Console.WriteLine("-----\nCalculating Correctness");

        float finalScore = 0;
        float STATUS_WEIGHT = 0;
        float STARGAZERS_WEIGHT = 0;
        float ISSUES_WEIGHT = 0;
        float MAINTAINERS_WEIGHT = 0;
        float VERSIONS_WEIGHT = 0;


        //Count issues
        if(urlInfo.getType() == "github" || urlInfo.getType() == "both")
        {
            //award less points for more issues
            int numissues = urlInfo.githubIssues;
            int openissues = urlInfo.githubOpenIssues;
            float issueratio = openissues/numissues;
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

            if (urlInfo.githubStargazers <= 10){
                STARGAZERS_WEIGHT += 0;
            } else if (urlInfo.githubStargazers <= 50){
                STARGAZERS_WEIGHT += 10;
            } else if (urlInfo.githubStargazers <= 100){
                STARGAZERS_WEIGHT += 20;
            } else {
                STARGAZERS_WEIGHT += 20;
            }

        }
        else if (urlInfo.getType() == "npm")
        {
            if (Convert.ToInt32(urlInfo.npmMaintainers) == 0){
                MAINTAINERS_WEIGHT += 0;
            } else if (Convert.ToInt32(urlInfo.npmMaintainers) == 10){
                MAINTAINERS_WEIGHT += .10f;
            } else if (Convert.ToInt32(urlInfo.npmMaintainers) == 20){
                MAINTAINERS_WEIGHT += .20f;
            } else if (Convert.ToInt32(urlInfo.npmMaintainers) == 30){
                MAINTAINERS_WEIGHT += .30f;
            } else if (Convert.ToInt32(urlInfo.npmMaintainers) == 50){
                MAINTAINERS_WEIGHT += .40f;
            } else if (Convert.ToInt32(urlInfo.npmMaintainers) >= 51){
                MAINTAINERS_WEIGHT += .50f;
            }
            if (urlInfo.npmVersions.Length == 0){
                VERSIONS_WEIGHT += 0;
            } else if (urlInfo.npmVersions.Length <= 10){
                VERSIONS_WEIGHT += 10;
            } else if (urlInfo.npmVersions.Length <= 25){
                VERSIONS_WEIGHT += 20;
            } else if (urlInfo.npmVersions.Length <= 50){
                VERSIONS_WEIGHT += 30;
            } else if (urlInfo.npmVersions.Length >= 80){
                VERSIONS_WEIGHT += 40;
            }
        }
        else
        {
            Console.WriteLine("No Type, Awarding no points");
        }
        finalScore = MAINTAINERS_WEIGHT + STATUS_WEIGHT + ISSUES_WEIGHT + VERSIONS_WEIGHT + STARGAZERS_WEIGHT;
        urlInfo.responseMaintainer_score = finalScore;
    }
}