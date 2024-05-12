using System.IO.Compression;
using Newtonsoft.Json;
using NoteSHR.Core.Exceptions;
using NoteSHR.Core.Models;
using NoteSHR.File.Schemes;

namespace NoteSHR.File;

public static class BoardImporter
{
    public static async Task<Board> ImportFromFile(string path)
    {
        var zipFile = ZipFile.Open(path, ZipArchiveMode.Read);
        
        var schemeFile = zipFile.Entries.First(e => e.Name == "board.json");
        await using var schemeStream = schemeFile.Open();
        var jsonContent = await new StreamReader(schemeStream).ReadToEndAsync();
        var scheme = JsonConvert.DeserializeObject<BoardScheme>(jsonContent);
        
        if (scheme is null)
        {
            throw new CorruptedBoardFileException("Cannot read board contents from file");
        }
        
        var board = BoardConverter.ConvertBack(scheme);

        return board;
    }
}