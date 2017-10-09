using System;
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
            Io.ClearFolder(_tmpDir);
        }

        public async Task Make(string runTimeVersion, Config config)
        {
            var p = config.Packages.Select(x => new PackagesInfo(x)).ToArray();
            var p1 = await Download(p);
            var p2 = await Extract(p1);
            var p3 = await CopyToDestination(p1, runTimeVersion);
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
            Io.RemoveFolder(tmp);
            Io.CreateDirIfNotExist(tmp);
            foreach (var packagesInfo in packages)
            {
                var filePath = await Common.Download.DownloadNugetAsync(
                    packagesInfo.NugetInfo.Name,
                    packagesInfo.NugetInfo.Version,
                    tmp);
                packagesInfo.SetNugetPath(filePath);
            }

            return packages;
        }

        private async Task<PackagesInfo[]> Extract(PackagesInfo[] packages)
        {
            var tmp = Path.Combine(_tmpDir, "extract");
            Io.RemoveFolder(tmp);
            Io.CreateDirIfNotExist(tmp);
            foreach (var packagesInfo in packages)
            {
                var dstDir = Path.Combine(tmp,
                    Path.GetFileNameWithoutExtension(packagesInfo.NugetPath) ?? throw new InvalidOperationException());
                Io.CreateDirIfNotExist(dstDir);
                var filePath = await Common.Extract.ExtractZipToDirectory(packagesInfo.NugetPath, dstDir);
                packagesInfo.SetUnzipPath(dstDir);
            }

            return packages;
        }
    }
}