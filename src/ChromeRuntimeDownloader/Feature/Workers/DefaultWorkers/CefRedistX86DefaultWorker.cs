using System;
using System.IO;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Feature.Workers.Models;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers
{
    public class CefRedistX86DefaultWorker : WorkerBase
    {
        public CefRedistX86DefaultWorker() : base(PackageType.CefRedistX86, "3.2987.1601")
        {
        }


      

        public override void Configure(string src, string dst, PackagesInfo pi)
        {
            var root = Path.Combine(pi.UnzipPath, $"CEF");
            var rootFiles = GetFiles(root);
            var files1 = MatchFiles(root, dst, rootFiles, "x86");
            FilesToCopy.AddRange(files1);

            var localesFilesDir = Path.Combine(root, $"locales");
            var localesFile = GetFiles(localesFilesDir);
            var files2 = MatchFiles(localesFilesDir, dst, localesFile, "x86\\locales");
            FilesToCopy.AddRange(files2);

            var x86FilesDir = Path.Combine(root, $"x86");
            var x86Files = GetFiles(x86FilesDir);
            var files3 = MatchFiles(x86FilesDir, dst, x86Files, "x86");
            FilesToCopy.AddRange(files3);

        }
    }
}