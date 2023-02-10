using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ECE461Project1
{
    class BusFactor : IScoreMetric
    {
        public float metricWeight { get; } = 0.35f;

        public float GetScore(string githubUrl)
        {
            Dictionary<string, int> commit_counts = new Dictionary<string, int>(); //Owner, num commits made

            // Get the repository name and owner from the URL
            string[] parts = githubUrl.Split('/');
            string repoOwner = parts[parts.Length - 2];
            string repoName = parts[parts.Length - 1];

            // Use the info to call GitHub REST/GraphQL API...

            // REST API...get 1000 commits (just looking for the author names)
            for (int i = 1; i < 11; i++)
            {
                string getURL = "https://api.github.com/repos/";
                getURL += repoOwner + "/" + repoName;
                getURL += "/commits";
                getURL += "?per_page=100&page=";
                getURL += i.ToString();

                using (var client = new HttpClient())
                {
                    // Headers and API call
                    client.DefaultRequestHeaders.Add("User-Agent", "MyApp");
                    var response = client.GetAsync(getURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse response
                        var json = response.Content.ReadAsStringAsync().Result;
                        var commits = JsonConvert.DeserializeObject<List<ResultCommit>>(json);

                        foreach (ResultCommit comm in commits)
                        {
                            string committer = comm.commit.author.name;
                            if (commit_counts.ContainsKey(committer))
                            {
                                commit_counts[committer]++;
                            }
                            else
                            {
                                commit_counts.Add(committer, 1);
                            }
                        }
                    }
                }
            }

            using (var client = new HttpClient())
            {
                // Headers and API call
                client.DefaultRequestHeaders.Add("User-Agent", "MyApp");
                var response = client.GetAsync("https://api.github.com/repos/espressif/esp-idf/branches/per_page=1").Result;
                if (response.IsSuccessStatusCode)
                {
                    // Parse response
                    var json = response.Content.ReadAsStringAsync().Result;
                    var commits = JsonConvert.DeserializeObject<List<ResultCommit>>(json);

                    foreach (ResultCommit comm in commits)
                    {
                        string committer = comm.commit.author.name;
                        if (commit_counts.ContainsKey(committer))
                        {
                            commit_counts[committer]++;
                        }
                        else
                        {
                            commit_counts.Add(committer, 1);
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }
    }


    class ResultCommit
    {
        public Commit commit;
    }
    class Commit
    {
        public Author author;
    }
    class Author
    {
        public string name;
    }
    class GitFile
    {
        public string filename;
    }
    class Stats
    {
        public int total;
        public int additions;
        public int deletions;
    }
}