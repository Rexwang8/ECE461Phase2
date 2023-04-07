using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NUglify;

namespace Utility;
public class Debloater
{
    public async Task Debloat(string filepath)
    {
        //FINDING JS FILES
        var builder = new WebHostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureServices(services => services.AddNodeServices());

        using (var host = builder.Build())
        {
            var env = host.Services.GetService<IWebHostEnvironment>();
            string path = Path.Combine(env.ContentRootPath, @"path\to\your\repository");

            await MinifyJsFilesAsync(path);
            Console.WriteLine("Minification complete.");
        }
        //DELETING README AND LICENSE
        DirectoryInfo dir = new DirectoryInfo(@filepath);
        foreach (FileInfo file in dir.GetFiles())
        {
            // Check if the file is not required for your project
            if (file.Name == "README.md" || file.Name == "LICENSE")
            {
                file.Delete();
            }
        }
        //FINDING AND DECOMPRESSING IMAGES
        string[] extensions = new [] {".jpg", ".png", ".gif"};
        string images = SearchForImages(filepath, extensions);
        Console.WriteLine(images);
    }
    static string SearchForImages(string path, string[] extensions)
    {
        StringBuilder sb = new StringBuilder();
        try
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (Array.IndexOf(extensions, Path.GetExtension(file)) != -1)
                {
                    sb.AppendLine(file);
                }
            }
            
            foreach (string dir in Directory.GetDirectories(path))
            {
                sb.Append(SearchForImages(dir, extensions));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        
        return sb.ToString();
    }
    static async Task MinifyJsFilesAsync(string path)
    {
        string[] jsFiles = Directory.GetFiles(path, "*.js", SearchOption.AllDirectories);


        foreach (var file in jsFiles)
        {
            var result = Uglify.Js(File.ReadAllText(file));
            File.WriteAllText($"minified_{file}", result.Code);
        }
    }
}
