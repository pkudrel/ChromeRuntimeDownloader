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
            return worker ?? new DefaultWorker();
        }
    }
}