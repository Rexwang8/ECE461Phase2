using System;
using System.Collections.Generic;

namespace ECE461ProjectPart1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> githubUrlList = GithubURLRetriever.GetURLList("/Users/ishaan/Desktop/ECE 461/ECE461TeamRepo/URLTestFile.txt");

            foreach (string url in githubUrlList)
            {
                Console.WriteLine(url);
            }
        }
    }
}
