using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Reactive.Joins;
using Utility;
namespace StaticAnalysis;
public static class License
{               
    public static int GetScore(URLInfo urlInfo, string licenseListPath)
    {
        /*if (urlInfo.licensePath == "" || urlInfo.licensePath == null)
        {
            urlInfo.license = "Not Available";
            return 0;
        }*/
        if (urlInfo.license == "" || urlInfo.license == null) {
            urlInfo.license = "Not Available";
            return 0;
        }
        //string License = File.ReadLines(urlInfo.licensePath).First(); // gets the first line from file.
        string License = urlInfo.license;
        var allLicenses = File.ReadAllLines(licenseListPath);
        var allLicensesArr = new List<string>(allLicenses);
        List<string> allLicensesArrCleaned = new List<string>();
        foreach (string li in allLicensesArr)
        {
            string newli = li.Trim().ToLower();
            allLicensesArrCleaned.Add(newli);
        }



        foreach (var LicenseVar in allLicensesArrCleaned)
        {
            string cleanedlicense = License.Trim().ToLower();
            string CleanedLicenseVar = LicenseVar.Trim().ToLower();
            if(cleanedlicense.Contains(CleanedLicenseVar))
            {
                urlInfo.license = CleanedLicenseVar;
                return 1;
            }
        }
        urlInfo.license = "Not Available";
        return 0;
    }
}