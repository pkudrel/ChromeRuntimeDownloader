using System;

namespace Helpers.MagicVersionService
{
    public class MagicVersion
    {
        public MagicVersion(MagicVersionGitSubData gitData, MagicVersionSimple simple) : this(gitData)
        {

            Major = simple.Major;
            Minor = simple.Minor;
            Patch = simple.Patch;
            BuildCounter = simple.BuildCounter;
            Special = simple.Special;
            DateTime = simple.DateUtc;
            Env = simple.Env;

        }

        public MagicVersion(MagicVersionGitSubData gitData)
        {
            GitSha = gitData.GitSha;
            GitCommitNumber = gitData.GitCommitNumber;
            GitBranch = gitData.GitBranch;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Private { get; } = 0;
        public int BuildCounter { get; }
        public string Special { get; }
        public string Env { get; set; }
        public string GitBranch { get; }
        public string GitSha { get; }
        public int GitCommitNumber { get; }

        public DateTime DateTime { get; } = DateTime.UtcNow;

        public string Version => $"{Major}.{Minor}.{Patch}";
        public string MajorMinor => $"{Major}.{Minor}.{Patch}";

        public string AssemblyVersion => $"{Major}.0.0.0";
        public string FileVersion => $"{Major}.{Minor}.{Patch}.0";
        public string PackageVersion => Version;
        public string SemVersion => string.IsNullOrEmpty(Special) ? Version : $"{Version}-{Special}";
        public string SemVersionExtend => string.IsNullOrEmpty(Special) ? $"{Version}+{BuildCounter} " : $"{Version}-{Special}+{BuildCounter}";


        public string NugetSpecial =>
            string.IsNullOrEmpty(Special) ?
                Special :
                Special.Replace(".", "").Replace("-", "");

        public string NugetVersion => string.IsNullOrEmpty(NugetSpecial) ? Version : $"{Version}-{NugetSpecial}";

        public string FullBuildMetaData => $"BuildCounter.{BuildCounter}." +
                                           $"Branch.{GitBranch}." +
                                           $"DateTime.{DateTime:s}Z." +
                                           $"Env.{Env}." +
                                           $"Sha.{GitSha}." +
                                           $"CommitsCounter.{GitCommitNumber}";


        public string InformationalVersion => $"{SemVersion}+{FullBuildMetaData}";

    }
}