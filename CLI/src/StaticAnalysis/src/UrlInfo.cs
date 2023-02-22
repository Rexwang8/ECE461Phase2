using Utility;
using System.Net.Http.Headers;
namespace StaticAnalysis
{
    public class URLInfo
    {
        bool isInvalid = false;

        string baseURL { get; set; } = "none";

        string name { get; set; }
        string githubURL { get; set; }
        string npmURL { get; set; }
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
            githubURL = "none";
            npmURL = "none";
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

        //API calls
        /* public async Task PullNpmInfo(Logger logger)
        {
            HttpClient client = new HttpClient();
            string currentDirectory = Directory.GetCurrentDirectory();

            if ((type != "npm" || type != "both") || isInvalid || npmUrl == "none" || npmUrl == null || npmUrl == "")
            {
                logger.Log("Invalid URL for npm pull or other issue with type" + " " + baseURL, 1);
            }
            
            //call npm api
            HttpResponseMessage response = await ClientCertificateOption.GetAsync(npmUrl);
            if(response.IsSuccessStatusCode)
            {
                logger.Log("Response from registry.npmjs.org: " + response.StatusCode, 1);
            }
            else
            {
                logger.Log("Response from registry.npmjs.org: " + response.StatusCode, 1);
                Environment.Exit(1);
            }            
            
            //decode json
            string responseBody = await response.Content.ReadAsStringAsync();
            var npmpath = Path.Combine(currentDirectory, "data/npm/" + path + ".json");
            //write to object
            if (!File.Exists(npmpath))
            {
                File.Create(npmpath);
            }
            File.WriteAllText(@npmpath, responseBody);
            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
        } */

        public void PullGitInfo(Logger logger, string token)
        {
            if ((type != "git" || type != "both") || isInvalid || githubURL == "none" || githubURL == null || githubURL == "")
            {
                logger.Log("Invalid URL for git pull or other issue with type" + " " + baseURL, 1);
            }

            //call git api

            //decode json

            //write to object
        }
        
        //-----------------------

        //Initializers
        public void initName()
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

        public string initType()
        {
            if (baseURL.Contains("https://github.com"))
            {
                githubURL = baseURL;
                type = "github";
                return "github";
            }
            else if (baseURL.Contains("https://www.npmjs.com"))
            {
                npmURL = baseURL;
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
    
       

        //Setter
        public void setGithubURL(string url)
        {
            githubURL = url;
        }
        public void setNpmURL(string url)
        {
            npmURL = url;
        }
        public void setTypeName(string name)
        {
            type = name;
        }
        


        //Getter
        public string getInfo()
        {
            return "{name: " + name + ", github url: " + githubURL + ", npm url: " + npmURL + ", type: " + type + ", path: " + path + "}";
        }
        public string getURL()
        {
            return baseURL;
        }

        public string getName()
        {
            return name;
        }

        public string getGithubUrl()
        {
            return githubURL;
        }

        public string getNpmUrl()
        {
            return npmURL;
        }

        public string getType()
        {
            return type;
        }

        public bool getIsInvalid()
        {
            return isInvalid;
        }
    }
}