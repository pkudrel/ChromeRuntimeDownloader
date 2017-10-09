using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Common;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.Workers.Models
{
    public abstract class WorkerBase : IWorker
    {
        protected List<(string src, string dst)> FilesToCopy = new List<(string src, string dst)>();

        public WorkerBase(PackageType type, string packageVersion)
        {
            Type = type;
            Version = packageVersion;
        }

        public PackageType Type { get; }
        public string Version { get; }


        public async Task Execute(PackagesInfo packagesInfo)
        {
            await Io.CopyFilesAsync(FilesToCopy, $"Copying '{packagesInfo.NugetInfo.Name}' ... ");
        }

        public abstract void Configure(string src, string dst, PackagesInfo pi);


        public List<string> GetFiles(string dir)
        {
            var list = new List<string>();
            var di = new DirectoryInfo(dir);
            list.AddRange(di.GetFiles().Select(x => x.Name));
            return list;
        }


        public List<(string src, string dst)> MatchFiles(string srcDir, string dstDir, List<string> files,
            string dstSufix)
        {
            var list = new List<(string src, string dst)>();

            foreach (var file in files)
            {
                var srcFullPath = Path.Combine(srcDir, file);
                var dst = Path.Combine(dstDir, dstSufix, file);
                list.Add((srcFullPath, dst));
            }


            return list;
        }
    }

    public interface IWorker
    {
        PackageType Type { get; }
        string Version { get; }
        Task Execute(PackagesInfo packagesInfo);
        void Configure(string src, string dst, PackagesInfo pi);
    }
}