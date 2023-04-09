namespace Utility;
public class DirectoryTool
{
    public List<string> sourceCodeEntries = new List<string>(); //List of all source code files
    public List<string> mdEntries = new List<string>(); //List of all .md files 
    public string licensePath = "none";
    public string readmePath = "none";
    public string packageJsonPath = "none"; 

    public string[] jsFileExt = {".css", ".sass", ".scss", ".less", ".styl", ".html", ".htmls", ".htm", ".js", ".jsx", ".ts", ".tsx", ".cjs", ".mjs", ".iced", ".liticed", ".ls", ".es", ".es6", ".sjs", ".php", ".jsp", ".asp", ".aspx"};
    
    public void getFiles(string directoryPath)
    {
        try
        {   
            getAllFiles(directoryPath);
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
            throw;
        }
    }

    public void Clear()
    {
        sourceCodeEntries.Clear();
        mdEntries.Clear();
        licensePath = "none";
        readmePath = "none";
        packageJsonPath = "none";
    }

    //recursive call for getFiles
    public void getAllFiles(string directoryPath)
    {
        //Gets important files in directory path
        string[] filePaths = Directory.GetFiles(directoryPath); 
        foreach (string filePath in filePaths)
        {
            String[] splitFilePath = filePath.Split("/");
            string fileName = splitFilePath[splitFilePath.Length - 1];

            if (jsFileExt.Any(fileName.EndsWith))
            {
                sourceCodeEntries.Add(filePath); 
            }
            else if (fileName.ToLower().Contains("license"))
            {
                licensePath = filePath;
            }
            else if (fileName.ToLower().Contains("readme"))
            {
                readmePath = filePath;
                mdEntries.Add(filePath);
            }
            else if (fileName.EndsWith(".md"))
            {
                mdEntries.Add(filePath);
            }
            else if (fileName.ToLower() == "package.json")
            {
                
                if (packageJsonPath == "none")
                {
                    packageJsonPath = filePath;
                }
            }
        }

        string[] dirs = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);
        foreach (string dir in dirs)
        {
            getAllFiles(dir);
        }
    }
}
