using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common.Bootstrap;
using ChromeRuntimeDownloader.Feature.MainTask;
using ChromeRuntimeDownloader.Services;

namespace ChromeRuntimeDownloader.Feature.Arguments
{
    public class CommandLineDefinition
    {
        public RootCommand GetRootCommand(AppEnvironment env)
        {
            var configFileString = new Option(
                new[] { "--config-file" },
                $"Config file. Default value: {Constants.DEFAULT_PACKAGE_CONFIG_FILE}",
                new Argument<string>("")
                {
                    ArgumentType = typeof(string),
                    Description = $"Config file. Default value: {Constants.DEFAULT_PACKAGE_CONFIG_FILE}",
                    Arity = ArgumentArity.ExactlyOne,
                    Name = "configFile"
                }.Decorate(argument =>
                {
                    argument.AddValidator(a =>
                    {
                       // return "aaa";
                        return null;
                        return string.Empty;
                    });
                })
            );


            var destinationString = new Option(
                new[] {"-d", "--destination"},
                $"Directory where program creates chrome-runtime. Default value: {Constants.DEFAULT_PACKAGE_DIR}"
                , new Argument<string>(Constants.DEFAULT_PACKAGE_DIR)
                {
                    Name = "destination"
                }
            );


            var arg2 = new Argument();
            arg2.ArgumentType = typeof(bool);
            arg2.Arity = ArgumentArity.OneOrMore;
            arg2.Name = "Clean -- Name";
            arg2.Description = "Clean -- Description";

            var cleanBool = new Option(
                new[] {"--clean"},
                "Force clean after process.",
                new Argument<bool>
                {
                    Name = "clean",
                    Description = "Clean -- Description"
                }
            );


            var rootCommand = new RootCommand();
            rootCommand.Description = "Chrome runtime downloader. Copyright(c) 2017-2019 DenebLab";
            rootCommand.AddOption(configFileString);
            rootCommand.AddOption(destinationString);
            rootCommand.AddOption(cleanBool);

            var method = typeof(CommandLineDefinition).GetMethod(nameof(DoSomething));
            rootCommand.Handler = CommandHandler.Create(method, () => this);
            return rootCommand;
        }

        private MethodInfo GetMethodInfo(string name)
        {
            return typeof(CommandLineDefinition)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == name);
        }

        public async Task DoSomething(string configFile, string destination, bool clean)
        {
            var env = Boot.Instance.GetAppEnvironment();
            var options = new Options(configFile, destination, clean);
            var setting = SettingsBuilder.Create(env, options);
            var mpc = MainProcessSettingsBuilder.Create(env, setting);
            Console.WriteLine($"Package config source: '{mpc.PackageConfigSource}'");
            Console.WriteLine($"Destination dir: '{mpc.Destination}'");
            Console.WriteLine($"Package id: '{mpc.PackageConfig.Name}'");
            var mp = new MainProcess(env);
            await mp.Do(mpc);
        }
    }
}