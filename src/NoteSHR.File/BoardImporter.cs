using System.IO.Compression;
using Newtonsoft.Json;
using NoteSHR.Core.Exceptions;
using NoteSHR.Core.Models;
using NoteSHR.Core.Utils;
using NoteSHR.File.Schemes;
using NoteSHR.File.Utils;

namespace NoteSHR.File;

public static class BoardImporter
{
    public static async Task<Board> ImportFromFile(string path)
    {
        ZipArchive zipFile;
        if (OperatingSystem.IsBrowser())
        {
            var stream = await HttpUtils.GetStreamFromUrl(path);
            zipFile = new ZipArchive(stream);
        }
        else
        {
            zipFile = ZipFile.Open(path, ZipArchiveMode.Read);
        }

        var schemeFile = zipFile.Entries.First(e => e.Name == "board.json");
        await using var schemeStream = schemeFile.Open();
        var jsonContent = await new StreamReader(schemeStream).ReadToEndAsync();
        var scheme = JsonConvert.DeserializeObject<BoardScheme>(jsonContent);

        if (scheme is null) throw new CorruptedBoardFileException("Cannot read board contents from file");

        zipFile.ExtractToDirectory(PathUtils.GetTemporaryPath(scheme.Id)); 

        var board = BoardConverter.ConvertBack(scheme);

        return board;
    }
}