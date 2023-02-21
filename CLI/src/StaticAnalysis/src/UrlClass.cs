namespace StaticAnalysis
{
    public class URLClass
    {
        Dictionary<string, URLInfo> urlDic = new Dictionary<string, URLInfo>(); //dictionary

        public URLClass(List<URLInfo> urlInfos)
        {
           
            foreach (var url in urlInfos)
            {
                url.getName();
                url.getType();
                if(url.returnName() != "none" && url.returnType() != "none" && url.returnIsInvalid() == false)
                {
                    //checks if the url is already in the dictionary
                    if(!urlDic.ContainsKey(url.returnName()))
                    {
                        urlDic.Add(url.returnName(), url);
                    }
                    else
                    {
                       // Console.WriteLine("Duplicate URL: " + url.returnName());
                        //identify duplicate url type and write the new url to the duplicate package if necessary
                        string type = url.returnType();
                        //Console.WriteLine("Type: " + type);
                        if(type == "github")
                        {
                            urlDic[url.returnName()].setGithubURL(url.returnURL());
                            urlDic[url.returnName()].setTypeName("both");
                        }
                        else if(type == "npm")
                        {
                            urlDic[url.returnName()].setNpmURL(url.returnURL());
                            urlDic[url.returnName()].setTypeName("both");
                        }
                        else if(type == "both")
                        {
                          //  Console.WriteLine("Error: Duplicate URL is already both");
                        }
                        else
                        {
                           // Console.WriteLine("Error: Duplicate URL type not recognized");
                        }

                    }
                }
            }
        }

        public URLInfo returnURLInfo(string name)
        {
            return urlDic[name];
        }

        public Dictionary<string, URLInfo> GetAllPackages()
        {
            return urlDic;
        }
    } 
}