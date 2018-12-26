using System;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities;

namespace Helpers.MagicVersionService
{

    [PublicAPI]
    public static class MagicVersionFactory
    {
        static DateTime DateTimeNowUtc = DateTime.UtcNow;
        static LocalRunner GitLocalRunner = new LocalRunner(GitTasks.GitPath);
        static readonly MagicVersionGitSubData _gitData = GetGitSubData();

        public static MagicVersion Make()
        {
            return Make(0, 0, 0, 0, MagicVersionStrategy.Standard, DateTime.UtcNow);
        }

        public static MagicVersion Make(int buildCounter)
        {
            return Make(0, 0, 0, buildCounter, MagicVersionStrategy.Standard, DateTime.UtcNow);
        }

        public static MagicVersion Make(int buildCounter, MagicVersionStrategy strategy)
        {
            return Make(0, 0, 0, buildCounter, strategy, DateTime.UtcNow);
        }

        public static MagicVersion Make(int buildCounter, MagicVersionStrategy strategy, DateTime utcNow)
        {
            return Make(0, 0, 0, buildCounter, strategy, DateTime.UtcNow);
        }

        public static MagicVersion Make(
            int major,
            int minor,
            int patch)
        {
            return Make(major, minor, patch, 0, MagicVersionStrategy.Standard, DateTime.UtcNow);
        }

        public static MagicVersion Make(
            int major,
            int minor,
            int patch,
            DateTime utcNow)
        {
            return Make(major, minor, patch, 0, MagicVersionStrategy.Standard, utcNow);
        }

        public static MagicVersion Make(
            int major,
            int minor,
            int patch,
            int buildCounter,
            MagicVersionStrategy strategy,
            DateTime utcNow, 
            string env = null)
        {
            var localStrategy = strategy ?? MagicVersionStrategy.Standard;


            switch (localStrategy.Name)
            {
                case nameof(MagicVersionStrategy.Standard):
                    var simple1 = GetForStandard(major, minor, patch, buildCounter, utcNow, env);
                    return new MagicVersion(_gitData, simple1);
                case nameof(MagicVersionStrategy.PatchFromCounter):
                    var simple2 = GetForPatchFromCounter(major, minor, patch, buildCounter, utcNow, env);
                    return new MagicVersion(_gitData, simple2);
                default:
                    return new MagicVersion(_gitData);
            }


           
        }

        static MagicVersionSimple GetForPatchFromCounter(int major,
            int minor,
            int patch,
            int buildCounter,
            DateTime dateUtc, 
            string env)
        {
            return new MagicVersionSimple(major, minor, buildCounter, string.Empty, buildCounter, dateUtc, env);
        }

        static MagicVersionSimple GetForStandard(int major, int minor, int patch, int buildCounter, DateTime dateUtc,
            string env)
        {
            return new MagicVersionSimple(major, minor, patch, string.Empty, buildCounter, dateUtc, env);
        }

        static int GetCommitNumber()
        {
            var path = NukeBuild.RootDirectory;
            var result = GitLocalRunner.Run($"rev-list --all --count", path, logOutput: false);
            var text = result.Select(x => x.Text).Take(1).Join(Environment.NewLine);
            try
            {
                var number = int.Parse(text);
                return number;

            }
            catch (Exception)
            {
            }

            return -1;
        }


        static string GetTimestamp()
        {
            var path = NukeBuild.RootDirectory;
            var result = GitLocalRunner.Run($"log --max-count=1 --pretty=format:%cI HEAD", path, logOutput: false);
            var text = result.Select(x => x.Text).Take(1).Join(Environment.NewLine);
            var date = DateTime.Parse(text).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            return date;
        }



        static string GetGitHash()
        {
            var path = NukeBuild.RootDirectory;
            var result = GitLocalRunner.Run($"log --max-count=1 --pretty=format:%H HEAD", path, logOutput: false);
            var hash = result.Select(x => x.Text).Take(1).Join(Environment.NewLine);
            return hash;
        }


        static MagicVersionGitSubData GetGitSubData()
        {

            var hash = GetGitHash();
            var commitNumber = GetCommitNumber();
            var branch = GetGitBranch();
            var ret = new MagicVersionGitSubData(hash, commitNumber, branch);
            return ret;
        }
        public static string GetGitBranch()
        {
            var path = NukeBuild.RootDirectory;
            var result1 = GitLocalRunner.Run($"rev-parse --abbrev-ref HEAD", path, logOutput: false);

            var result2 = result1.Select(x => x.Text).Take(1).Join(Environment.NewLine);
            if (result2.IndexOf("HEAD", StringComparison.OrdinalIgnoreCase) == -1) return result2;

            var result3 = GitTasks.Git($"symbolic-ref --short -q HEAD", path, logOutput: false);
            var result4 = result3.Select(x => x.Text).Take(1).Join(Environment.NewLine);
            if (result4.IndexOf("HEAD", StringComparison.OrdinalIgnoreCase) == -1) return result2;

            return string.Empty;
        }

    }
}