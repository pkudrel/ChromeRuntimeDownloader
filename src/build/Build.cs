using System;
using Helpers;
using Helpers.MagicVersionService;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

class Build : NukeBuild
{
    [Parameter("Build counter from outside environment")] readonly int BuildCounter;

    readonly DateTime BuildDate = DateTime.UtcNow;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [GitRepository] readonly GitRepository GitRepository;

    [Solution("src/ChromeRuntimeDownloader.sln")] readonly Solution Solution;

    Project CrdProject =>
        Solution.GetProject("ChromeRuntimeDownloader").NotNull();


    AbsolutePath SourceDir => RootDirectory / "src";
    AbsolutePath ToolsDir => RootDirectory / "tools";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath DevDir => RootDirectory / "dev";
    AbsolutePath TmpBuild => TemporaryDirectory / "build";
    AbsolutePath LibzPath => ToolsDir / "LibZ.Tool" / "tools" / "libz.exe";
    AbsolutePath NugetPath => ToolsDir / "nuget.exe";

    MagicVersion MagicVersion => MagicVersionFactory.Make(1, 0, 0,
        BuildCounter,
        MagicVersionStrategy.PatchFromCounter,
        BuildDate,
        MachineName);

    Target Information => _ => _
        .Executes(() =>
        {
            var b = MagicVersion;
            Logger.Info($"Host: {Host}");
            Logger.Info($"Version: {b.SemVersion}");
            Logger.Info($"Date: {b.DateTime:s}Z");
            Logger.Info($"FullVersion: {b.InformationalVersion}");
        });


    Target CheckTools => _ => _
        .DependsOn(Information)
        .Executes(() =>
        {
            Downloader.DownloadIfNotExists("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", NugetPath,
                "Nuget");
        });

    Target Clean => _ => _
        .DependsOn(CheckTools)
        .Executes(() =>
        {
            EnsureExistingDirectory(TmpBuild);
            DeleteDirectories(GlobDirectories(TmpBuild, "**/*"));
            EnsureCleanDirectory(ArtifactsDir);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            using (var process = ProcessTasks.StartProcess(
                NugetPath,
                $"restore  {Solution.Path}",
                SourceDir))
            {
                process.AssertWaitForExit();
                ControlFlow.AssertWarn(process.ExitCode == 0,
                    "Nuget restore report generation process exited with some errors.");
            }
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>

        {
            var buildOut = TmpBuild / CommonDir.Build /
                           CrdProject.Name;
            EnsureExistingDirectory(buildOut);

            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetTargetPlatform(MSBuildTargetPlatform.x64)
                .SetOutDir(buildOut)
                .SetVerbosity(MSBuildVerbosity.Quiet)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(MagicVersion.AssemblyVersion)
                .SetFileVersion(MagicVersion.FileVersion)
                .SetInformationalVersion(MagicVersion.InformationalVersion)
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild));
        });

    Target Marge => _ => _
        .DependsOn(Compile)
        .Executes(() =>

        {
            var buildOut = TmpBuild / CommonDir.Build /
                           CrdProject.Name;
            var margeOut = TmpBuild / CommonDir.Merge /
                           CrdProject.Name;

            EnsureExistingDirectory(margeOut);
            CopyDirectoryRecursively(buildOut, margeOut);

            using (var process = ProcessTasks.StartProcess(
                LibzPath,
                "inject-dll --assembly SampleAnalyzer.exe --include *.dll --move",
                margeOut))
            {
                process.AssertWaitForExit();
                ControlFlow.AssertWarn(process.ExitCode == 0,
                    "Libz report generation process exited with some errors.");
            }
        });

    public static int Main() => Execute<Build>(x => x.Compile);
}