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
    public class Index
    {
        public void GetURLStatistics()
        {
            List<URLInfo> urlInfos = new List<URLInfo>();
            URLInfo test = new URLInfo("example.com");
            urlInfos.Add(test);
            URLClass AllPackages = new URLClass(urlInfos);
        }
    }
}