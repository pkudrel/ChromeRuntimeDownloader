using System.Collections.Generic;
using System.IO;
using ChromeRuntimeDownloader.Models;

namespace ChromeRuntimeDownloader.Services
{
    public static class ConfigFactory
    {
        public static Config GetConfig(string programDir, string configFile)
        {
            var config = GetDefaultConfig();

            if (string.IsNullOrEmpty(configFile)) return config;

            // theoretically it should works for file in program directory and relatives paths
            var fileInProgramDirectory = Path.GetFullPath(Path.Combine(programDir, configFile));
            if (File.Exists(fileInProgramDirectory)) return MargeConfigs(config, fileInProgramDirectory);

            // rootPath
            if (Path.IsPathRooted(configFile) && File.Exists(configFile))
                return MargeConfigs(config, configFile);


            return config;
        }

        private static Config MargeConfigs(Config config, string configFilePath)
        {
            var json = File.ReadAllText(configFilePath);
            var configDisk = SimpleJson.SimpleJson.DeserializeObject<Config>(json);

            //packages
            foreach (var package in configDisk.Packages)
                if (!config.Packages.ContainsKey(package.Key))
                    config.Packages.Add(package.Key, package.Value);
                else
                    // override default version
                    config.Packages[package.Key] = package.Value;

            //version
            if (!string.IsNullOrEmpty(configDisk.DefaultPackageVersion))
                config.DefaultPackageVersion = configDisk.DefaultPackageVersion;

            return config;
        }

        public static Config GetDefaultConfig()
        {
            var config = new Config();

            var defaultPack = new KeyValuePair<string, List<NugetInfo>>("57.0.0", new List<NugetInfo>
            {
                new NugetInfo(PackageType.CefSharpCommon, "CefSharp.Common", "57.0.0"),
                new NugetInfo(PackageType.CefSharpWpf, "CefSharp.Wpf", "57.0.0"),
                new NugetInfo(PackageType.CefRedistX64, "cef.redist.x64", "3.2987.1601"),
                new NugetInfo(PackageType.CefRedistX86, "cef.redist.x86", "3.2987.1601")
            });


            config.Packages.Add(defaultPack.Key, defaultPack.Value);
            config.DefaultPackageVersion = "57.0.0";
            return config;
        }
    }
}