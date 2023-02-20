using Utility;

namespace StaticAnalysis
{
    class URLInfo
    {
        bool isInvalid = false;

        string name { get; set; }
        string githubUrl { get; set; }
        string npmUrl { get; set; }
        string type { get; set; }
        string path { get; set; }
        
        int license_score { get; set; }
        float rampUp_score { get; set; }
        float busFactor_score { get; set; }
        float correctness_score { get; set; }
        float responseMaintainer_score { get; set; }
        float net_score { get; set; }
        
        public void addURL(string url)
        {
            name = getName(url);
            type = getType(url);
            if (type == "npm")
            {
                githubUrl = "none";
                npmUrl = url;
            }
            else if (type == "github")
            {
                githubUrl = url;
                npmUrl = "none";
            }
            else
            {
                githubUrl = "none";
                npmUrl = "none";
            }
        }

        //Need function for download github and npm

        public string getInfo()
        {
            return "{name: " +  name + ", github url: " + githubUrl + ", npm url: " + npmUrl + ", type: " + type + ", path: " + path + "}"; 
        }

        string getName(string url)
        {
            String[] splitUrl = url.Split("/");
            return splitUrl[splitUrl.Length - 1];
        }

        string getType(string url)
        {
            if (url.Contains("github.com"))
            {
                return "github";
            }
            else if (url.Contains("npmjs.com"))
            {
                return "npm";
            }
            else
            {
                isInvalid = true;
                return "invalid";
            }
        }
    }
}