using FileScannerLibrary.Classes;

namespace FileScannerLibrary.Intreface
{
    public interface IFileScanner
    {
        void ScanAndOrganizeFiles(string sourcePath, string destinationPath, bool compress = false);
        event EventHandler<FileProcessedEventArgs> FileProcessed;
        event EventHandler<FileCopiedEventArgs> FileCopied;
    }
}
