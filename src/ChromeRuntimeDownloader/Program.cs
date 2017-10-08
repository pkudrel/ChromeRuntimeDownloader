using System;
using System.Threading.Tasks;
using ChromeRuntimeDownloader.Feature.Arguments;
using ChromeRuntimeDownloader.Feature.MainTask;
using ChromeRuntimeDownloader.Feature.Workers.DefaultWorkers;
using ChromeRuntimeDownloader.Feature.Workers.Services;
using ChromeRuntimeDownloader.Models;
using ChromeRuntimeDownloader.Services;

namespace ChromeRuntimeDownloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Async";
            var x = Task.Run(() => MainAsync(args));
            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {

            var options = new Options();
            var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            //var i = @"https://clients.deneblab.com/dl/InteractiveExtractor/runtime.x86.57.0.0.zip";
            //var d = @"D:\!112\";
            ////await x.DownloadFileAsync(i, d);
            //var t = await Download.DownloadNugetAsync("CefSharp.Wpf", "57.0.0", d);

            //var t2 = await Download.DownloadNugetAsync("CefSharp.Common", "57.0.0", d);
            //var f1 = Path.Combine(d, "cefsharp.common.57.0.0.nupkg");
            //var d1 = Path.Combine(d, "a");
            //var t3 = await Extract.ExtractZipToDirectory(f1, d1);

            //var cc = new CefSharpCommon("D:\\!112\\a", "D:\\!112\\a");


            var config = GetConfig.GetDefaultConfig();

            WorkerService workerService = new WorkerService();
            var m = new Main("C:\\work\\DenebLab\\ChromeRuntimeDownloader\\stuff\\tmp\\chrome-runtime", workerService);
            await m.Make("57.0.0.", config);

            var srcDir =
                @"C:\work\DenebLab\ChromeRuntimeDownloader\stuff\tmp\chrome-runtime\tmp\extract\cef.redist.x64.3.2987.1601";
            var dstDir = @"C:\work\DenebLab\ChromeRuntimeDownloader\stuff\tmp\chrome-runtime\57.0.0";



            NugetInfo nugetInfo = new NugetInfo(PackageType.CefRedistX64, "cef.redist.x64", "3.2987.1601");
            PackagesInfo p = new PackagesInfo(nugetInfo);
            p.SetUnzipPath(srcDir);
            var w = new CefRedistX64DefaultWorker();
            w.Configure(srcDir, dstDir,p);
            await  w.Execute(p);
        }
    }
}