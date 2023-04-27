/*using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PackagePopularity
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const double EMA_ALPHA = 0.1; // Adjust this value to control the weight of recent activity

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter package name:");
            string packageName = Console.ReadLine();

            PackageData packageData = await FetchPackageData("https://your-custom-api.com/package", packageName);

            double popularityScore = CalculatePopularityScore(packageData);

            Console.WriteLine($"Popularity Score: {popularityScore}");
        }

        private static async Task<PackageData> FetchPackageData(string apiUrl, string packageName)
        {
            apiUrl = $"{apiUrl}?name={packageName}";
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
                var packageData = JsonConvert.DeserializeObject<PackageData>(jsonString);
                return packageData;
            }

            return null;
        }

        private static double CalculatePopularityScore(PackageData packageData)
        {
            if (packageData == null)
            {
                return 0.0;
            }

            double emaStars = CalculateEMA(packageData.Stars, EMA_ALPHA);
            double emaDownloads = CalculateEMA(packageData.Downloads, EMA_ALPHA);

            // You can adjust the weights for stars and downloads based on your preference
            double popularityScore = (0.5 * emaStars) + (0.5 * emaDownloads);

            return popularityScore;
        }

        private static double CalculateEMA(List<int> data, double alpha)
        {
            double ema = data[0];

            for (int i = 1; i < data.Count; i++)
            {
                ema = (alpha * data[i]) + ((1 - alpha) * ema);
            }

            return ema;
        }
    }

    public class PackageData
    {
        [JsonProperty("stars")]
        public List<int> Stars { get; set; }

        [JsonProperty("downloads")]
        public List<int> Downloads { get; set; }
    }
}*/