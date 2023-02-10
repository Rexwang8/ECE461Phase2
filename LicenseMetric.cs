using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string repoUrl = "https://github.com/nodejs/node-addon-api";
        
        
        string apiUrl = "https://api.github.com/repos/OWNER/REPO/contents/LICENSE.md";
        apiUrl = apiUrl.Replace("OWNER", repoUrl.Split('/')[3]).Replace("REPO", repoUrl.Split('/')[4]);

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            var response = httpClient.GetAsync(apiUrl).Result;
            if (response.IsSuccessStatusCode)
            {   
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(content);
                var encodedContent = json.content.ToString();
                var cleanedEncodedContent = encodedContent.TrimEnd('\r', '\n');
                var decodedBytes = Convert.FromBase64String(cleanedEncodedContent);
                string x = Encoding.UTF8.GetString(decodedBytes);
                Console.WriteLine(x);
                string l1 = "MIT";
                string l2 = "X11";
                string l3 = "BSD-3-Clause";
                string l4 = "BSD-2-Clause";
                string l5 = "LGPL-2.1";
                if (x.Contains(l1)) { Console.WriteLine("True"); }
                if (x.Contains(l2)) { Console.WriteLine("True"); }
                if (x.Contains(l3)) { Console.WriteLine("True"); }
                if (x.Contains(l4)) { Console.WriteLine("True"); }
                if (x.Contains(l5)) { Console.WriteLine("True"); }

            }
            else
            {
                Console.WriteLine("Failed to retrieve license information.");
            }
        }
    }
}



