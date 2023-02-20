using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Handles loading & converting github urls from a file containing github urls and npmjs urls
    /// </summary>
    public static class GithubURLRetriever
    {
        public static Dictionary<string, string> githubUrlToRawURL = new Dictionary<string, string>();

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
                    githubUrlToRawURL.Add(url, url);
                }
                else if (url.Contains("npmjs.com"))
                {
                    npmjsURLs.Add(url);
                }
                else Logger.WriteLine("Error, invalid URL", 1);
            }

            githubURLs.AddRange(await ConvertNpmjsToGithubUrlAsync(npmjsURLs));

            return githubURLs;
        }

        //Returns a List of Github URLs
        static async Task<List<string>> ConvertNpmjsToGithubUrlAsync(List<string> npmjsURLs)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "request");

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
                    string githubUrl = "https://" + webPageParsed[0];
                    githubUrlList.Add(githubUrl);
                    if (!githubUrlToRawURL.ContainsKey(githubUrl)) githubUrlToRawURL.Add(githubUrl, npmjsURL);
                    else githubUrlToRawURL[githubUrl] = npmjsURL;
                }
                else Logger.WriteLine("Error, github url not found on: " + npmjsURL, 1);
            }

            return githubUrlList;
        }
    }
}
