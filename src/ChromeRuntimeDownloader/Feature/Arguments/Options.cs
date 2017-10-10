using CommandLine;
using CommandLine.Text;

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
            HelpText =
                "Package version. Default value: '57.0.0'. By default, the program knows only this version. Other versions should  be specified in config file.")]
        public string PackageVersion { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Copyright = new CopyrightInfo("DenebLab", 2017),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage: app -p Someone");
            help.AddOptions(this);
            return help;
        }
    }
}