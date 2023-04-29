using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IO.Swagger.CLI
{
    //License
    //Pull Request
    public static class Grader
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
        public static float GetRampupTimeScore(URLInfo urlInfo)
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
        public static float GetResponseMaintainerScore(URLInfo urlInfo)
        {   
            Console.WriteLine("-----\nCalculating Response Maintainer");

            float finalScore = 0;
            float STATUS_WEIGHT = 0;
            float UPDATE_WEIGHT = 0;
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
                UPDATE_WEIGHT += 0.15f;

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
                UPDATE_WEIGHT += 0.4f;
            
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
            finalScore = MAINTAINER_WEIGHT + STATUS_WEIGHT + ISSUES_WEIGHT + UPDATE_WEIGHT;
            urlInfo.responseMaintainer_score = finalScore;
            return finalScore;
        }

        public static float GetDependencyScore(URLInfo urlInfo)
        {
            Console.WriteLine("-----\nCalculating Dependency");
            String? line;
            String[] lineArr;
            bool dependency = false;
            float totalDependencies = 0;
            float pinnedDependencies = 0;

            if (urlInfo.packageJsonPath == "" | urlInfo.packageJsonPath == "none")
            {
                Console.WriteLine("**WARNING** Package does not have a package.json file");
                return -1;
            }

            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(urlInfo.packageJsonPath);
                
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    if (line.Contains(" \"dependencies\":"))
                    {
                        if (!line.Contains("}"))
                        {
                            dependency = true;
                        }
                        line = sr.ReadLine();
                        break;
                    }
                        
                    line = sr.ReadLine();
                }
                
                //If there are dependencies, check them
                if (dependency == true)
                {
                    while (!line.Contains("}"))
                    {
                        Console.WriteLine(line);
                        lineArr = line.Split('"');

                        pinnedDependencies += CheckPinned(lineArr[3]);
                        totalDependencies += 1;

                        line = sr.ReadLine();
                    }

                    sr.Close();
                    Console.WriteLine("Pinned Dependencies: " + pinnedDependencies);
                    Console.WriteLine("Total Dependencies: " + totalDependencies);
                    return pinnedDependencies/totalDependencies;
                }
                
                sr.Close();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return 0;
            }

        }

        private static int CheckPinned(string dependency)
        {
            Console.WriteLine("Checking " + dependency);
            //Check if dependency is contains a letter
            if (dependency.Any(x => char.IsLetter(x)))
            {
                //if dependency contains a letter not x
                if (dependency.Any(x => char.IsLetter(x) && (x != 'x' || x != 'X')))
                {
                    return 0;
                }

                string[] dependencyRange = dependency.Split('.');
                
                //Check if major contains a letter x
                if (dependencyRange[0].Contains('x') || dependencyRange[0].Contains('X'))
                {
                    return 0;
                }
                //Check if minor contains a letter x
                if (dependencyRange[1].Contains('x') || dependencyRange[1].Contains('X'))
                {
                    return 0;
                }

                return 1;
            }
            //Check if dependency starts with a carat
            if (dependency.StartsWith('^'))
            {
                if (dependency[1] == '0')
                {
                    return 1;
                }
                
                return 0;
            }
            //Check if dependency starts with a tilde
            else if (dependency.StartsWith('~'))
            {
                if (dependency.Contains('.'))
                {
                    Console.WriteLine(" Dependency has a period");
                    return 1;
                }
                return 0;
            }
            //Check if dependency has a dash
            else if (dependency.Contains('-'))
            {
                string[] dependencyRange = dependency.Split('-');
                //Major version is not the same
                if (dependencyRange[0][0] == dependencyRange[1][0])
                {
                    return 1;
                }

                //Minor version is not the same
                if (dependencyRange[0].Split('.')[1] == dependencyRange[1].Split('.')[1])
                {
                    return 1;
                }

                return 0;
            }
            //Check if dependency starts with a number
            else if (Char.IsDigit(dependency[0]))
            {
                if (dependency.Contains('.'))
                {
                    return 1;
                }

                return 0;
            }
            //Check if dependency is a wildcard
            else if (dependency[0] == '*')
            {
                return 0;
            }
            
            Console.WriteLine("Unknowned Dependency");
            return 0;
        }
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
        public static float GetCorrectnessScore(URLInfo urlInfo)
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

                if (urlInfo.githubStargazers <= 10){
                    STARGAZERS_WEIGHT += 0;
                } else if (urlInfo.githubStargazers <= 50){
                    STARGAZERS_WEIGHT += .10f;
                } else if (urlInfo.githubStargazers <= 100){
                    STARGAZERS_WEIGHT += .20f;
                } else {
                    STARGAZERS_WEIGHT += .20f;
                }

            }
            else if (urlInfo.getType() == "npm")
            {
                if ((urlInfo.npmMaintainers).Count() == 0){
                    MAINTAINERS_WEIGHT += 0;
                } else if ((urlInfo.npmMaintainers).Count() == 10){
                    MAINTAINERS_WEIGHT += .10f;
                } else if ((urlInfo.npmMaintainers).Count() == 20){
                    MAINTAINERS_WEIGHT += .20f;
                } else if ((urlInfo.npmMaintainers).Count() == 30){
                    MAINTAINERS_WEIGHT += .30f;
                } else if ((urlInfo.npmMaintainers).Count() == 50){
                    MAINTAINERS_WEIGHT += .40f;
                } else if ((urlInfo.npmMaintainers).Count() >= 51){
                    MAINTAINERS_WEIGHT += .50f;
                }
                if (urlInfo.npmVersions.Length == 0){
                    VERSIONS_WEIGHT += 0;
                } else if (urlInfo.npmVersions.Length <= 10){
                    VERSIONS_WEIGHT += .10f;
                } else if (urlInfo.npmVersions.Length <= 25){
                    VERSIONS_WEIGHT += .20f;
                } else if (urlInfo.npmVersions.Length <= 50){
                    VERSIONS_WEIGHT += .30f;
                } else if (urlInfo.npmVersions.Length >= 80){
                    VERSIONS_WEIGHT += .40f;
                }
            }
            else
            {
                Console.WriteLine("No Type, Awarding no points");
            }
            finalScore = MAINTAINERS_WEIGHT + STATUS_WEIGHT + ISSUES_WEIGHT + VERSIONS_WEIGHT + STARGAZERS_WEIGHT;
            urlInfo.correctness_score = finalScore;
            return finalScore;
        }
        public static float GetBusFactorScore(URLInfo urlInfo)
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

        public static int GetLicenseScore(URLInfo urlInfo, string []LicenseList)
        {
            /*if (urlInfo.licensePath == "" || urlInfo.licensePath == null)
            {
                urlInfo.license = "Not Available";
                return 0;
            }*/
            if (urlInfo.license == "" || urlInfo.license == null) {
                urlInfo.license = "Not Available";
                return 0;
            }
            //string License = File.ReadLines(urlInfo.licensePath).First(); // gets the first line from file.
            string License = urlInfo.license;
            var allLicenses = LicenseList;
            var allLicensesArr = allLicenses;
            List<string> allLicensesArrCleaned = new List<string>();
            foreach (string li in allLicensesArr)
            {
                string newli = li.Trim().ToLower();
                allLicensesArrCleaned.Add(newli);
            }



            foreach (var LicenseVar in allLicensesArrCleaned)
            {
                string cleanedlicense = License.Trim().ToLower();
                string CleanedLicenseVar = LicenseVar.Trim().ToLower();
                if(cleanedlicense.Contains(CleanedLicenseVar))
                {
                    urlInfo.license = CleanedLicenseVar;
                    return 1;
                }
            }
            urlInfo.license = "Not Available";
            return 0;
        }
        public static float GetNetScore(URLInfo urlInfo)
        {
            float netScore = 0;
            netScore = urlInfo.license_score + urlInfo.rampUp_score + urlInfo.busFactor_score + urlInfo.correctness_score + urlInfo.responseMaintainer_score + urlInfo.dependency_score + urlInfo.pullreview_score;
            netScore = netScore / 7;
            return netScore;
        }

        public static float GetPullRequestsScore(URLInfo urlInfo)
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
}