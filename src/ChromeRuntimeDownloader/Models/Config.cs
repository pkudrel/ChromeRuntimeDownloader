using System.Collections.Generic;

namespace ChromeRuntimeDownloader.Models
{
    public class Config
    {
        public Dictionary<string, List<NugetInfo>> Packages { get; set; } = new Dictionary<string, List<NugetInfo>>();

        public string DefaultPackageVersion { get; set; }
    }
}