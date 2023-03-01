using Utility;
using System.Net.Http;
using Newtonsoft.Json;

namespace StaticAnalysis
{
    public class APIError
    {
        public string message { get; set; } = "none";
        public enum errorType { none, invalidtype, badresponse };

        public errorType typeOfError { get; set; } = errorType.none;

        public APIError()
        {
            message = "none";
            typeOfError = errorType.none;
        }

        public APIError(string message, errorType type)
        {
            this.message = message;
            typeOfError = type;
        }
        public override string ToString()
        {
            return "Error: " + message + " Type: " + typeOfError.ToString();
        }

        public string GetError()
        {
            return message;
        }


        public void SetError(string message, errorType type)
        {
            this.message = message;
            typeOfError = type;
        }

        public errorType GetErrType()
        {
            return typeOfError;
        }
    }


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

        public int codeLineCount { get; set; } //lines if code
        public int commentLineCount { get; set; } // lines of comments
        public int codeCharCount { get; set; } //code characters
        public int commentCharCount { get; set; }
        public string licensePath { get; set; }
        public string readmePath { get; set; }
        public string license {get; set; }
        public int licenseCompatibility {get; set; }

        //npm specific
        public string npmDescription { get; set; }
        public string npmReadme { get; set; }
        public string npmHomepage { get; set; }
        public string[] npmMaintainers { get; set; }
        public string[] npmVersions { get; set; }
        public string[] npmTimes { get; set; }

        
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

            codeLineCount = 0; 
            commentLineCount = 0;
            codeCharCount = 0;
            commentCharCount = 0;
            licensePath = "";
            readmePath = "";
            license = "";

            //npm specific
            npmDescription = "";
            npmReadme = "";
            npmHomepage = "";
            npmMaintainers = new string[0];
            npmVersions = new string[0];
            npmTimes = new string[0];
        }

        //API calls
        public async Task<APIError> PullNpmInfo(Logger logger)
        {
            APIError error = new APIError();

            HttpClient client = new HttpClient();
            string currentDirectory = Directory.GetCurrentDirectory();

            if(type != "npm" && type != "both")
            {
                error.SetError("Invalid URL for npm pull or other issue with type " + baseURL, APIError.errorType.invalidtype);
                logger.Log(error.ToString(), 2);
                return error;
            }

            if (isInvalid || name == "none" || name == null || name == "")
            {
                error.SetError("Empty npm url or invalid package." + baseURL, APIError.errorType.invalidtype);
                logger.Log(error.ToString(), 2);
                return error;
            }
            
            //call npm api
            string npmURLRegistry = "https://registry.npmjs.org/" + name;
            logger.Log("Calling npm api with url: " + npmURLRegistry, 1);
            Console.WriteLine("Calling npm api with url: " + npmURLRegistry);
            HttpResponseMessage response = await client.GetAsync(npmURLRegistry);
            if(response.IsSuccessStatusCode)
            {
                logger.Log("Response from registry.npmjs.org: " + response.StatusCode, 1);
                //Console.WriteLine("Response from registry.npmjs.org: " + response.StatusCode);
            }
            else
            {
                error.SetError("Response from registry.npmjs.org: " + response.StatusCode, APIError.errorType.badresponse);
                logger.Log(error.ToString(), 1);
                //Console.WriteLine("Response from registry.npmjs.org: " + response.StatusCode);
                return error;
            }            
            
            //decode json
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody == null || responseBody == "")
            {
                error.SetError("Response from registry.npmjs.org: " + response.StatusCode, APIError.errorType.badresponse);
                logger.Log(error.ToString(), 1);
                Console.WriteLine("Response from registry.npmjs.org: " + response.StatusCode);
                return error;
            }

            //null forgiving operator
            //Console.WriteLine((JsonConvert.DeserializeObject(responseBody)).ToString().Length);
            //File.WriteAllText(currentDirectory + "/npmresponse.json", (JsonConvert.DeserializeObject(responseBody)).ToString());
            dynamic jsoncontent = JsonConvert.DeserializeObject(responseBody)!;
            if(jsoncontent._id == null)
            {
                error.SetError("Response from registry.npmjs.org: " + response.StatusCode, APIError.errorType.badresponse);
                logger.Log(error.ToString(), 1);
                Console.WriteLine(error.ToString());
                return error;
            }

            //break into data
            string[] times = jsoncontent.time.ToString().Split(',');
            string[] versions = jsoncontent.versions.ToString().Split(',');
            string[] maintainers = jsoncontent.maintainers.ToString().Split(',');
            string description = jsoncontent.description.ToString();
            string readme = jsoncontent.readme.ToString();
            string homepage = jsoncontent.homepage.ToString();
            string repository = jsoncontent.repository.url.ToString().Replace(".git", "").Replace("git+", "").Replace("git://", "https://").Replace("git+ssh://", "https://").Replace("ssh://", "https://").Replace("git+http://", "https://").Replace("git+https://", "https://");
            
            string licensetype = "";
            try
            {
                licensetype = jsoncontent.license.type.ToString();
            }
            catch
            {
                licensetype = jsoncontent.license.ToString();
            }

            //write to urlinfo
            license = licensetype;
            npmDescription = description;
            npmReadme = readme;
            npmHomepage = homepage;
            npmMaintainers = maintainers;
            npmVersions = versions;
            npmTimes = times;
            
            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            return error;
        } 

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

            //return all text after substring "package/"
            int ix = baseURL.IndexOf("package/"); 
            if (ix >= 0)
            {
                name = baseURL.Substring(ix + 8);
                return;
            }
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
        public void setPath(string pathway)
        {
            path = pathway;
        }


        //Getter
        public string getInfo()
        {
            return "{name: " + name + ", github url: " + githubURL + ", npm url: " + npmURL + ", type: " + type + ", path: " + path + "}";
        }

        public string getNPMInfo()
        {
            return "{name: " + name + ", description length: " + npmDescription.Length + ", readme length: " + npmReadme.Length + ", homepage: " + npmHomepage +  ", maintainers: " + npmMaintainers + ", versions: " + npmVersions + ", times: " + npmTimes + ", license: " + license +"}";

        }

        public string getStaticInfo()
        {
            return "{codeLineCount: " + codeLineCount + ", commentLineCount: " + commentLineCount + ", codeCharCount: " + codeCharCount + ", commentCharCount: " + commentCharCount + ", licensePath: " + licensePath + ", readmePath:" + readmePath + "}";
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

        public string getPath()
        {
            return path;
        }
    }
}