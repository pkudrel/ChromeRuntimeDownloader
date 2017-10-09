using System.Collections.Generic;

namespace ChromeRuntimeDownloader.Models
{
    public class PackageVersion
    {
        public string Version { get; set; }
        public List<NugetInfo> Nugets { get; set; }
    }
}