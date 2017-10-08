using System.Collections.Generic;

namespace ChromeRuntimeDownloader.Models
{
    public class Config
    {
        public NugetInfo CefSharpWpf { get; set; }
        public NugetInfo CefSharpCommon { get; set; }
        public NugetInfo CefRedistx64 { get; set; }
        public NugetInfo CefRedistx86 { get; set; }

        public List<NugetInfo> Packages { get; set; }
    }
}