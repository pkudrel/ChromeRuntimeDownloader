using System.Text;
using CommandLine;

namespace ChromeRuntimeDownloader.Feature.Arguments
{
    public class Options
    {
        [Option('d', "destination",
            DefaultValue = ".",
            HelpText = "Directory where program creates chrome-runtime. Default value: current program directory")]
        public string Destination { get; set; }


        [Option('c', "configFile",
            DefaultValue = "crd-config.json",
            HelpText = "Config file. Default value: 'crd-config.json'")]
        public string Config { get; set; }


        [Option('p', "packageVersion",
            DefaultValue = "57.0.0",
            HelpText = "Package version. Default value: '57.0.0'. By default, the program knows only this version. Other versions should  be specified in config file.")]
        public string PackageVersion { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }

    }
}