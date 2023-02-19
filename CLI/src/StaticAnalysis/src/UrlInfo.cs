namespace StaticAnalysis
{
    class URLInfo
    {
        bool isInvalid = false;

        string name { get; set; }
        string url { get; set; } //todo: change it to a list because npm has npm and github url
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
            url = url;
            Console.WriteLine("name: " + name);
            Console.WriteLine("type: " + type);
        }

        public string getType(string url)
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

        public string getName(string url)
        {
            String[] splitUrl = url.Split("/");
            return splitUrl[splitUrl.Length - 1];
        }

    
    }


}