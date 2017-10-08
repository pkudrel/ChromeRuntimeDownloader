namespace ChromeRuntimeDownloader.Models
{
    public class NugetInfo
    {
        public NugetInfo(PackageType packageType, string name, string version)
        {
            PackageType = packageType;
            Name = name;
            Version = version;
        }

        public PackageType PackageType { get; }
        public string Name { get; set; }
        public string Version { get; set; }

    }
}