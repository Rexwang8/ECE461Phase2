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

namespace IO.Swagger.CLI
{
    public static class APICalls
    {
        public static void GetURLStatistics(URLInfo urlInfo)
        {
            List<URLInfo> urlInfos = new List<URLInfo>();
            urlInfos.Add(urlInfo);
            URLClass AllPackages = new URLClass(urlInfos);

            //Call NPM 
            for (int i = 0; i < 3; i++)
            {
                if (AllPackages.getTotalNpmPulled() == AllPackages.getTotalNpm())
                {
                    break;
                }

                foreach (var pkg in AllPackages.GetAllPackages().Values)
                {
                    Console.WriteLine("Getting npm info for " + pkg.getName());
                    if ((pkg.getType() == "npm" || pkg.getType() == "both") && pkg.getNPMSuccess() == false)
                    {
                        callNPM(pkg);
                        
                        //add built in delay to avoid rate limiting
                        System.Threading.Thread.Sleep(500);
                    }
                }
                //update total npm pulled
                AllPackages.countNpmPulled();
                if (AllPackages.getTotalNpmPulled() == AllPackages.getTotalNpm())
                {
                    Console.WriteLine("All npm packages pulled successfully( " + AllPackages.getTotalNpmPulled() + " out of " + AllPackages.getTotalNpm() + " )");
                    break;
                }
                
                Console.WriteLine("Retrying npm pulls(retry " + (i + 1) + " of 3)");
                System.Threading.Thread.Sleep(2000);
            }
        }

        public static async void callNPM(URLInfo urlInfo)
        {
            Console.WriteLine("Inside callNPM Function");
            //prevent double calls
            if (urlInfo.getNPMSuccess())
                return;

            //Execute the task and handle any errors
            Task<APIError> task = Task.Run(() => urlInfo.PullNpmInfo());
            APIError err = await task;
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
    }
}