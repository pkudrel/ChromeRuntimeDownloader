using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Feature.Workers.Models;
using ChromeRuntimeDownloader.Models;
using ChromeRuntimeDownloader.Tools;

namespace ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers
{
    public class CefRedistX64DefaultWorker : WorkerBase
    {
        public CefRedistX64DefaultWorker() : base(PackageType.CefRedistX64, "3.2987.1601")
        {
        }


        

        public override void Configure(string src, string dst, PackagesInfo pi)
        {
            var root = Path.Combine(pi.UnzipPath, $"CEF");
            var rootFiles = GetFiles(root);
            var files1 = MatchFiles(root, dst, rootFiles, "x64");
            FilesToCopy.AddRange(files1);

            var localesFilesDir = Path.Combine(root, $"locales");
            var localesFile = GetFiles(localesFilesDir);
            var files2 = MatchFiles(localesFilesDir, dst, localesFile, "x64\\locales");
            FilesToCopy.AddRange(files2);

            var x64FilesDir = Path.Combine(root, $"x64");
            var x64File = GetFiles(x64FilesDir);
            var files3 = MatchFiles(x64FilesDir, dst, x64File, "x64");
            FilesToCopy.AddRange(files3);
        }
    }
}