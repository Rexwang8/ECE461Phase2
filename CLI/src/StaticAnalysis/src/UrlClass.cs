namespace StaticAnalysis
{
    public class URLClass
    {
        IDictionary<string, URLInfo> urlDic = new Dictionary<string, URLInfo>(); //di

        //This functions adds url to the class with everything filled out
        public void addUrls(string ori_url)
        {   
            string[] urls = File.ReadAllLines(ori_url);
            foreach (string url in urls)
            {
                if (urlDic.ContainsKey(getName(url)))
                {
                    continue;
                }

                URLInfo urlInfo = new URLInfo();
                urlInfo.addURL(url);
                urlDic.Add(urlInfo.getName(url), urlInfo);
            }
            
            Console.WriteLine(getUrls());
            //URLInfo urlInfo = new URLInfo();

            //urlInfo.addURL(url);

            //urlDic.Add(getName(url), null);
        }

        /* public void downloadURL()
        {

        } */
        public string getUrls()
        {
            string returnString = string.Empty;
            foreach (KeyValuePair<string, URLInfo> item in urlDic)
            {
                returnString += (item.Value.getInfo() + "\n");               
            } 

            return returnString;
        }

        string getName(string url)
        {
            String[] splitUrl = url.Split("/");
            return splitUrl[splitUrl.Length - 1];
        }
    } 
}