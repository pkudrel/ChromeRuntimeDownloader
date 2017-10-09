namespace ChromeRuntimeDownloader.Models
{
    public class PackagesInfo
    {
        public PackagesInfo(NugetInfo nugetInfo)
        {
            NugetInfo = nugetInfo;
        }

        public string NugetPath { get; private set; }
        public string UnzipPath { get; private set; }

        public NugetInfo NugetInfo { get; }

        public void SetNugetPath(string nugetPath)
        {
            NugetPath = nugetPath;
        }

        public void SetUnzipPath(string unzipPath)
        {
            UnzipPath = unzipPath;
        }
    }
}