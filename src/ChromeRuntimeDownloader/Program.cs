using System;
using System.IO;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common;
using ChromeRuntimeDownloader.Common.Version;
using ChromeRuntimeDownloader.Feature.Arguments;
using ChromeRuntimeDownloader.Feature.MainTask;
using ChromeRuntimeDownloader.Feature.Workers.Services;
using ChromeRuntimeDownloader.Services;
using CommandLine;

namespace ChromeRuntimeDownloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();

            //var x = Task.Run(() => MainAsync(args));
            //Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine($"Chrome Runtime Downloader {VersionGenerator.GetVersion().SemVer}");

            var programDir = Tools.GetProgramDir();
            var options = new Options();
            var isValid = Parser.Default.ParseArgumentsStrict(args, options);


            // Parse in 'strict mode', success or quit
            if (isValid)
            {
                var config = ConfigFactory.GetConfig(programDir, options.Config);

                var workDir = options.Destination == "." || options.Destination == ""
                    ? programDir
                    : options.Destination;

                if (!Directory.Exists(workDir))
                    throw new DirectoryNotFoundException($"Can not find directory: '{workDir}'");

                var packageVersion = string.IsNullOrEmpty(options.Config)
                    ? config.DefaultPackageVersion
                    : options.PackageVersion;


                Console.WriteLine($"Work dir: {workDir}");
                Console.WriteLine($"Package version: {packageVersion}");


                var workerService = new WorkerService();
                var m = new Main(workDir, workerService);
                await m.Make(packageVersion, config);
            }
            else
            {
                // Display the default usage information
                Console.WriteLine(options.GetUsage());
            }
        }
    }
}