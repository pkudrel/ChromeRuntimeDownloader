using System.Collections.Generic;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Defaults
{
    public static class KnownPacks
    {
        public const string DEFAULT_PACKAGE_VERSION = "63.0.1";

        public static KeyValuePair<string, List<NugetInfo>>[] Packs =
        {
            new KeyValuePair<string, List<NugetInfo>>("57.0.0", new List<NugetInfo>
            {
                new NugetInfo(PackageType.CefSharpCommon, "CefSharp.Common", "57.0.0", new[]
                {
                    new CopyPath {Src = "/CefSharp/x64", Dst = "/x64"},
                    new CopyPath {Src = "/CefSharp/x86", Dst = "/x86"}
                }),
                new NugetInfo(PackageType.CefSharpWpf, "CefSharp.Wpf", "57.0.0", new[]
                {
                    new CopyPath {Src = "/CefSharp/x64", Dst = "/x64"},
                    new CopyPath {Src = "/CefSharp/x86", Dst = "/x86"}
                }),
                new NugetInfo(PackageType.CefRedistX64, "cef.redist.x64", "3.2987.1601", new[]
                {
                    new CopyPath {Src = "/CEF", Dst = "/x64"},
                    new CopyPath {Src = "/CEF/locales", Dst = "/x64/locales"},
                    new CopyPath {Src = "/CEF/x64", Dst = "/x64"}
                }),
                new NugetInfo(PackageType.CefRedistX86, "cef.redist.x86", "3.2987.1601", new[]
                {
                    new CopyPath {Src = "/CEF/", Dst = "/x86/"},
                    new CopyPath {Src = "/CEF/locales", Dst = "/x86/locales"},
                    new CopyPath {Src = "/CEF/x86", Dst = "/x86"}
                })
            }),
            new KeyValuePair<string, List<NugetInfo>>("63.0.1", new List<NugetInfo>
            {
                new NugetInfo(PackageType.CefSharpCommon, "CefSharp.Common", "63.0.1", new[]
                {
                    new CopyPath {Src = "/CefSharp/x64", Dst = "/x64"},
                    new CopyPath {Src = "/CefSharp/x86", Dst = "/x86"}
                }),
                new NugetInfo(PackageType.CefSharpWpf, "CefSharp.Wpf", "63.0.1", new[]
                {
                    new CopyPath {Src = "/CefSharp/x64", Dst = "/x64"},
                    new CopyPath {Src = "/CefSharp/x86", Dst = "/x86"}
                }),
                new NugetInfo(PackageType.CefRedistX64, "cef.redist.x64", "3.3239.1723", new[]
                {
                    new CopyPath {Src = "/CEF", Dst = "/x64"},
                    new CopyPath {Src = "/CEF/locales", Dst = "/x64/locales"},
                    new CopyPath {Src = "/CEF/swiftshader", Dst = "/x64/swiftshader"}
                }),
                new NugetInfo(PackageType.CefRedistX86, "cef.redist.x86", "3.3239.1723", new[]
                {
                    new CopyPath {Src = "/CEF/", Dst = "/x86/"},
                    new CopyPath {Src = "/CEF/locales", Dst = "/x86/locales"},
                    new CopyPath {Src = "/CEF/swiftshader", Dst = "/x86/swiftshader"}
                })
            })
        };
    }
}