using System.Collections.Generic;
using ChromeRuntimeDownloader.Defaults;
using CommandLine;
using CommandLine.Text;

namespace ChromeRuntimeDownloader.Feature.Arguments
{
    public class Options
    {
        [Option('d', "destination",
            Default = ".",
            HelpText = "Directory where program creates chrome-runtime. Default value: current program directory")]
        public string Destination { get; set; }

        [Option('l', "list",
            HelpText = "List known packages")]
        public bool ShowList { get; set; }


        [Option('c', "configFile",
            Default= "crd-config.json",
            HelpText = "Config file. Default value: 'crd-config.json'")]
        public string Config { get; set; }


        [Option('p', "packageVersion",
         
            HelpText =
                "Package version. Default value: '"+ KnownPacks.DEFAULT_PACKAGE_VERSION + "'. By default, the program knows only this version. Other versions should  be specified in config file.")]
        public string PackageVersion { get; set; }

        //[HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Copyright = new CopyrightInfo("DenebLab aa", 2017),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage: app -p Someone");
         //   help.AddOptions(this);
            return help;
        }

        public Options()
        {
            
        }


      
    }
}