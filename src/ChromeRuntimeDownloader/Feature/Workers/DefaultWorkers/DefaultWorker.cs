using System.IO;
using ChromeRuntimeDownloader.Common;
using ChromeRuntimeDownloader.Feature.Workers.Models;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers
{
    public class DefaultWorker : WorkerBase
    {
        public override void Configure(string src, string dst, PackagesInfo pi)
        {
            foreach (var copyPath in pi.NugetInfo.CopyPaths)
            {
                var s = copyPath.Src.StartsWith("/") ? copyPath.Src.Substring(1) : copyPath.Src;
                var d = copyPath.Dst.StartsWith("/") ? copyPath.Dst.Substring(1) : copyPath.Dst;
                var src1 = Path.Combine(src, s);
                var dst1 = Path.Combine(dst, d);
                Io.CreateDirIfNotExist(dst1);
                var files = GetFiles(src1);
                var copyList = CreateCopyList(src1, dst1, files);
                FilesToCopy.AddRange(copyList);
            }
        }
    }
}