using CommandLine;
using CommandLine.Text;

namespace ChromeRuntimeDownloader.Feature.Arguments
{
    public class Options
    {
        [Option('c', "configFile",
            Default = null,
            HelpText = "Config file. Default value: " + Constants.DEFAULT_PACKAGE_CONFIG_FILE)]
        public string PackageConfig { get; set; }

        [Option("Clean",
            Default = null,
            HelpText = "Force clean after process. ")]
        public bool? Clean { get; set; }

        [Option('d', "destination",
            Default = null,
            HelpText = "Directory where program creates chrome-runtime. Default value: " +
                       Constants.DEFAULT_PACKAGE_DIR)]
        public string Destination { get; set; }

        //[HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Copyright = new CopyrightInfo("DenebLab", 2018),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage: app -p Someone");
            //   help.AddOptions(this);
            return help;
        }
    }
}