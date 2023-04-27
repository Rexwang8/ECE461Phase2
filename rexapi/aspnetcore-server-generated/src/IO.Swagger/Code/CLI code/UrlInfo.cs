using Newtonsoft.Json;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL.Client.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System;
using LibGit2Sharp;
using System.Linq;

namespace IO.Swagger.CLI
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

        public string baseURL { get; set; } = "none";

        public string name { get; set; }
        public string githubURL { get; set; }
        public string npmURL { get; set; }
        public  string type { get; set; }
        public  string path { get; set; }

        public int license_score { get; set; } = -1;
        float rampUp_score { get; set; } = -1;
        float busFactor_score { get; set; } = -1;
        public float correctness_score { get; set; } = -1;
        public float responseMaintainer_score { get; set; } = -1;
        float dependency_score { get; set; } = -1;
        float pullreview_score { get; set; } = -1;
        float net_score { get; set; } = -1;




        //static analysis
        public int codeLineCount { get; set; } //lines if code
        public int commentLineCount { get; set; } // lines of comments
        public int codeCharCount { get; set; } //code characters
        public int commentCharCount { get; set; }
        public string licensePath { get; set; }
        public string readmePath { get; set; }
        public string packageJsonPath {get; set; }
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

        public bool SuccessGrabGithub { get; set; } = false;
        public bool SuccessGrabNpm { get; set; } = false;
        public bool SuccessDoClone { get; set; } = false;
        public bool SuccessDoStaticAnalysis { get; set; } = false;
        public bool SuccessDoLicenseAnalysis { get; set; } = false;
        
        public bool SuccessClone { get; set; }
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
            dependency_score = 0;
            pullreview_score = 0;

            net_score = 0;
            baseURL = url.Trim().ToLower();

            codeLineCount = 0;
            commentLineCount = 0;
            codeCharCount = 0;
            commentCharCount = 0;
            licensePath = "";
            packageJsonPath = "";
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
        
        #region Static Analysis

        public string returnNameFromPackage()
        {
            StreamReader sr = new StreamReader(packageJsonPath);
            string line = sr.ReadLine();

            while (line != null)
            {
                if (line.Contains("\"name\":"))
                {
                    string[] splitLine = line.Split("\"");
                    return splitLine[3];
                }
                line = sr.ReadLine();
            }

            return "none";
        }

        public string returnVersionFromPackage()
        {
            Console.WriteLine("Inside returnVersionFromPackage Function");
            StreamReader sr = new StreamReader(packageJsonPath);
            string line = sr.ReadLine();

            while (line != null)
            {
                if (line.Contains("\"version\":"))
                {
                    string[] splitLine = line.Split("\"");
                    Console.WriteLine(splitLine[3]);
                    return splitLine[3];
                }

                line = sr.ReadLine();
            }

            return "none";
        }
        public void getJsonFile(string directoryPath)
        {
            Console.WriteLine("Inside getJsonFile Function");
            try
            {   
                searchJsonFile(directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                throw;
            }
        }

        public void searchJsonFile(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace("\\", "/");
                Console.WriteLine(files[i]);
            }

            foreach (string file in files)
            {
                String[] splitFilePath = file.Split("/");
                string fileName = splitFilePath[splitFilePath.Length - 1];

                if (fileName.ToLower() == "package.json")
                {
                    if (packageJsonPath == "")
                    {
                        packageJsonPath = file;
                    }
                }
            }

            string[] dirs = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                searchJsonFile(dir);
            }
        }

        // <summary>
        // This function is used to clone the package from the link
        // </summary>
        public void TaskIssueClonePackage()
        {
            Console.WriteLine("Inside TaskIssueClonePackage Function");
            Task.Run(() => ClonePackage());
        }

        // <summary>
        // This function is used to clone the package from the link
        // </summary>
        public async Task<int> ClonePackage()
        {
            try
            {
                Console.WriteLine("trying to delete temp folder");
                if(Directory.Exists("/app/TempDirectory"))
                {
                    Directory.Delete("/app/TempDirectory", true);
                }

                Console.WriteLine("deleted temp folder");
                if (baseURL.Contains("https://github.com"))
                {
                    Console.WriteLine("Cloning for github link");
                    var co = new CloneOptions();
                    co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "KingRex212", Password = "3tH')>bGp]}D_S" };
                    Repository.Clone(baseURL, "/app/TempDirectory", co);
                    Console.WriteLine("Cloned for github link");
                    SuccessClone = true;
                    return 1;
                }
                else if (baseURL.Contains("https://www.npmjs.com"))
                {
                    Console.WriteLine("Cloning for npm link");
                    bool task = await Task.Run(async() => await GetGitHubFromNPM());
                    Console.WriteLine("345");
                    bool result = task;

                    SuccessClone = result;
                    var co = new CloneOptions();
                    co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = "KingRex212", Password = "3tH')>bGp]}D_S" };
                    //delete repo if it exists
                    if (Directory.Exists("/app/TempDirectory"))
                    {
                        Directory.Delete("/app/TempDirectory", true);
                    }
                    Console.WriteLine("githubURL URL: " + githubURL);
                    Repository.Clone(githubURL, "/app/TempDirectory", co);
                    Console.WriteLine("result: " + result);
                    return 1;

                }
                
                Console.WriteLine("Nothing was hit");
                SuccessClone = false;
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                SuccessClone = false;
                return 0;
            }
        }

        async Task<bool> GetGitHubFromNPM()
        {
            Console.WriteLine("Inside GetGitHubFromNPM Function");
            var apiUrl = $"https://registry.npmjs.org/{name}";
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var packageInfo = JsonConvert.DeserializeObject<PackageInfo>(responseBody);

                    var repositoryUrl = packageInfo.Repository.Url.ToString().Replace(".git", "").Replace("git+", "").Replace("git://", "https://").Replace("git+ssh://", "https://").Replace("ssh://", "https://").Replace("git+http://", "https://").Replace("git+https://", "https://");;
                    Console.WriteLine("379: The github Link is " + repositoryUrl);
                    githubURL = repositoryUrl;
                    return true;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error retrieving repository URL for npm link: {ex.Message}");
                    return false;
                }
            }
        }

        #endregion


        #region API calls
        public async Task<APIError> PullNpmInfo()
        {
            Console.WriteLine("Inside PullNpmInfo Function");
            APIError error = new APIError();

            HttpClient client = new HttpClient();
            string currentDirectory = Directory.GetCurrentDirectory();

            if (type != "npm" && type != "both")
            {
                error.SetError("Invalid URL for npm pull or other issue with type " + baseURL, APIError.errorType.invalidtype);
                return error;
            }

            if (isInvalid || name == "none" || name == null || name == "")
            {
                error.SetError("Empty npm url or invalid package." + baseURL, APIError.errorType.invalidtype);
                return error;
            }

            //call npm api
            string npmURLRegistry = "https://registry.npmjs.org/" + name;
            Console.WriteLine("Calling npm api with url: " + npmURLRegistry);
            HttpResponseMessage response = await client.GetAsync(npmURLRegistry);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("396");
                error.SetError("Response from registry.npmjs.org: " + response.StatusCode, APIError.errorType.badresponse);
                //Console.WriteLine("Response from registry.npmjs.org: " + response.StatusCode);
                return error;
            }
            Console.WriteLine("401");
            //decode json
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody == null || responseBody == "")
            {
                Console.WriteLine("406");
                error.SetError("Response from registry.npmjs.org: " + response.StatusCode, APIError.errorType.badresponse);
                Console.WriteLine("Response from registry.npmjs.org: " + response.StatusCode);
                return error;
            }
            Console.WriteLine("411");
            //null forgiving operator
            //Console.WriteLine((JsonConvert.DeserializeObject(responseBody)).ToString().Length);
            //File.WriteAllText(currentDirectory + "/npmresponse.json", (JsonConvert.DeserializeObject(responseBody)).ToString());
            dynamic jsoncontent = JsonConvert.DeserializeObject(responseBody)!;
            if (jsoncontent._id == null)
            {
                error.SetError("Response from registry.npmjs.org: " + response.StatusCode, APIError.errorType.badresponse);
                Console.WriteLine(error.ToString());
                return error;
            }
            Console.WriteLine("422");
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
            githubURL = repository;
            Console.WriteLine("450 + " + githubURL);
            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            //mark as npm complete
            SuccessGrabNpm = true;

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
                            isForkname
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

                            number
                            commits(first: 100) {{
                                nodes {{
                                    commit {{
                                        number
                                    }}
                                }}
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
            //check response
            {
                error.SetError("Response from github api: " + graphQLResponse.Errors[0].Message, APIError.errorType.badresponse);
                logger.Log(error.ToString(), 2);
                Console.WriteLine("Error Graphql response");
                return error;
            }
            Console.WriteLine("Response from github api: " + resp);

            Console.WriteLine("Name of repo is: " + resp.repository.name);
            Console.WriteLine("Description of repo is: " + resp.repository.description);
            Console.WriteLine("Created at: " + resp.repository.createdAt);
            Console.WriteLine("Updated at: " + resp.repository.updatedAt);
            Console.WriteLine("Disk usage: " + resp.repository.diskUsage);
            Console.WriteLine("URL: " + resp.repository.url);
            Console.WriteLine("Homepage URL: " + resp.repository.homepageUrl);
            Console.WriteLine("Flags: " + resp.repository.isArchived + " " + resp.repository.isDisabled + " " + resp.repository.isFork + " " + resp.repository.isLocked + " " + resp.repository.isPrivate + " " + resp.repository.isEmpty);
            //Console.WriteLine("License: " + resp.repository.licenseInfo.name + " " + resp.repository.licenseInfo.spdxId);
            if(resp.repository.releases.nodes.Count > 0)
            Console.WriteLine("Release: " + resp.repository.releases.nodes[0].name + " " + resp.repository.releases.nodes[0].description + " " + resp.repository.releases.nodes[0].url + " " + resp.repository.releases.nodes[0].publishedAt);
            Console.WriteLine("Watchers: " + resp.repository.watchers.totalCount);
            Console.WriteLine("Issues: " + resp.repository.issues.totalCount);
            Console.WriteLine("Open issues: " + resp.repository.openIssues.totalCount);
            Console.WriteLine("Forks: " + resp.repository.forks.totalCount);
            Console.WriteLine("Pull requests: " + resp.repository.pullRequests.totalCount);
            if(resp.repository.pullRequests.nodes.Count > 0)
            Console.WriteLine("Pull request comments: " + resp.repository.pullRequests.nodes[0].comments.totalCount);
            Console.WriteLine("Pull request committed line count: " + resp.repository.pullRequests.nodes[1].commits.number);

            Console.WriteLine("Open pull requests: " + resp.repository.openPullRequests.totalCount);
            Console.WriteLine("Discussions: " + resp.repository.discussions.totalCount);
            Console.WriteLine("Stargazers: " + resp.repository.stargazers.totalCount);
            

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
            //githubLicense = resp.repository.licenseInfo.name;
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
            


            Console.WriteLine("Finished loading github data");

            //mark as loaded
            SuccessGrabGithub = true;


            return error;
        }

        #endregion

        #region CalcMethods

        public void CalcValidLicense()
        {
            //priority is local license, then github license
            //assuming they are valid
            if(license == null || license == "")
            {
                if(githubLicense != null && githubLicense != "")
                {
                    license = githubLicense;
                }
            }
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

        public void setBusFactor(float busFactor)
        {
            this.busFactor_score = busFactor;
        }

        public void setRampUpTime(float rampUpTime)
        {
            this.rampUp_score = rampUpTime;
        }
        
        public void setDependencyScore(float dependency)
        {
            this.dependency_score = dependency;
        }

        public void setCorrectnessScore(float correctnessScore)
        {
            this.correctness_score = correctnessScore;
        }

        public void setResponseMaintainerScore(float responseMaintainerScore)
        {
            this.responseMaintainer_score = responseMaintainerScore;
        }

        public void setLicenseScore(int licenseScore)
        {
            this.license_score = licenseScore;
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
    
        public string getScoreInfo()
        {
            string response = "{license: " + license + " license_score: " + license_score + ", rampUp_score: " + rampUp_score + ", busFactor_score: " + busFactor_score + ", correctness_score: " + correctness_score + ", responseMaintainer_score: " + responseMaintainer_score + ", dependancy_score: " + dependency_score + ", pullreview_score: " + pullreview_score + " net_score: " + net_score + "}";
            return response;
        }
        
        public bool getGHSuccess()
        {
            return SuccessGrabGithub;
        }

        public bool getNPMSuccess()
        {
            return SuccessGrabNpm;
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
        public string name { get; set; } = "";
        public QLMergedPullRequestNodeCommentsCount comments { get; set; } = new QLMergedPullRequestNodeCommentsCount();
        public QLMergedPullRequestNodeAdditions commits { get; set; } = new QLMergedPullRequestNodeAdditions();
    }

    public class QLMergedPullRequestNodeCommentsCount
    {
        public int totalCount { get; set; } = 0;
    }
    public class QLMergedPullRequestNodeAdditions
    {
        public int number { get; set; } = 0;
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

    class PackageInfo
    {
        public RepositoryInfo Repository { get; set; }
    }

    class RepositoryInfo
    {
        public string Url { get; set; }
    }

    #endregion
    public class StaticAnalysisLibrary
    {
        DirectoryTool DirectoryTool = new DirectoryTool();

        public void Analyze(URLInfo urlInfo)
        {
            string repoPath = urlInfo.getPath();
            DirectoryTool.getFiles(repoPath);
            urlInfo.licensePath = DirectoryTool.licensePath;
            urlInfo.readmePath = DirectoryTool.readmePath;
            urlInfo.packageJsonPath = DirectoryTool.packageJsonPath;

            foreach(string file in DirectoryTool.sourceCodeEntries)
            {
                ReadFile(file, urlInfo);
            }

            foreach(string file in DirectoryTool.mdEntries)
            {
                urlInfo.commentCharCount += File.ReadAllLines(file).Sum(s => s.Length);
            }

            DirectoryTool.Clear();
        }

        //Reads the file and does the static analysis on the file
        public void ReadFile(string filename, URLInfo urlInfo)
        {
            String? line;
            
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(filename);
                
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    AnalyzeLine(line, urlInfo);
                    line = sr.ReadLine();
                }
                
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                throw;
            }
        }

        public void AnalyzeLine(string text, URLInfo urlInfo)
        {
            if (text.StartsWith("//") || text.StartsWith("/*"))
            {
                urlInfo.commentLineCount++;
            }
            else if (text.Contains("//") || text.Contains("/*"))
            {
                urlInfo.commentLineCount++;
                urlInfo.codeLineCount++;
                
                //finds length of code in lines with code and comments
                String[] separator = { "//,", "/*" };
                String[] textArr = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                urlInfo.codeCharCount += textArr[0].Length;
            }
            else
            {
                urlInfo.codeLineCount++;
                urlInfo.codeCharCount += text.Length;
            }
        }
    }

    public class DirectoryTool
    {
        public List<string> sourceCodeEntries = new List<string>(); //List of all source code files
        public List<string> mdEntries = new List<string>(); //List of all .md files 
        public string licensePath = "none";
        public string readmePath = "none";
        public string packageJsonPath = "none"; 

        public string[] jsFileExt = {".css", ".sass", ".scss", ".less", ".styl", ".html", ".htmls", ".htm", ".js", ".jsx", ".ts", ".tsx", ".cjs", ".mjs", ".iced", ".liticed", ".ls", ".es", ".es6", ".sjs", ".php", ".jsp", ".asp", ".aspx"};
        
        public void getFiles(string directoryPath)
        {
            try
            {   
                getAllFiles(directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                throw;
            }
        }

        public void Clear()
        {
            sourceCodeEntries.Clear();
            mdEntries.Clear();
            licensePath = "none";
            readmePath = "none";
            packageJsonPath = "none";
        }

        //recursive call for getFiles
        public void getAllFiles(string directoryPath)
        {
            //Gets important files in directory path
            string[] filePaths = Directory.GetFiles(directoryPath); 
            foreach (string filePath in filePaths)
            {
                String[] splitFilePath = filePath.Split("/");
                string fileName = splitFilePath[splitFilePath.Length - 1];

                if (jsFileExt.Any(fileName.EndsWith))
                {
                    sourceCodeEntries.Add(filePath); 
                }
                else if (fileName.ToLower().Contains("license"))
                {
                    licensePath = filePath;
                }
                else if (fileName.ToLower().Contains("readme"))
                {
                    readmePath = filePath;
                    mdEntries.Add(filePath);
                }
                else if (fileName.EndsWith(".md"))
                {
                    mdEntries.Add(filePath);
                }
                else if (fileName.ToLower() == "package.json")
                {
                    
                    if (packageJsonPath == "none")
                    {
                        packageJsonPath = filePath;
                    }
                }
            }

            string[] dirs = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                getAllFiles(dir);
            }
        }
    }
}