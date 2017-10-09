using System.IO;
using System.Reflection;

namespace ChromeRuntimeDownloader.Common
{
    public static class Tools
    {
        public static string GetProgramDir()
        {
            var app = Assembly.GetEntryAssembly().Location;
            var dir = Path.GetDirectoryName(app);
            return dir;
        }
    }
}