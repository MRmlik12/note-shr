using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using NoteSHR.Core.Models;
using NoteSHR.File.Utils;

namespace NoteSHR.File;

public static class BoardExporter
{
    public static async Task<string> ExportToFile(Guid id, List<Note> notes, string name, string path)
    {
        var tempFolder = PathUtils.GetTemporaryPath(id);
        if (Directory.Exists(tempFolder))
        {
            Directory.Delete(tempFolder, true); 
        }
        
        Directory.CreateDirectory(tempFolder);
        var boardScheme = BoardConverter.ConvertToScheme(id, name, notes);
        var json = JsonConvert.SerializeObject(boardScheme);

        var boardFile = System.IO.File.Create(Path.Combine(tempFolder, "board.json"));
        await boardFile.WriteAsync(Encoding.UTF8.GetBytes(json));
        boardFile.Close();

        var destinationPath = $"{path}/{name}.zip";
        if (System.IO.File.Exists(destinationPath)) System.IO.File.Delete(destinationPath);

        ZipFile.CreateFromDirectory(tempFolder, destinationPath);

        return destinationPath;
    }
}