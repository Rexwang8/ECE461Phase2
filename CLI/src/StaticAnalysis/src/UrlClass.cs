namespace StaticAnalysis
{
    public class URLClass
    {
        Dictionary<string, URLInfo> urlDic = new Dictionary<string, URLInfo>(); //dictionary

        int totalPackages = 0;
        int totalNpm = 0;
        int totalNpmPulled = 0;
        int totalGithub = 0;
        int totalGithubPulled = 0;

        public URLClass(List<URLInfo> urlInfos)
        {
           
            foreach (var url in urlInfos)
            {
                url.initType();
                url.initName();

                if(url.getIsInvalid() == true)
                {
                    continue;
                }

                //checks if the url is already in the dictionary
                if(urlDic.ContainsKey(url.getName()))
                {
                    dealWDuplicate(url);
                    continue;
                }
                
                
                //Finally add
                urlDic.Add(url.getName(), url);
            }
        }

        private void dealWDuplicate(URLInfo url)
        {
            //If it us the exact url, dont do anything
            if (url.getURL() == urlDic[url.getName()].getURL())
            {
                return;
            }

            //Different Url
            string type1 = url.getType();
            string type2 = urlDic[url.getName()].getType();

            if (type1 == "github" & type2 == "npm")
            {
                urlDic[url.getName()].setGithubURL(url.getURL());
                urlDic[url.getName()].setTypeName("both");
            }
            else if (type1 == "npm" & type2 == "github")
            {
                urlDic[url.getName()].setNpmURL(url.getURL());
                urlDic[url.getName()].setTypeName("both");
            }
            else
            {
                Console.WriteLine("Error: Duplicate URL type not recognized");
            }
        } 


        public URLInfo getURLInfo(string name)
        {
            return urlDic[name];
        }

        public Dictionary<string, URLInfo> GetAllPackages()
        {
            return urlDic;
        }

        public int getTotalPackages()
        {
            return totalPackages;
        }

        public int getTotalNpm()
        {
            return totalNpm;
        }

        public int getTotalGithub()
        {
            return totalGithub;
        }

        public void printNumPackages()
        {
            Console.WriteLine("Total Packages: " + totalPackages);
            Console.WriteLine("Total NPM: " + totalNpm);
            Console.WriteLine("Total Github: " + totalGithub);
        }

        public void setTotalPackages(int num)
        {
            totalPackages = num;
        }

        public void countNPM()
        {
            foreach (var url in urlDic)
            {
                if (url.Value.getType() == "npm" || url.Value.getType() == "both")
                {
                    totalNpm++;
                }
            }
        }

        public void countGithub()
        {
            foreach (var url in urlDic)
            {
                if (url.Value.getType() == "github" || url.Value.getType() == "both")
                {
                    totalGithub++;
                }
            }
        }

        public void countNpmPulled()
        {
            foreach (var url in urlDic)
            {
                if (url.Value.getNPMSuccess() == true)
                {
                    totalNpmPulled++;
                }
            }
        }

        public void countGithubPulled()
        {
            foreach (var url in urlDic)
            {
                if (url.Value.getGHSuccess() == true)
                {
                    totalGithubPulled++;
                }
            }
        }

        public int getTotalNpmPulled()
        {
            return totalNpmPulled;
        }

        public int getTotalGithubPulled()
        {
            return totalGithubPulled;
        }
    } 
}