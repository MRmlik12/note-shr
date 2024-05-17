using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using NoteSHR.Core.Models;
using NoteSHR.File.Utils;

namespace NoteSHR.File;

public static class BoardExporter
{
    public static async Task<string> ExportToFile(List<Note> notes, string name, string path)
    {
        var boardScheme = BoardConverter.ConvertToScheme(name, notes);
        var json = JsonConvert.SerializeObject(boardScheme);

        var tempFolder = PathUtils.GetTemporaryPath(boardScheme.Id);
        Directory.CreateDirectory(tempFolder);

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