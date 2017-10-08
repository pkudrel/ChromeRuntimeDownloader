using System.Collections.Generic;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Services
{
    public class GetConfig
    {
        public static Config GetDefaultConfig()
        {
            var c = new Config
            {
                CefRedistx64 = new NugetInfo(PackageType.CefRedistX64, "cef.redist.x64", "3.2987.1601"),
                CefRedistx86 = new NugetInfo(PackageType.CefRedistX86, "cef.redist.x86", "3.2987.1601"),
                CefSharpCommon = new NugetInfo(PackageType.CefSharpCommon, "CefSharp.Common", "57.0.0"),
                CefSharpWpf = new NugetInfo(PackageType.CefSharpWpf, "CefSharp.Wpf", "57.0.0")
            };

            c.Packages = new List<NugetInfo>
            {
                new NugetInfo(PackageType.CefSharpCommon, "CefSharp.Common", "57.0.0"),
                new NugetInfo(PackageType.CefSharpWpf, "CefSharp.Wpf", "57.0.0"),
                new NugetInfo(PackageType.CefRedistX64, "cef.redist.x64", "3.2987.1601"),
                new NugetInfo(PackageType.CefRedistX86, "cef.redist.x86", "3.2987.1601")
            };
            return c;
        }
    }
}