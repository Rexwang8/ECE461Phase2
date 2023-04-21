using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PackagePopularityTracker
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter GitHub Repository (format: owner/repo):");
            string? githubRepo = Console.ReadLine();

            Console.WriteLine("Enter NPM Package Name:");
            string? npmPackageName = Console.ReadLine();

            if (githubRepo != null && npmPackageName != null)
            {
                int? stars = await GetGitHubStars(githubRepo);
                int? downloads = await GetNpmDownloads(npmPackageName);

                if (stars.HasValue && downloads.HasValue)
                {
                    Console.WriteLine($"GitHub Stars: {stars}");
                    Console.WriteLine($"NPM Downloads: {downloads}");
                }
                else
                {
                    Console.WriteLine("Error fetching data. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        private static async Task<int?> GetGitHubStars(string repo)
        {
            string apiUrl = $"https://api.github.com/repos/{repo}";
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<GitHubRepoResponse>(jsonString);
                return json?.StargazersCount;
            }

            return null;
        }

        private static async Task<int?> GetNpmDownloads(string packageName)
        {
            string apiUrl = $"https://api.npmjs.org/downloads/point/last-month/{packageName}";
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<NpmDownloadsResponse>(jsonString);
                return json?.Downloads;
            }

            return null;
        }
    }

    public class GitHubRepoResponse
    {
        [JsonProperty("stargazers_count")]
        public int StargazersCount { get; set; }
    }

    public class NpmDownloadsResponse
    {
        [JsonProperty("downloads")]
        public int Downloads { get; set; }
    }
}
