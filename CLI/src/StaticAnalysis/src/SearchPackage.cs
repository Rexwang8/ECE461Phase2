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
            Console.WriteLine("Enter regular expression:");
            string regexPattern = Console.ReadLine();

            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            List<Package> packages = await FetchPackages("url");

            Console.WriteLine("Filtered Packages:");
            foreach (var package in packages)
            {
                if (regex.IsMatch(package.Name) || regex.IsMatch(package.Readme))
                {
                    Console.WriteLine($"Name: {package.Name}, Readme: {package.Readme}");
                }
            }
        }

        private static async Task<List<Package>> FetchPackages(string apiUrl)
        {
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

        [JsonProperty("readme")]
        public string Readme { get; set; }
    }
}