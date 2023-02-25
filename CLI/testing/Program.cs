using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubGraphQLDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var repositoryUrl = "https://github.com/Rexwang8/ECE461SoftwareEngineeringProject";
            var repositoryParts = repositoryUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            /*
            if (repositoryParts.Length < 2 || repositoryParts[0] != "https:" || repositoryParts[1] != "" || repositoryParts[2] != "github.com")
            {
                Console.WriteLine("Invalid repository URL.");
                Console.WriteLine(repositoryParts[3]);
                Console.WriteLine(repositoryParts[4]);
                return;
            }
            */

            //var owner = repositoryParts[3];
            //var name = repositoryParts[4];

            var owner = "ECE461SoftwareEngineeringProject";
            var name = "Rexwang8";

            // Create an HttpClient with an access token for the GitHub GraphQL API
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TOKEN HERE");

            // Create a GraphQL query
            var query = new
            {
                query = @"
                    query($owner: String!, $name: String!) {
                        repository(owner: $owner, name: $name) {
                            name
                            description
                            stargazerCount
                        }
                    }",
                variables = new
                {
                    owner,
                    name
                }
            };

            // Serialize the query to JSON
            var queryJson = JsonConvert.SerializeObject(query);

            // Create a StringContent object with the serialized query
            var content = new StringContent(queryJson, Encoding.UTF8, "application/json");

            // Make a POST request to the GitHub GraphQL API
            var response = await client.PostAsync("https://api.github.com/graphql", content);

            if (response.IsSuccessStatusCode)
            {
                // Decode the response JSON
                var responseBody = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<JObject>(responseBody);

                // Extract the repository data from the response
                var repository = data["data"]["repository"];

                // Write the repository data to a JSON file
                var gitPath = Path.Combine(Directory.GetCurrentDirectory(), $"{owner}_{name}.json");
                if (!File.Exists(gitPath))
                {
                    File.Create(gitPath);
                }
                File.WriteAllText(@gitPath, repository.ToString());
                
                Console.WriteLine($"Data for {repository["name"]} written to {gitPath}");
            }
            else
            {
                Console.WriteLine($"Request failed with status code {response.StatusCode}");
            }

            // Dispose once all HttpClient calls are complete.
            client.Dispose();
        }
    }
}