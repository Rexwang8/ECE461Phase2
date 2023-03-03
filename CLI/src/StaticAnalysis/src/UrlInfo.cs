using Utility;
using Newtonsoft.Json;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL.Client.Http;

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
        #region Variables
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
        public string license { get; set; }
        public int licenseCompatibility { get; set; }

        //npm specific
        public string npmDescription { get; set; }
        public string npmReadme { get; set; }
        public string npmHomepage { get; set; }
        public string[] npmMaintainers { get; set; }
        public string[] npmVersions { get; set; }
        public string[] npmTimes { get; set; }

        //github specific
        public string githubDescription { get; set; }
        public string githubHomepage { get; set; }
        public string githubCreatedAt { get; set; }
        public string githubUpdatedAt { get; set; }
        public int githubDiskUsage { get; set; }
        public string githubURL2 { get; set; }
        public bool githubIsFork { get; set; }
        public bool githubIsArchived { get; set; }
        public bool githubIsDisabled { get; set; }
        public bool githubIsLocked { get; set; }
        public bool githubIsPrivate { get; set; }
        public bool githubIsEmpty { get; set; }
        public string githubLicense { get; set; } = "none";
        public string githubReleaseName { get; set; } = "none";
        public string githubReleasePublishedAt { get; set; } = "none";
        public string githubReleaseDescription { get; set; } = "none";
        public string githubReleaseURL { get; set; } = "none";

        public int githubWatchers { get; set; } = -1;
        public int githubStargazers { get; set; } = -1;
        public int githubForks { get; set; } = -1;
        public int githubOpenIssues { get; set; } = -1;
        public int githubIssues { get; set; } = -1;
        public List<QLMergedPullRequestNode> githubMergedPullRequests { get; set; } = new List<QLMergedPullRequestNode>();
        public int githubMergedPullRequestsCount { get; set; } = -1;

        public int githubStargazersCount { get; set; } = -1;
        public int githubDiscussions { get; set; } = -1;
        public int githubOpenPullRequests { get; set; } = -1;
        #endregion

        #region Constructors
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

            //github specific
            githubDescription = "";
            githubHomepage = "";
            githubCreatedAt = "";
            githubUpdatedAt = "";
            githubDiskUsage = 0;
            githubURL2 = "";

            githubIsFork = false;
            githubIsArchived = false;
            githubIsDisabled = false;
            githubIsLocked = false;
            githubIsPrivate = false;
            githubIsEmpty = false;

        }

        #endregion
        
        #region API calls
        public async Task<APIError> PullNpmInfo(Logger logger)
        {
            APIError error = new APIError();

            HttpClient client = new HttpClient();
            string currentDirectory = Directory.GetCurrentDirectory();

            if (type != "npm" && type != "both")
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
            if (response.IsSuccessStatusCode)
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
            if (jsoncontent._id == null)
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

        public async Task<APIError> PullGithubInfo(Logger logger, string token)
        {
            APIError error = new APIError();

            if (type != "github" && type != "both")
            {
                error.SetError("Invalid URL for github pull or other issue with type " + baseURL, APIError.errorType.invalidtype);
                logger.Log(error.ToString(), 2);
                return error;
            }

            if (isInvalid || name == "none" || name == null || name == "")
            {
                error.SetError("Empty github url or invalid package." + baseURL, APIError.errorType.invalidtype);
                Console.WriteLine("name: " + name);
                logger.Log(error.ToString(), 2);
                return error;
            }


            string owner = githubURL.Split("/")[3];
            string repo = githubURL.Split("/")[4];
            Console.WriteLine("Calling github api with owner: " + owner + " and repo: " + repo);

            string uri = "https://api.github.com/graphql";
            // Create GraphQL client
            var graphQLClient = new GraphQLHttpClient(uri, new NewtonsoftJsonSerializer());
            // Create query request
            var graphQLRequest = new GraphQL.GraphQLRequest
            {
                Query = @$"query {{
                        repository(name:""{name}"", owner:""{owner}"") {{ 
                            name
                            description
                            createdAt
                            updatedAt
                            diskUsage
                            url
                            homepageUrl
                            isArchived
                            isDisabled
                            isFork
                            isLocked
                            isPrivate
                            isEmpty
                            licenseInfo {{
                                name
                                spdxId
                            }}
                            releases(last: 1) {{
                        nodes {{
                            name
                            description
                            url
                            publishedAt
                        }}
                    }}
                    watchers {{
                        totalCount
                    }}
                    issues {{
                        totalCount
                    }}
                    openIssues: issues(states: OPEN) {{
                        totalCount
                    }}
                    forks {{
                        totalCount
                    }}
                    pullRequests(states: MERGED, first: 100) {{
                        totalCount
                        nodes {{
                            title
                            comments {{
                                totalCount
                            }}
                        }}
                    }}
                    openPullRequests: pullRequests(states: OPEN) {{
                        totalCount
                    }}
                    discussions {{
                        totalCount
                    }}
                    stargazers {{
                        totalCount
                    }}

                    }}
                    }}"
            
            };
           // Add auth token
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Get response
            var graphQLResponse = await graphQLClient.SendQueryAsync<QLResponse>(graphQLRequest);
            QLResponse resp = graphQLResponse.Data;
            if(graphQLResponse.Errors != null)
            /*
            Console.WriteLine("Response from github api: " + graphQLResponse.Errors[0].Message);
            Console.WriteLine("Response from github api: " + resp);

            Console.WriteLine("Name of repo is: " + resp.repository.name);
            Console.WriteLine("Description of repo is: " + resp.repository.description);
            Console.WriteLine("Created at: " + resp.repository.createdAt);
            Console.WriteLine("Updated at: " + resp.repository.updatedAt);
            Console.WriteLine("Disk usage: " + resp.repository.diskUsage);
            Console.WriteLine("URL: " + resp.repository.url);
            Console.WriteLine("Homepage URL: " + resp.repository.homepageUrl);
            Console.WriteLine("Flags: " + resp.repository.isArchived + " " + resp.repository.isDisabled + " " + resp.repository.isFork + " " + resp.repository.isLocked + " " + resp.repository.isPrivate + " " + resp.repository.isEmpty);
            Console.WriteLine("License: " + resp.repository.licenseInfo.name + " " + resp.repository.licenseInfo.spdxId);
            if(resp.repository.releases.nodes.Count > 0)
            Console.WriteLine("Release: " + resp.repository.releases.nodes[0].name + " " + resp.repository.releases.nodes[0].description + " " + resp.repository.releases.nodes[0].url + " " + resp.repository.releases.nodes[0].publishedAt);
            Console.WriteLine("Watchers: " + resp.repository.watchers.totalCount);
            Console.WriteLine("Issues: " + resp.repository.issues.totalCount);
            Console.WriteLine("Open issues: " + resp.repository.openIssues.totalCount);
            Console.WriteLine("Forks: " + resp.repository.forks.totalCount);
            Console.WriteLine("Pull requests: " + resp.repository.pullRequests.totalCount);
            if(resp.repository.pullRequests.nodes.Count > 0)
            Console.WriteLine("Pull request comments: " + resp.repository.pullRequests.nodes[0].comments.totalCount);
            Console.WriteLine("Open pull requests: " + resp.repository.openPullRequests.totalCount);
            Console.WriteLine("Discussions: " + resp.repository.discussions.totalCount);
            Console.WriteLine("Stargazers: " + resp.repository.stargazers.totalCount);
            */

            //load data into urlinfo
            githubDescription = resp.repository.description;
            githubCreatedAt = resp.repository.createdAt;
            githubUpdatedAt = resp.repository.updatedAt;
            githubDiskUsage = resp.repository.diskUsage;
            githubURL2 = resp.repository.url;
            githubHomepage = resp.repository.homepageUrl;
            githubIsArchived = resp.repository.isArchived;
            githubIsDisabled = resp.repository.isDisabled;
            githubIsFork = resp.repository.isFork;
            githubIsLocked = resp.repository.isLocked;
            githubIsPrivate = resp.repository.isPrivate;
            githubIsEmpty = resp.repository.isEmpty;
            githubLicense = resp.repository.licenseInfo.name;
            if(resp.repository.releases.nodes.Count > 0)
            {
                githubReleaseName = resp.repository.releases.nodes[0].name;
                githubReleaseDescription = resp.repository.releases.nodes[0].description;
                githubReleaseURL = resp.repository.releases.nodes[0].url;
                githubReleasePublishedAt = resp.repository.releases.nodes[0].publishedAt;
            }
            githubWatchers = resp.repository.watchers.totalCount;
            githubIssues = resp.repository.issues.totalCount;
            githubOpenIssues = resp.repository.openIssues.totalCount;
            githubForks = resp.repository.forks.totalCount;
            githubMergedPullRequests = resp.repository.pullRequests.nodes;
            githubMergedPullRequestsCount = resp.repository.pullRequests.totalCount;
            githubOpenPullRequests = resp.repository.openPullRequests.totalCount;
            githubDiscussions = resp.repository.discussions.totalCount;
            githubStargazers = resp.repository.stargazers.totalCount;



            


            return error;
        }

        #endregion

        #region Initializers
        public void initName()
        {
            if (isInvalid)
            {
                Console.WriteLine("Invalid URL");
                return;
            }

            //return all text after substring "package/" for npm
            if (type == "npm")
            {
                int ix = baseURL.IndexOf("package/");
                if (ix >= 0)
                {
                    name = baseURL.Substring(ix + 8);
                    Console.WriteLine("Name: " + name);
                    return;
                }
            }
            else
            {
                //it is a github name
                name = baseURL.Split("/")[4];
                Console.WriteLine("Name: " + name);
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

        #endregion

        #region Setter
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

        #endregion
        
        #region Getter
        public string getInfo()
        {
            return "{name: " + name + ", github url: " + githubURL + ", npm url: " + npmURL + ", type: " + type + ", path: " + path + "}";
        }

        public string getNPMInfo()
        {
            return "{name: " + name + ", description length: " + npmDescription.Length + ", readme length: " + npmReadme.Length + ", homepage: " + npmHomepage + ", maintainers: " + npmMaintainers + ", versions: " + npmVersions + ", times: " + npmTimes + ", license: " + license + "}";

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
    
        #endregion
    }

    #region QLJSON Classes
    class QLResponse
    {
        public QLrepository repository { get; set; } = new QLrepository();
    }

    class QLrepository
    {
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public string createdAt { get; set; } = "";
        public string updatedAt { get; set; } = "";
        public int diskUsage { get; set; } = 0;
        public string url { get; set; } = "";
        public string homepageUrl { get; set; } = "";

        //flags
        public bool isArchived { get; set; } = false;
        public bool isDisabled { get; set; } = false;
        public bool isFork { get; set; } = false;
        public bool isLocked { get; set; } = false;
        public bool isPrivate { get; set; } = false;
        public bool isEmpty { get; set; } = false;

        //license
        public QLLicenseInfo licenseInfo { get; set; } = new QLLicenseInfo();

        //releases
        public QLRelease releases { get; set; } = new QLRelease();

        //watchers
        public QLWatchers watchers { get; set; } = new QLWatchers();

        //issues
        public QLIssues issues { get; set; } = new QLIssues();

        //open issues
        public QLOpenIssues openIssues { get; set; } = new QLOpenIssues();
        //forks
        public QLForks forks { get; set; } = new QLForks();

        //merged pulls
        public QLMergedPullRequest pullRequests { get; set; } = new QLMergedPullRequest();

        //stargazers
        public QLStargazers stargazers { get; set; } = new QLStargazers();

        //open PRs
        public QLOpenPullRequestCount openPullRequests { get; set; } = new QLOpenPullRequestCount();

        //discussions
        public QLDiscussions discussions { get; set; } = new QLDiscussions();
    }

    class QLLicenseInfo
    {
        public string name { get; set; } = "";
        public string spdxId { get; set; } = "";
    }

    class QLRelease
    {
        public List<QLReleaseNode> nodes { get; set; } = new List<QLReleaseNode>();

    }
    class QLReleaseNode
    {
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public string url { get; set; } = "";
        public string publishedAt { get; set; } = "";
    }

    class QLWatchers
    {
        public int totalCount { get; set; } = 0;
    }

    class QLStargazers
    {
        public int totalCount { get; set; } = 0;
    }

    class QLForks
    {
        public int totalCount { get; set; } = 0;
    }

    class QLIssues
    {
        public int totalCount { get; set; } = 0;
    }

    class QLOpenIssues
    {
        public int totalCount { get; set; } = 0;
    }

    public class QLMergedPullRequestNode
    {
        public string title { get; set; } = "";
        public QLMergedPullRequestNodeCommentsCount comments { get; set; } = new QLMergedPullRequestNodeCommentsCount();
    }

    public class QLMergedPullRequestNodeCommentsCount
    {
        public int totalCount { get; set; } = 0;
    }

    class QLMergedPullRequest
    {
        public List<QLMergedPullRequestNode> nodes { get; set; } = new List<QLMergedPullRequestNode>();
        public int totalCount { get; set; } = 0;
    }

    class QLOpenPullRequestCount
    {
        public int totalCount { get; set; } = 0;
    }

    class QLDiscussions
    {
        public int totalCount { get; set; } = 0;
    }

    #endregion
}