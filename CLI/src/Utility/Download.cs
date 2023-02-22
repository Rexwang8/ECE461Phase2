using CliWrap;
using System.Text;

namespace Utility
{
    static public class Download
    {
        static public void githubUrl(string url, string path)
        {
            try{
                Console.WriteLine("Im trying...");
                var stdOutBuffer = new StringBuilder();
                var stdErrBuffer = new StringBuilder();

                var cliresultGit = Cli.Wrap("python3")
                    .WithArguments($"Utility/gitPython.py")
                    .WithValidation(CommandResultValidation.None)
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                    .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                    .ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                throw;
            }
            
            
        }
    }
}