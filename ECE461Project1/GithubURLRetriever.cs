using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECE461Project1
{
    /// <summary>
    /// Handles loading & converting github urls from a file containing github urls and npmjs urls
    /// </summary>
    public static class GithubURLRetriever
    {
        /// <summary>
        /// Returns a list of Github URLs when given an array containing github and npmjs urls
        /// </summary>
        /// <param name="rawUrls">array of raw urls</param>
        /// <returns>A list of Github URLs</returns>
        public static List<string> GetURLList(string[] rawUrls)
        {
            Task<List<string>> asyncCall = GetGithubURLListAsync(rawUrls);
            asyncCall.Wait();

            return asyncCall.Result;
        }


        /// <summary>
        /// Returns an array of raw URLs when given a text file containing github and npmjs urls
        /// </summary>
        /// <param name="filePath">path to text file</param>
        /// <returns>A list of Github and npmjs urls</returns>
        public static string[] GetRawListFromFile(string filePath)
        {
            //Load File
            return File.ReadAllLines(@filePath);
        }

        //Returns a list of Github URLs
        static async Task<List<string>> GetGithubURLListAsync(string[] urlArray)
        {
            List<string> githubURLs = new List<string>();
            List<string> npmjsURLs = new List<string>();

            foreach (string url in urlArray)
            {
                if (url.Contains("github.com"))
                {
                    githubURLs.Add(url);
                }
                else if (url.Contains("npmjs.com"))
                {
                    npmjsURLs.Add(url);
                }
                else Console.WriteLine("Error, invalid URL");
            }

            githubURLs.AddRange(await ConvertNpmjsToGithubUrlAsync(npmjsURLs));

            return githubURLs;
        }

        //Returns a List of Github URLs
        static async Task<List<string>> ConvertNpmjsToGithubUrlAsync(List<string> npmjsURLs)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            List<string> githubUrlList = new List<string>();
            foreach (string npmjsURL in npmjsURLs)
            {
                string webPage = await client.GetStringAsync(npmjsURL);

                //remove all text before the github url
                string[] webPageParsed = webPage.Split("\"repository-link\">", StringSplitOptions.RemoveEmptyEntries);

                //remove all text after the github url
                webPageParsed = webPageParsed[1].Split('<', StringSplitOptions.RemoveEmptyEntries);

                if (webPageParsed[0].Contains("github.com"))
                {
                    githubUrlList.Add("https://" + webPageParsed[0]);
                }
                else Console.WriteLine("Error, github url not found on: " + npmjsURL);
            }

            return githubUrlList;
        }
    }
}
