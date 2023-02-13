using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Reactive.Joins;
//using System.Text.Json.Serialization;

namespace ECE461Project1
{
    public class License : IScoreMetric
    {
        public float metricWeight { get; } = 0.40f;
        public string metricName { get; } = "LICENSE";

        public bool unsuccesfullHTTPRequestFlag = false;

        public float GetScore(string githubUrl)
        {
            string api_url = "https://api.github.com/repos/OWNER/REPO/contents/README.md";
            api_url = api_url.Replace("OWNER", githubUrl.Split('/')[3]).Replace("REPO", githubUrl.Split('/')[4]);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
                var response = httpClient.GetAsync(api_url).Result;
                if (response.IsSuccessStatusCode)
                {
                    unsuccesfullHTTPRequestFlag = false;
                    var content = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(content);
                    var encodedContent = json.content.ToString();
                    var cleanedEncodedContent = encodedContent.TrimEnd('\r', '\n');
                    var decodedBytes = Convert.FromBase64String(cleanedEncodedContent);
                    string x = Encoding.UTF8.GetString(decodedBytes);
                    Logger.WriteLine("\n" + x + "\n", 2);
                    string pattern = @"(?:2-[Cc]lause\sBSD|BSD\s2-[Cc]lause)|(?:3-[Cc]lause\sBSD|BSD\s3-[Cc]lause)|ISC|MIT|LGPL[-\s]2\.1|GNU LESSER GENERAL PUBLIC LICENSE|X11";
                    Regex regex = new Regex(pattern);
                    if (regex.IsMatch(x) == true) { return 1; } else { return 0; }
                }
                else
                {
                    unsuccesfullHTTPRequestFlag = true;
                    Logger.WriteLine(githubUrl, 1);
                    Logger.WriteLine(api_url, 1);
                    Logger.WriteLine("\nUnsuccesful attempt, Response code: " + response.StatusCode, 1);
                    Logger.WriteLine(response.ReasonPhrase, 1);
                    return 0;
                }
            }
        }
    }
}
