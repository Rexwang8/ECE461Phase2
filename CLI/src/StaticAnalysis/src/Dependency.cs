namespace StaticAnalysis;
public static class Dependency
{
    public static float GetScore(URLInfo urlInfo)
    {
        Console.WriteLine("-----\nCalculating Dependency");
        String? line;
        String[] lineArr;
        bool dependency = false;
        int totalDependencies = 0;
        int pinnedDependencies = 0;

        if (urlInfo.packageJsonPath == "")
        {
            Console.WriteLine("**WARNING** Package does not have a package.json file");
            return 1;
        }

        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(urlInfo.packageJsonPath);
            
            line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                if (line.Contains(" \"dependencies\":"))
                {
                    dependency = true;
                    line = sr.ReadLine();
                    break;
                }
                    
                line = sr.ReadLine();
            }
            Console.WriteLine("Its fine");
            if (dependency == true)
            {
                Console.WriteLine("Its not fine");
                while (!line.Contains("},"))
                {
                    lineArr = line.Split('"');

                    Console.WriteLine("****** " + lineArr[3]);
                    if (!(lineArr[3].Contains("^") | lineArr[3].Contains("~") | lineArr[3].Contains("-") | !lineArr[3].Contains(".")))
                    {
                        Console.WriteLine("Valid");
                        pinnedDependencies += 1;
                    }
                    
                    totalDependencies += 1;

                    line = sr.ReadLine();
                }

                return pinnedDependencies/totalDependencies;
            }
            
            sr.Close();
            return 1;
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
            throw;
        }

    }
}