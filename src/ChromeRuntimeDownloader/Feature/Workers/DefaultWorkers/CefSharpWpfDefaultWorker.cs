using System;
using System.IO;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Feature.Workers.Models;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers
{
    public class CefSharpWpfDefaultWorker : WorkerBase
    {
        public CefSharpWpfDefaultWorker() : base(PackageType.CefSharpWpf, "57.0.0")
        {
        }


        public override void Configure(string src, string dst, PackagesInfo pi)
        {
            var root = Path.Combine(pi.UnzipPath, $"CefSharp");
            var rootFiles = GetFiles(root);
            var files1 = MatchFiles(root, dst, rootFiles, "");
            FilesToCopy.AddRange(files1);

        }

    }
}