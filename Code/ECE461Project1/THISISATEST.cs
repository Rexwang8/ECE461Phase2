using System;
using System.Net.Http;
using Newtonsoft.Json;

//given a npmjs url get a github repo url in c#s

class THISISATEST
{
    public static void GetGithubUrlFromNpmjsUrl(string npmjsUrl)
    {
        Console.WriteLine("Getting GitHub URL from: " + npmjsUrl + "          " + $"https://api.npmjs.org/package/{npmjsUrl.Split('/')[4]}");
        // Use the NPM API to retrieve the package information
        using (var client = new HttpClient())
        {
            var response = client.GetAsync($"https://api.npmjs.org/package/{npmjsUrl.Split('/')[4]}").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<dynamic>(json);
                var repositoryUrl = result.repository.url;
                Console.WriteLine("GitHub Repository URL: " + repositoryUrl);
            }
            else
            {
                Console.WriteLine("Error: " + response.ReasonPhrase);
            }
        }
    }
}