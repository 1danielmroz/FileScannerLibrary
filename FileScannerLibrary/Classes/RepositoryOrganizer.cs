namespace FileScannerLibrary.Classes
{
    public class RepositoryOrganizer
    {
        private static readonly string[] RepositoryIndicators = { ".git", ".svn", ".hg" };

        public static void OrganizeRepositories(string sourcePath, string destinationPath, bool move = true)
        {
            var repositories = FindRepositories(sourcePath);
            var repositoryFolder = Path.Combine(destinationPath, "Repositories");

            if (!Directory.Exists(repositoryFolder))
            {
                Directory.CreateDirectory(repositoryFolder);
            }

            foreach (var repo in repositories)
            {
                var repoName = Path.GetFileName(repo);
                var destinationRepoPath = Path.Combine(repositoryFolder, repoName);

                if (move)
                {
                    Directory.Move(repo, destinationRepoPath);
                }
                else
                {
                    CopyDirectory(repo, destinationRepoPath);
                }
            }
        }

        private static void CopyDirectory(string source, string destination)
        {
            var sourceDir = new DirectoryInfo(source);
            var destinationDir = new DirectoryInfo(destination);

            if (!destinationDir.Exists)
            {
                destinationDir.Create();
            }

            foreach (var file in sourceDir.GetFiles())
            {
                var destFile = Path.Combine(destinationDir.FullName, file.Name);
                file.CopyTo(destFile, true);
            }

            foreach (var subdir in sourceDir.GetDirectories())
            {
                var destDir = Path.Combine(destinationDir.FullName, subdir.Name);
                CopyDirectory(subdir.FullName, destDir);
            }
        }

        private static List<string> FindRepositories(string sourcePath)
        {
            var directories = Directory.GetDirectories(sourcePath, "*.*", SearchOption.AllDirectories);

            return directories.Where(dir => RepositoryIndicators.Any(indicator => dir.EndsWith(indicator,
                    StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }
}
