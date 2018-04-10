using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common;
using ChromeRuntimeDownloader.Common.Version;
using ChromeRuntimeDownloader.Feature.Arguments;
using ChromeRuntimeDownloader.Feature.MainTask;
using ChromeRuntimeDownloader.Feature.Workers.Services;
using ChromeRuntimeDownloader.Models;
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
            var parser = new Parser(config => { config.HelpWriter = null; });
            var result = parser.ParseArguments<Options>(args);
            var res = result
                .MapResult(
                    async options => await RunAndReturnExitCodeAsync(options),
                    errors => ShowErrors(result));

            await res;
        }


        private static Task<int> ShowErrors(ParserResult<Options> errors)
        {
            var helpText = HelpText.AutoBuild(errors);
            helpText.Heading = $"ChromeRuntimeDownloader {VersionGenerator.GetVersion().SemVer}";
            helpText.Copyright = "Copyright (c) 2017-2018 DenebLab";
            Console.WriteLine(helpText);
            return Task.FromResult(1);
        }


        private static async Task<int> RunAndReturnExitCodeAsync(Options options)
        {
            var programDir = Tools.GetProgramDir();
            var config = ConfigFactory.GetConfig(programDir, options.Config);

            if (options.ShowList) return ShowPackagesList(config);

            var workDir = GetWorkDir(options, programDir);

            if (!Directory.Exists(workDir))
            {
                Console.WriteLine($"Can not find directory: '{workDir}'");
                return 0;
            }

            var packageVersion = config.DefaultPackageVersion;

            if (!string.IsNullOrEmpty(options.PackageVersion))
            {
                var packExists = config.Packages.Any(x => x.Key == options.PackageVersion);
                if (!packExists)
                {
                    Console.WriteLine($"Can not find package version: '{options.PackageVersion}'");
                    return 0;
                }

                packageVersion = options.PackageVersion;
            }

            Console.WriteLine($"Work dir: {workDir}");
            Console.WriteLine($"Package version: {packageVersion}");

            var workerService = new WorkerService();
            var m = new Main(workDir, workerService);
            await m.Make(packageVersion, config);

            return 0;
        }

        private static string GetWorkDir(Options options, string programDir)
        {
            return options.Destination == "." || options.Destination == ""
                ? programDir
                : options.Destination;
        }

        private static int ShowPackagesList(Config config)
        {
            foreach (var p in config.Packages)
            {
                Console.WriteLine($"Package: '{p.Key}' ");
                foreach (var i in p.Value) Console.WriteLine($"{i.Name} {i.Version}");
                Console.WriteLine();
            }

            return 0;
        }
    }
}