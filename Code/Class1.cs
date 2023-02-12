using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GithubApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The repository's URL
            string repositoryUrl = "https://api.github.com/repos/nodejs/node-addon-api";

            // Create an HttpClient instance
            using (var client = new HttpClient())
            {
                // Make a GET request to the repository URL
                var response = await client.GetAsync(repositoryUrl);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var content = await response.Content.ReadAsStringAsync();

                    // Deserialize the response content into a dynamic object
                    dynamic repo = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    // Get the license type
                    string licenseType = repo.license?.name;

                    Console.WriteLine("The license type of the repository is: " + licenseType);
                }
                else
                {
                    Console.WriteLine("The request was not successful.");
                }
            }
        }
    }
}
