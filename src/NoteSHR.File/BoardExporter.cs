using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using NoteSHR.Core.Models;
using NoteSHR.File.Utils;

namespace NoteSHR.File;

public static class BoardExporter
{
    private static string GetTemporaryPath()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), $"NoteSHR/{Guid.NewGuid()}");
        Directory.CreateDirectory(tempFolder);
        
        return tempFolder;
    }
    
    public static async Task<string> ExportToFile(List<Note> notes, string name, string path)
    {
        var boardScheme = await BoardConverter.ConvertToScheme(name, notes);
        var json = JsonConvert.SerializeObject(boardScheme);

        var tempFolder = GetTemporaryPath();

        var boardFile = System.IO.File.Create(PathUtils.GetTemporaryPath(boardScheme.Id));
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