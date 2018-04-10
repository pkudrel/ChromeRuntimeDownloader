using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common;
using ChromeRuntimeDownloader.Common.Version;
using ChromeRuntimeDownloader.Feature.Arguments;
using ChromeRuntimeDownloader.Feature.MainTask;
using ChromeRuntimeDownloader.Feature.Workers.Services;
using ChromeRuntimeDownloader.Services;
using CommandLine;
using Newtonsoft.Json;

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


                if (options.ShowList)
                {
                    foreach (var p in config.Packages)
                    {
                        Console.WriteLine($"Package: '{p.Key}' ");
                        foreach (var i in p.Value) Console.WriteLine($"{i.Name} {i.Version}");
                        Console.WriteLine();
                    }
                    return;
                }

                var workDir = options.Destination == "." || options.Destination == ""
                    ? programDir
                    : options.Destination;
                if (!Directory.Exists(workDir))
                {
                    Console.WriteLine($"Can not find directory: '{workDir}'");
                    return;
                }

                var packageVersion = config.DefaultPackageVersion;

                if (!string.IsNullOrEmpty(options.PackageVersion))
                {
                    var packExists = config.Packages.Any(x => x.Key == options.PackageVersion);
                    if (!packExists)
                    {
                        Console.WriteLine($"Can not find package version: '{options.PackageVersion}'");
                        return;
                    }

                    packageVersion = options.PackageVersion;
                }


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