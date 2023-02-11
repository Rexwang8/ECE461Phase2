using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

using GraphQL;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Threading.Tasks;

namespace ECE461Project1
{
    class BusFactor : IScoreMetric
    {
        public float metricWeight { get; } = 0.35f;

        public float GetScore(string githubUrl)
        {
            // Get token
            var token = System.Environment.GetEnvironmentVariable("$GITHUB_TOKEN");

            // Setup score variable
            float finalScore = 0;

            // Get the repository name and owner from the URL
            string githubUrl = githubUrl;
            string[] parts = githubUrl.Split('/');
            string repoOwner = parts[parts.Length - 2];
            string repoName = parts[parts.Length - 1];

            // GitHub REST API - https://docs.github.com/en/rest/metrics/community?apiVersion=2022-11-28
            // Checks existence of certain non-code documents (like the Readme)
            string commURL = "https://api.github.com/repos/";
            commURL += repoOwner + "/" + repoName;
            commURL += "/community/profile";

            using (var client = new HttpClient())
            {
                // Headers and API call
                // Doesn't seem to need a github token
                client.DefaultRequestHeaders.Add("User-Agent", "BusFactor App");
                var response = client.GetAsync(commURL).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Parse response
                    var json = response.Content.ReadAsStringAsync().Result;
                    var health = JsonConvert.DeserializeObject<HealthResponse>(json);

                    // This value counts for 30% of the returned BusFactor score
                    float restScore = 0.3f * health.health_percentage / 100;
                    finalScore += restScore;
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
                Query = @"query {
                    repository(name:""esp-idf"", owner:""espressif"") { 
                        ref(qualifiedName: ""master"") { 
                            target { 
                                ... on Commit { 
                                    history(first: 100) { 
                                        edges { 
                                            node { 
                                                additions 
                                                deletions
                                                author { 
                                                    name
                                                } 
                                            } 
                                        } 
                                    } 
                                } 
                            } 
                        } 
                    }
                }"
            };

			// Add auth token
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

			// Get response
            var graphQLResponse = await graphQLClient.SendQueryAsync<QLData>(graphQLRequest);

			// All those classes at the bottom are used here
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
			// Meaning RSE = 0.5 means a score of 0; RSE = 1.0 a score of 1
            double average = linesArray.Average();
            double sumSquDiff = linesArray.Select(val => (val - average) * (val - average)).Sum();
            double stDev = Math.Sqrt(sumSquDiff / (linesArray.Length - 1));
            double RSE = stDev / (Math.Sqrt(linesArray.Length) * average);

			// This value counts as 70% of the returned BusFactor score
            double graphScore = 0.7 * (1 - 2 * RSE);
            finalScore += (float) graphScore;

            return finalScore;
        }
    }


    class HealthResponse
    {
        public int health_percentage;
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