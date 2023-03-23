using Newtonsoft.Json;

namespace FileScannerLibrary.Classes
{
    public class DuplicateFileReport
    {
        public static void GenerateReport(Dictionary<string, List<string>> duplicates, string destinationPath)
        {
            var duplicatesReport = duplicates
                .Where(entry => entry.Value.Count > 1)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            var json = JsonConvert.SerializeObject(duplicatesReport, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(destinationPath, "duplicatesReport.json"), json);
        }
    }
}
