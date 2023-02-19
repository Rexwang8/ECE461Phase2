using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Utility;
using GraphQL;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Threading.Tasks;

namespace StaticAnalysis
{
    public class BusFactor : IScoreMetric
    {
        public float metricWeight { get; } = 0.35f;
        public string metricName { get; } = "BUS_FACTOR";

        public bool hitRateLimitFlag = false;

        public float GetScore(string githubUrl)
        {
            try
            {
                Task<float> score = mainCalculation(githubUrl);
                score.Wait();
                hitRateLimitFlag = false;
                return score.Result;
            }
            catch 
            {
                hitRateLimitFlag = true;
                Logger.WriteLine("Bus Factor rate limit reached, all scores will be 0");
                return 0; //in case rate limit is hit
            }
        }

        static async Task<float> mainCalculation(string githubUrl)
        {
            // Get token
            var token = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            // Setup score variable
            float finalScore = 0;

            // Get the repository name and owner from the URL
            string[] parts = githubUrl.Split('/');
            string repoOwner = parts[parts.Length - 2];
            string repoName = parts[parts.Length - 1];
            string defaultBranch = "";

            // GitHub REST API
            // Checks existence of certain non-code documents (like the Readme)
            string getURL = "https://api.github.com/repos/";
            getURL += repoOwner + "/" + repoName;

            string healthURL = getURL + "/community/profile";
            string branchURL = getURL + "/branches?per_page=1";

            // Get health_percentage (https://docs.github.com/en/rest/metrics/community?apiVersion=2022-11-28)
            using (var client = new HttpClient())
            {
                // Headers and API call
                // Doesn't seem to need a github token
                client.DefaultRequestHeaders.Add("User-Agent", "BusFactor App");
                var healthResponse = client.GetAsync(healthURL).Result;
                if (healthResponse.IsSuccessStatusCode)
                {
                    // Parse response
                    var json = healthResponse.Content.ReadAsStringAsync().Result;
                    var health = JsonConvert.DeserializeObject<HealthResponse>(json);

                    // This value counts for 30% of the returned BusFactor score
                    float restScore = 0.3f * health.health_percentage / 100;
                    finalScore += restScore;
                }
            }

            // Get default branch name (usually master or main)
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "BusFactor App");
                var branchResponse = client.GetAsync(branchURL).Result;
                if (branchResponse.IsSuccessStatusCode)
                {
                    // Parse response
                    var json = branchResponse.Content.ReadAsStringAsync().Result;
                    var branch = JsonConvert.DeserializeObject<List<BranchResponse>>(json);
                    defaultBranch = branch[0].name;
                }
            }

            // GitHub GraphQL API

            // Dictionary: <committer, # of lines changed>
            Dictionary<string, int> commit_counts = new Dictionary<string, int>();

            // GraphQL Endpoint
            string uri = "https://api.github.com/graphql";

            // GraphQL request process using GraphQL dotnet
            // Setup client
            var graphQLClient = new GraphQL.Client.Http.GraphQLHttpClient(uri, new NewtonsoftJsonSerializer());

            // Create query request
            var graphQLRequest = new GraphQL.GraphQLRequest
            {
                Query = @$"query {{
                        repository(name:""{repoName}"", owner:""{repoOwner}"") {{ 
                            ref(qualifiedName: ""{defaultBranch}"") {{ 
                                target {{ 
                                    ... on Commit {{ 
                                        history(first: 100) {{ 
                                            edges {{ 
                                                node {{ 
                                                    additions 
                                                    deletions
                                                    author {{ 
                                                        name
                                                    }} 
                                                }} 
                                            }} 
                                        }} 
                                    }} 
                                }} 
                            }} 
                        }}
                    }}"
            };

            // Add auth token
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Get response
            var graphQLResponse = await graphQLClient.SendQueryAsync<QLData>(graphQLRequest);

            // All those classes at the bottom are used here, they are based entirely on the query
            // Populate the commit_counts dictionary
            foreach (QLEdges edge in graphQLResponse.Data.repository.@ref.target.history.edges)
            {
                string commitAuthor = edge.node.author.name;
                int linesChanged = edge.node.additions + edge.node.deletions;

                if (commit_counts.ContainsKey(commitAuthor))
                {
                    commit_counts[commitAuthor] += linesChanged;
                }
                else
                {
                    commit_counts.Add(commitAuthor, linesChanged);
                }
            }

            // Move commit_counts dictionary values into a list, then to an array
            List<int> linesList = new List<int>();
            foreach (int lineCount in commit_counts.Values)
            {
                linesList.Add(lineCount);
            }
            int[] linesArray = linesList.ToArray();

            // Calculate the Relative Standard Error with 0.50 (50%) as the high value
            // Meaning RSE = 0.5 means a score of 0; RSE = 0.0 a score of 1
            double lineSum = 0;
            foreach (int lineCount in linesArray) { lineSum += lineCount; }
            double average = lineSum / linesArray.Length;

            double squSum = 0;
            foreach (int lineCount in linesArray) { squSum += Math.Pow((lineCount - average), 2); }

            double stDev = Math.Sqrt(squSum / (linesArray.Length - 1));
            double RSE = stDev / (Math.Sqrt(linesArray.Length) * average);
            if (linesArray.Length == 1) { RSE = 0.5; }

            // This value counts as 70% of the returned BusFactor score
            double graphScore = 0.7 * (1 - 2 * RSE);
            if (graphScore < 0) { graphScore = 0; }
            finalScore += (float)graphScore;

            return finalScore;
        }
    }


    class HealthResponse
    {
        public int health_percentage;
    }
    class BranchResponse
    {
        public string name;
    }
    class QLData
    {
        public QLRepo repository;
    }
    class QLRepo
    {
        public QLRef @ref;
    }
    class QLRef
    {
        public QLTarget target;
    }
    class QLTarget
    {
        public QLHistory history;
    }
    class QLHistory
    {
        public List<QLEdges> edges;
    }
    class QLEdges
    {
        public QLNode node;
    }
    class QLNode
    {
        public int additions;
        public int deletions;
        public Author author;
    }
    class Author
    {
        public string name;
    }
}