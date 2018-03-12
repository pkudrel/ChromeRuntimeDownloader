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
using CommandLine.Text;

namespace ChromeRuntimeDownloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var programDir = Tools.GetProgramDir();

            var parser = new Parser(config => { config.HelpWriter = null; });
            var isValid = parser.ParseArguments<Options>(args);


            var res = isValid
                .WithParsed(async options =>
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
                })
                .WithNotParsed(errors =>
                {
                    var helpText = HelpText.AutoBuild(isValid);
                    helpText.Heading = $"ChromeRuntimeDownloader {VersionGenerator.GetVersion().SemVer}";
                    helpText.Copyright = "Copyright (c) 2017-2018 DenebLab";
                    Console.WriteLine(helpText);
                });
            // Parse in 'strict mode', success or quit
        }
    }
}