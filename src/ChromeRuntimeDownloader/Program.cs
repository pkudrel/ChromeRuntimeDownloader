using System;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common.Bootstrap;
using ChromeRuntimeDownloader.Feature.Arguments;

namespace ChromeRuntimeDownloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static async Task<int> MainAsync(string[] args)
        {
            //var args = new[] { "-n"};

            var env = Boot.Instance.GetAppEnvironment();
            var app = CommandLineDefinition.Make(env);
            return app.Execute(args);
                
        }
    }

    internal class Make
    {
    }
}