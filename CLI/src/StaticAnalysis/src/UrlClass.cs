namespace StaticAnalysis
{
    public class URLClass
    {
        IDictionary<string, URLInfo> urlDic = new Dictionary<string, URLInfo>(); //di

        //This functions adds url to the class with everything filled out
        public void addURL(string url)
        {
            URLInfo urlInfo = new URLInfo();

            urlInfo.addURL(url);

            //urlDic.Add(getName(url), null);
        }

        /* public void downloadURL()
        {

        } */
    } 
}