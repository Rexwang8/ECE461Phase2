using Newtonsoft.Json;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL.Client.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System;
using LibGit2Sharp;
using System.Linq;
using IO.Swagger.Controllers;

namespace IO.Swagger.CLI
{
    public static class APICalls
    {
        public static void GetURLStatistics(URLInfo urlInfo)
        {
            List<URLInfo> urlInfos = new List<URLInfo>();
            urlInfos.Add(urlInfo);
            URLClass AllPackages = new URLClass(urlInfos);
            Console.WriteLine("Getting URL statistics for some packages");
            Console.WriteLine("type: " + urlInfo.getType());

            if (urlInfo.getType() == "npm" || urlInfo.getType() == "both")
            {
                for(int i = 0; i < 3; i++)
                {
                    callNPM(urlInfo);
                    if (urlInfo.getNPMSuccess() == true)
                    {
                        Console.WriteLine("NPM info for " + urlInfo.getName() + " pulled successfully");
                        break;
                    }
                    Console.WriteLine("Retrying npm pulls(retry " + (i + 1) + " out of 3)");
                }
            }
           


            //set gh token from bq
            BigQueryFactory factory = new BigQueryFactory();
            var ghtoken = factory.GetGithubTokenStoredInBQ();
            Console.WriteLine("Github token is " + ghtoken);

            if(urlInfo.getType() == "github" || urlInfo.getType() == "both")
            {
                for (int i = 0; i < 3; i++)
                {
                    callGithub(urlInfo, ghtoken);
                    if (urlInfo.getGHSuccess() == true)
                    {
                        Console.WriteLine("Github info for " + urlInfo.getName() + " pulled successfully");
                        break;
                    }
                    Console.WriteLine("Retrying github pulls(retry " + (i + 1) + " out of 3)");
                }
            }

        }

        public static void callNPM(URLInfo urlInfo)
        {
            Console.WriteLine("Inside callNPM Function");
            //prevent double calls
            if (urlInfo.getNPMSuccess())
                return;

            //Execute the task and handle any errors
            Task<APIError> task = Task.Run(() => urlInfo.PullNpmInfo());
            task.Wait();
            APIError err = task.Result;
            if (err.GetErrType() == APIError.errorType.none)
            {
                Console.WriteLine("NPM Data Recieved for package: " + urlInfo.getName());
            }
            else
            {
                Console.WriteLine("NPM Error: " + err.ToString());
            }
            return;
        }

        public static void callGithub(URLInfo urlInfo, string githubToken)
        {
            Console.WriteLine("Inside callGithub Function");
            //prevent double calls
            if (urlInfo.getGHSuccess())
                return;

            //Execute the task and handle any errors
            Task<APIError> task = Task.Run(() => urlInfo.PullGithubInfo(githubToken));
            task.Wait();
            APIError err = task.Result;
            if (err.GetErrType() == APIError.errorType.none)
            {
                Console.WriteLine("Github Data Recieved for package: " + urlInfo.getName());
            }

            else
            {
                Console.WriteLine("Error: " + err.ToString());
            }
            return;
        }



    }
}