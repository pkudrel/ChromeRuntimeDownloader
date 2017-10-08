using CommandLine;

namespace ChromeRuntimeDownloader.Feature.Arguments
{
    public class Options
    {
        [Option('d', "destination", DefaultValue = ".",
            HelpText = "Directory where program creates chrome-runtime. Default value: current program directory")]
        public string Destination { get; set; }
    }
}