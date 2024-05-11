using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using NoteSHR.Core.Models;

namespace NoteSHR.File;

public class BoardExporter
{
    private string GetTemporaryPath()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), $"NoteSHR/{Guid.NewGuid()}");
        Directory.CreateDirectory(tempFolder);
        
        return tempFolder;
    }
    
    public async Task<string> ExportToFile(List<Note> notes, string name, string path)
    {
        var boardScheme = BoardConverter.ConvertToScheme(name, notes);
        var json = JsonConvert.SerializeObject(boardScheme);

        var tempFolder = GetTemporaryPath();

        Path.Combine(tempFolder, "board.json");
        var boardFile = System.IO.File.Create(Path.Combine(tempFolder, "board.json"));
        await boardFile.WriteAsync(Encoding.UTF8.GetBytes(json));
        boardFile.Close();

        var destinationPath = $"{path}/{name}.zip";
        if (System.IO.File.Exists(destinationPath))
        {
            System.IO.File.Delete(destinationPath);
        }
        
        ZipFile.CreateFromDirectory(tempFolder, destinationPath);

        return destinationPath;
    }
}