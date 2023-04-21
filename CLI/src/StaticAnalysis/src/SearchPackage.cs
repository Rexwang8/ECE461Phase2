using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PackageSearch
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter search query:");
            string query = Console.ReadLine();

            Console.WriteLine("Enter regular expression:");
            string regexPattern = Console.ReadLine();

            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            List<Package> gitHubPackages = await SearchPackages("https://ourapi.com/github", query);
            List<Package> npmPackages = await SearchPackages("https://ourapi.com/npm", query);

            Console.WriteLine("Filtered GitHub Packages:");
            foreach (var package in gitHubPackages)
            {
                if (regex.IsMatch(package.Name) || regex.IsMatch(package.Description))
                {
                    Console.WriteLine($"Name: {package.Name}, Description: {package.Description}");
                }
            }

            Console.WriteLine("Filtered NPM Packages:");
            foreach (var package in npmPackages)
            {
                if (regex.IsMatch(package.Name) || regex.IsMatch(package.Description))
                {
                    Console.WriteLine($"Name: {package.Name}, Description: {package.Description}");
                }
            }
        }

        private static async Task<List<Package>> SearchPackages(string apiUrl, string query)
        {
            apiUrl = $"{apiUrl}?q={query}";
            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return new List<Package>();
            }

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<List<Package>>(jsonString);
                return json;
            }

            return new List<Package>();
        }
    }

    public class Package
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}