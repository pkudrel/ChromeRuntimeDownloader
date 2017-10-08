using System.Reflection;

namespace ChromeRuntimeDownloader.Common
{
    public static class Tools
    {
        public static string GetProgramDir()
        {

            return Assembly.GetEntryAssembly().Location;
        }
    }
}