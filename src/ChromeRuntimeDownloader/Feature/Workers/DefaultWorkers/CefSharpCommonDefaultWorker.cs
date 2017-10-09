using System;
using System.IO;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Feature.Workers.Models;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers
{
    public class CefSharpCommonDefaultWorker : WorkerBase
    {
        public CefSharpCommonDefaultWorker() : base(PackageType.CefSharpCommon, "57.0.0")
        {
        }



        public override void Configure(string src, string dst, PackagesInfo pi)
        {
            var root = Path.Combine(pi.UnzipPath, $"CefSharp");

            var x86FilesDir = Path.Combine(root, $"x86");
            var x86Files = GetFiles(x86FilesDir);
            var pack1 = MatchFiles(x86FilesDir, dst, x86Files, "x86");
            FilesToCopy.AddRange(pack1);

            var x64FilesDir = Path.Combine(root, $"x64");
            var x64File = GetFiles(x64FilesDir);
            var pack2 = MatchFiles(x64FilesDir, dst, x64File, "x64");
            FilesToCopy.AddRange(pack2);


        }
    }
}