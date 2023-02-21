using Utility;

namespace StaticAnalysis
{
    public class URLInfo
    {
        bool isInvalid = false;

        string baseURL { get; set; } = "none";

        string name { get; set; }
        string githubUrl { get; set; }
        string npmUrl { get; set; }
        string type { get; set; }
        string path { get; set; }

        int license_score { get; set; }
        float rampUp_score { get; set; }
        float busFactor_score { get; set; }
        float correctness_score { get; set; }
        float responseMaintainer_score { get; set; }
        float net_score { get; set; }



        public URLInfo(string url)
        {
            name = "none";
            githubUrl = "none";
            npmUrl = "none";
            type = "none";
            path = "none";
            license_score = 0;
            rampUp_score = 0;
            busFactor_score = 0;
            correctness_score = 0;
            responseMaintainer_score = 0;
            net_score = 0;
            baseURL = url.Trim().ToLower();
        }



        public string getInfo()
        {
            return "{name: " + name + ", github url: " + githubUrl + ", npm url: " + npmUrl + ", type: " + type + ", path: " + path + "}";
        }

        public void getName()
        {
            if(isInvalid)
            {
                Console.WriteLine("Invalid URL");
                return;
            }

            String[] splitUrl = baseURL.Split("/");
            name = splitUrl[splitUrl.Length - 1];
            return;
        }

        public string getType()
        {
            if (baseURL.Contains("https://github.com"))
            {
                githubUrl = baseURL;
                type = "github";
                return "github";
            }
            else if (baseURL.Contains("https://www.npmjs.com"))
            {
                npmUrl = baseURL;
                type = "npm";
                return "npm";
            }
            else
            {
                isInvalid = true;
                type = "invalid";
                return "invalid";
            }
        }
    
        //API calls
        public void PullNpmInfo(Logger logger)
        {
            if ((type != "npm" || type != "both") || isInvalid || npmUrl == "none" || npmUrl == null || npmUrl == "")
            {
                logger.Log("Invalid URL for npm pull or other issue with type" + " " + baseURL, 1);
            }

            //call npm api

            //decode json

            //write to object
        }

        public void PullGitInfo(Logger logger, string token)
        {
            if ((type != "git" || type != "both") || isInvalid || githubUrl == "none" || githubUrl == null || githubUrl == "")
            {
                logger.Log("Invalid URL for git pull or other issue with type" + " " + baseURL, 1);
            }

            //call git api

            //decode json

            //write to object
        }



        //setter
        public void setGithubURL(string url)
        {
            githubUrl = url;
        }
        public void setNpmURL(string url)
        {
            npmUrl = url;
        }
        public void setTypeName(string name)
        {
            type = name;
        }


        //Getter
        public string returnURL()
        {
            return baseURL;
        }

        public string returnName()
        {
            return name;
        }

        public string returnGithubUrl()
        {
            return githubUrl;
        }

        public string returnNpmUrl()
        {
            return npmUrl;
        }

        public string returnType()
        {
            return type;
        }

        public bool returnIsInvalid()
        {
            return isInvalid;
        }
    }




}