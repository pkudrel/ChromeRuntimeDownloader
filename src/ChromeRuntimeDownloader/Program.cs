using System;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common.Bootstrap;
using ChromeRuntimeDownloader.Feature.Arguments;
using ChromeRuntimeDownloader.Feature.MainTask;
using ChromeRuntimeDownloader.Services;
using CommandLine;
using CommandLine.Text;

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
            var env = AppEnvironmentBuilder.Instance.GetAppEnvironment();
            var helpText = HelpText.AutoBuild(errors);
            helpText.Heading = $"ChromeRuntimeDownloader {env.AppVersion.SemVer}";
            helpText.Copyright = "Copyright (c) 2017-2018 DenebLab";
            Console.WriteLine(helpText);
            return Task.FromResult(1);
        }


        private static async Task<int> RunAndReturnExitCodeAsync(Options options)
        {
            var env = Boot.Instance.GetAppEnvironment();
            var setting = SettingsBuilder.Create(env, options);
            var mpc = MainProcessSettingsBuilder.Create(env, setting);
            Console.WriteLine($"Package config source: '{mpc.PackageConfigSource}'");
            Console.WriteLine($"Destination dir: '{mpc.Destination}'");
            Console.WriteLine($"Package id: '{mpc.PackageConfig.Name}'");


            var mp = new MainProcess(env);
            await mp.Do(mpc);
            return await Task.FromResult(0);
        }
    }
}