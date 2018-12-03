using System.Collections.Generic;

namespace ChromeRuntimeDownloader.Models
{
    public class PackageConfig
    {
        public List<NugetDefinition> Nugets { get; set; } = new List<NugetDefinition>();

        public string Name { get; set; }
    }


}