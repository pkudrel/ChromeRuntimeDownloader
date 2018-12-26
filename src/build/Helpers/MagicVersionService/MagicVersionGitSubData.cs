namespace Helpers.MagicVersionService
{
    public class MagicVersionGitSubData
    {

        public string GitBranch { get; }
        public string GitSha { get; }
        public int GitCommitNumber { get; }

        public MagicVersionGitSubData(string gitSha, int gitCommitNumber, string gitBranch)
        {
            GitBranch = gitBranch;
            GitSha = gitSha;
            GitCommitNumber = gitCommitNumber;
        }
    }
}