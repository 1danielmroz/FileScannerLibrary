using FileScannerLibrary.Intreface;
using System.Security.Cryptography;

namespace FileScannerLibrary.Classes
{
    public class FileScanner : IFileScanner
    {
        private static FileScanner _instance;
        private Dictionary<string, List<string>> _fileHashes;

        public event EventHandler<FileProcessedEventArgs> FileProcessed;
        public event EventHandler<FileCopiedEventArgs> FileCopied;

        // Singleton pattern: Ensures only one instance of FileScanner exists
        private FileScanner()
        {
            _fileHashes = new Dictionary<string, List<string>>();
        }

        public static FileScanner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileScanner();
                }

                return _instance;
            }
        }

        public void ScanAndOrganizeFiles(string sourcePath, string destinationPath, bool compress = false)
        {
            // Scan for files
            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var hash = CalculateFileHash(file);

                if (!_fileHashes.ContainsKey(hash))
                {
                    _fileHashes[hash] = new List<string>();
                }

                _fileHashes[hash].Add(file);
                // Raise FileProcessed event
                OnFileProcessed(new FileProcessedEventArgs { FilePath = file });
            }

            // Organize files in destination
            OrganizeFiles(destinationPath, compress);
            DuplicateFileReport.GenerateReport(_fileHashes, destinationPath);
        }

        protected virtual void OnFileProcessed(FileProcessedEventArgs e) => FileProcessed?.Invoke(this, e);

        protected virtual void OnFileCopied(FileCopiedEventArgs e) => FileCopied?.Invoke(this, e);

        private string CalculateFileHash(string filePath)
        {
            // Calculate file hash (unique identifier)
            using var stream = new BufferedStream(File.OpenRead(filePath), 1200000);
            var sha = new SHA256Managed();
            var hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private void OrganizeFiles(string destinationPath, bool compress)
        {
            // Factory Method pattern: Create an IFileOrganizer based on the compress flag
            var fileOrganizer = FileOrganizerFactory.CreateFileOrganizer(compress);

            foreach (var fileGroup in _fileHashes)
            { 
                var firstFile = fileGroup.Value.First();
                var extension = Path.GetExtension(firstFile);
                fileOrganizer.OrganizeFiles(fileGroup.Value, destinationPath, extension);
                foreach (var file in fileGroup.Value)
                {
                    OnFileCopied(new FileCopiedEventArgs { FilePath = file });
                } 
            }
        }
    }
}