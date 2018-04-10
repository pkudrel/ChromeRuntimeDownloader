using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common;
using ChromeRuntimeDownloader.Feature.Workers.Services;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.MainTask
{
    public class Main
    {
        private readonly string _dstRuntimeDir;
        private readonly string _tmpDir;
        private readonly WorkerService _workerService;

        public Main(string dstRuntimeDir, WorkerService workerService)
        {
            _dstRuntimeDir = dstRuntimeDir;
            _workerService = workerService;
            _tmpDir = Path.Combine(_dstRuntimeDir, "tmp");
            Io.CreateDirIfNotExist(_tmpDir);
            //Io.ClearFolder(_tmpDir);
        }

        public async Task Make(string runTimeVersion, Config config)
        {
            var set = config.Packages.FirstOrDefault(x => x.Key == runTimeVersion).Value;
            if (set == null)
                throw new ArgumentNullException($"Cannot find package version: '{runTimeVersion}'");


            Console.WriteLine($"Runtime will be created in: {Path.Combine(_dstRuntimeDir, runTimeVersion)}");
            Console.WriteLine("Application will use those packages, to create runtime:");
            foreach (var nugetInfo in set) Console.WriteLine($"Name: {nugetInfo.Name}; Version: {nugetInfo.Version}");


            Console.WriteLine($"Begin process");
            var sw = new Stopwatch();
            sw.Start();
            var p = set.Select(x => new PackagesInfo(x)).ToArray();
            var p1 = await Download(p);
            var p2 = Extract(p1);
            var p3 = await CopyToDestination(p2, runTimeVersion);
            sw.Stop();
            Console.WriteLine($"Done - process took: {sw.ElapsedMilliseconds / 1000}s");
            // Io.RemoveFolder(_tmpDir);
        }

        private async Task<string> CopyToDestination(PackagesInfo[] packages, string runTimeVersion)
        {
            var dst = Path.Combine(_dstRuntimeDir, runTimeVersion);
            Io.RemoveFolder(dst);
            Io.CreateDirIfNotExist(dst);
            foreach (var pi in packages)
            {
                var worker =
                    _workerService.GetWorker(pi.NugetInfo.PackageType, pi.NugetInfo.Version);

                worker.Configure(pi.UnzipPath, dst, pi);
                await worker.Execute(pi);
            }

            return dst;
        }

        private async Task<PackagesInfo[]> Download(PackagesInfo[] packages)
        {
            var tmp = Path.Combine(_tmpDir, "download");
            Io.CreateDirIfNotExist(tmp);
            foreach (var packagesInfo in packages)
            {
                var n = packagesInfo.NugetInfo;
                var url = $"https://www.nuget.org/api/v2/package/{n.Name}/{n.Version}";

                var fileName = $"{n.Name.ToLower()}.{n.Version.ToLower()}.nupkg";
                var dstFile = Path.Combine(tmp, fileName);
                if (!File.Exists(dstFile)) await Common.Download.DownloadFileAsync(url, dstFile);
                packagesInfo.SetNugetPath(dstFile);
            }

            return packages;
        }

        private PackagesInfo[] Extract(PackagesInfo[] packages)
        {
            var tmp = Path.Combine(_tmpDir, "extract");
            Io.RemoveFolder(tmp);
            Io.CreateDirIfNotExist(tmp);
            foreach (var packagesInfo in packages)
            {
                var dstDir = Path.Combine(tmp,
                    Path.GetFileNameWithoutExtension(packagesInfo.NugetPath) ?? throw new InvalidOperationException());
                Io.CreateDirIfNotExist(dstDir);
                var filePath = Common.Extract.ExtractZipToDirectory(packagesInfo.NugetPath, dstDir);
                packagesInfo.SetUnzipPath(dstDir);
            }

            return packages;
        }
    }
}