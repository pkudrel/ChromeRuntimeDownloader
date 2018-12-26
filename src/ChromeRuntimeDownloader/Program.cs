using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
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


        private static async Task MainAsync(string[] args)
        {
            //var args = new[] { "-n"};

            var env = Boot.Instance.GetAppEnvironment();
            var definition = new CommandLineDefinition();
            var rootCommand = definition.GetRootCommand(env);
            var builder = new CommandLineBuilder(rootCommand);
            var parser = builder
                .UseHelp()
                .UseParseDirective()
                .UseDebugDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseParseErrorReporting()
                .UseExceptionHandler()
                .CancelOnProcessTermination()
                .Build();


          


            try
            {
                await parser.InvokeAsync(args);
            }
            catch (Exception e)
            {
                    Console.WriteLine(e.Message);
                    throw;
            }
        }
        
    }
}