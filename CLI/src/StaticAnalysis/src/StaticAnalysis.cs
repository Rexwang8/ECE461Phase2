using Utility;

namespace StaticAnalysis;
public class StaticAnalysisLibrary
{
    DirectoryTool DirectoryTool = new DirectoryTool();

    public void Analyze(URLInfo urlInfo)
    {
        string repoPath = urlInfo.getPath();
        DirectoryTool.getFiles(repoPath);
        urlInfo.licensePath = DirectoryTool.licensePath;
        urlInfo.readmePath = DirectoryTool.readmePath;
        urlInfo.packageJsonPath = DirectoryTool.packageJsonPath;

        foreach(string file in DirectoryTool.sourceCodeEntries)
        {
            ReadFile(file, urlInfo);
        }

        foreach(string file in DirectoryTool.mdEntries)
        {
            urlInfo.commentCharCount += File.ReadAllLines(file).Sum(s => s.Length);
        }

        DirectoryTool.sourceCodeEntries.Clear();
    }

    //Reads the file and does the static analysis on the file
    public void ReadFile(string filename, URLInfo urlInfo)
    {
        String? line;
        
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(filename);
            
            line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                AnalyzeLine(line, urlInfo);
                line = sr.ReadLine();
            }
            
            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
            throw;
        }
    }

    public void AnalyzeLine(string text, URLInfo urlInfo)
    {
        if (text.StartsWith("//") || text.StartsWith("/*"))
        {
            urlInfo.commentLineCount++;
        }
        else if (text.Contains("//") || text.Contains("/*"))
        {
            urlInfo.commentLineCount++;
            urlInfo.codeLineCount++;
            
            //finds length of code in lines with code and comments
            String[] separator = { "//,", "/*" };
            String[] textArr = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            urlInfo.codeCharCount += textArr[0].Length;
        }
        else
        {
            urlInfo.codeLineCount++;
            urlInfo.codeCharCount += text.Length;
        }


    }
}