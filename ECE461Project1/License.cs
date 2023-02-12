using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
//using System.Text.Json.Serialization;

namespace ECE461Project1
{
    public class License : IScoreMetric
    {
        public float metricWeight { get; } = 0.40f;
        public float GetScore(string githubUrl)
        {
            string api_url = "https://api.github.com/repos/OWNER/REPO/contents/LICENSE.md";
            api_url = api_url.Replace("OWNER", githubUrl.Split('/')[3]).Replace("REPO", githubUrl.Split('/')[4]);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
                var response = httpClient.GetAsync(api_url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(content);
                    var encodedContent = json.content.ToString();
                    var cleanedEncodedContent = encodedContent.TrimEnd('\r', '\n');
                    var decodedBytes = Convert.FromBase64String(cleanedEncodedContent);
                    string x = Encoding.UTF8.GetString(decodedBytes);
                    string l1 = "MIT";
                    string l2 = "X11";
                    string l3 = "BSD-3-Clause";
                    string l4 = "BSD-2-Clause";
                    string l5 = "LGPL-2.1";
                    if (x.Contains(l1)) { return 1; }
                    if (x.Contains(l2)) { return 1; }
                    if (x.Contains(l3)) { return 1; }
                    if (x.Contains(l4)) { return 1; }
                    if (x.Contains(l5)) { return 1; }
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
