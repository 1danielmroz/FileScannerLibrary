using FileScannerLibrary.Intreface;
using System.IO.Compression;

namespace FileScannerLibrary.Classes
{
    public class CompressedFileOrganizer : IFileOrganizer
    {
        public void OrganizeFiles(List<string> files, string destinationPath, string extension)
        {
            var compressedFolder = Path.Combine(destinationPath, "Compressed");

            if (!Directory.Exists(compressedFolder))
            {
                Directory.CreateDirectory(compressedFolder);
            }

            var zipPath = Path.Combine(compressedFolder, $"{extension.TrimStart('.')}.zip");

            using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
            Parallel.ForEach(files, file =>
            {
                var entryName = $"{Path.GetFileNameWithoutExtension(file)}{extension}";
                archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
            });
        }
    }
}
