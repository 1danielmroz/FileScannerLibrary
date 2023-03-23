using FileScannerLibrary.Intreface;

namespace FileScannerLibrary.Classes
{
    public class UncompressedFileOrganizer : IFileOrganizer
    {
        public void OrganizeFiles(List<string> files, string destinationPath, string extension)
        {
            var extensionFolder = Path.Combine(destinationPath, extension.TrimStart('.'));

            if (!Directory.Exists(extensionFolder))
            {
                Directory.CreateDirectory(extensionFolder);
            }

            Parallel.ForEach(files, file =>
            { 
                var destinationFile = Path.Combine(extensionFolder, Path.GetFileName(file));
                File.Copy(file, destinationFile, true); 
            });

        }
    }
}
