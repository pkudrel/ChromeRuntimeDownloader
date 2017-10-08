using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Vendors.ShellProgressBar;

namespace ChromeRuntimeDownloader.Common
{
    public static class Download
    {
        public static async Task<bool> DownloadFileAsync(string fileUrl, string dst)
        {
            var downloadLink = new Uri(fileUrl);
            var file = Path.GetFileName(dst);

            using (var pb = new ProgressBar($"Downloading '{file}' ... "))
            {
                void DownloadProgressChangedEvent(object s, DownloadProgressChangedEventArgs e)
                {
                    pb.Report((double) e.ProgressPercentage / 100);
                }

                using (var webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += DownloadProgressChangedEvent;
                    await webClient.DownloadFileTaskAsync(downloadLink, dst);
                }


                pb.Finish();
            }

            return true;
        }

        public static async Task<string> DownloadNugetAsync(string name, string version, string dstDirectory)
        {
            var url = $"https://www.nuget.org/api/v2/package/{name}/{version}";
            var fileName = $"{name.ToLower()}.{version.ToLower()}.nupkg";
            var fileToDownload = Path.Combine(dstDirectory, fileName);
            await DownloadFileAsync(url, fileToDownload);
            return fileToDownload;
        }
    }
}