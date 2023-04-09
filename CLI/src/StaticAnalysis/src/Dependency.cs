namespace StaticAnalysis;
public static class Dependency
{
    public static float GetScore(URLInfo urlInfo)
    {
        Console.WriteLine("-----\nCalculating Dependency");
        String? line;
        String[] lineArr;
        bool dependency = false;
        float totalDependencies = 0;
        float pinnedDependencies = 0;

        if (urlInfo.packageJsonPath == "" | urlInfo.packageJsonPath == "none")
        {
            Console.WriteLine("**WARNING** Package does not have a package.json file");
            return -1;
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
                    if (!line.Contains("}"))
                    {
                        dependency = true;
                    }
                    line = sr.ReadLine();
                    break;
                }
                    
                line = sr.ReadLine();
            }
            
            //If there are dependencies, check them
            if (dependency == true)
            {
                while (!line.Contains("}"))
                {
                    Console.WriteLine(line);
                    lineArr = line.Split('"');

                    pinnedDependencies += CheckPinned(lineArr[3]);
                    totalDependencies += 1;

                    line = sr.ReadLine();
                }

                sr.Close();
                Console.WriteLine("Pinned Dependencies: " + pinnedDependencies);
                Console.WriteLine("Total Dependencies: " + totalDependencies);
                return pinnedDependencies/totalDependencies;
            }
            
            sr.Close();
            return 1;
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
            return 0;
        }

    }

    private static int CheckPinned(string dependency)
    {
        Console.WriteLine("Checking " + dependency);
        //Check if dependency is contains a letter
        if (dependency.Any(x => char.IsLetter(x)))
        {
            //if dependency contains a letter not x
            if (dependency.Any(x => char.IsLetter(x) && (x != 'x' || x != 'X')))
            {
                return 0;
            }

            string[] dependencyRange = dependency.Split('.');
            
            //Check if major contains a letter x
            if (dependencyRange[0].Contains('x') || dependencyRange[0].Contains('X'))
            {
                return 0;
            }
            //Check if minor contains a letter x
            if (dependencyRange[1].Contains('x') || dependencyRange[1].Contains('X'))
            {
                return 0;
            }

            return 1;
        }
        //Check if dependency starts with a carat
        if (dependency.StartsWith('^'))
        {
            if (dependency[1] == '0')
            {
                return 1;
            }
            
            return 0;
        }
        //Check if dependency starts with a tilde
        else if (dependency.StartsWith('~'))
        {
            if (dependency.Contains('.'))
            {
                Console.WriteLine(" Dependency has a period");
                return 1;
            }
            return 0;
        }
        //Check if dependency has a dash
        else if (dependency.Contains('-'))
        {
            string[] dependencyRange = dependency.Split('-');
            //Major version is not the same
            if (dependencyRange[0][0] == dependencyRange[1][0])
            {
                return 1;
            }

            //Minor version is not the same
            if (dependencyRange[0].Split('.')[1] == dependencyRange[1].Split('.')[1])
            {
                return 1;
            }

            return 0;
        }
        //Check if dependency starts with a number
        else if (Char.IsDigit(dependency[0]))
        {
            if (dependency.Contains('.'))
            {
                return 1;
            }

            return 0;
        }
        //Check if dependency is a wildcard
        else if (dependency[0] == '*')
        {
            return 0;
        }
        
        Console.WriteLine("Unknowned Dependency");
        return 0;
    }
}