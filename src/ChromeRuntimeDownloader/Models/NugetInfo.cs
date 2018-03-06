namespace ChromeRuntimeDownloader.Models
{
    public class NugetInfo
    {
        public NugetInfo(PackageType packageType, string name, string version, CopyPath[] copyPaths)
        {
            PackageType = packageType;
            Name = name;
            Version = version;
            CopyPaths = copyPaths;
        }

        public PackageType PackageType { get; }
        public string Name { get; set; }
        public string Version { get; set; }
        public CopyPath[] CopyPaths { get; }
    }
}