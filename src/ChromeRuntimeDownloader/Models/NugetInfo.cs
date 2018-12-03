using System.Collections.Generic;

namespace ChromeRuntimeDownloader.Models
{
    public class NugetDefinition
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<CopyPath> CopyPaths { get; set; } = new List<CopyPath>();
    }
}