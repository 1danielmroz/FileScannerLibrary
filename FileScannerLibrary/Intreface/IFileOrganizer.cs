namespace FileScannerLibrary.Intreface
{
    public interface IFileOrganizer
    {
        void OrganizeFiles(List<string> files, string destinationPath, string extension);
    }
}
