using System;
using System.Collections.Generic;
using System.Linq;
using ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers;
using ChromeRuntimeDownloader.Feature.Workers.Models;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Feature.Workers.Services
{
    public class WorkerService
    {
        private readonly List<IWorker> _workers = new List<IWorker>();

        public void AddWorker(IWorker worker)
        {
            _workers.Add(worker);
        }

        public IWorker GetWorker(PackageType packageType, string version)
        {
            var worker = _workers.FirstOrDefault(x => x.Type == packageType && x.Version == version);
            return worker ?? GetDefaultWorker(packageType);
        }

        private IWorker GetDefaultWorker(PackageType packageType)
        {
            switch (packageType)
            {
                case PackageType.CefRedistX86:
                    return new CefRedistX86DefaultWorker();

                case PackageType.CefRedistX64:
                    return new CefRedistX64DefaultWorker();
                case PackageType.CefSharpCommon:
                    return new CefSharpCommonDefaultWorker();
                case PackageType.CefSharpWpf:
                    return new CefSharpWpfDefaultWorker();
                default:
                    throw new ArgumentOutOfRangeException(nameof(packageType), packageType, null);
            }
        }
    }
}