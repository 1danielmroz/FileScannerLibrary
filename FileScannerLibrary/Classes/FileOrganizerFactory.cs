using FileScannerLibrary.Intreface;

namespace FileScannerLibrary.Classes
{
    public static class FileOrganizerFactory
    {
        public static IFileOrganizer CreateFileOrganizer(bool compress)
        {
            if (compress)
            {
                return new CompressedFileOrganizer();
            }
            else
            {
                return new UncompressedFileOrganizer();
            }
        }
    }
}
