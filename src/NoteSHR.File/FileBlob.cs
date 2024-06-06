using System.IO.Compression;
using NoteSHR.File.Utils;

namespace NoteSHR.File;

public class FileBlob(Stream? stream = null) : IDisposable, IAsyncDisposable
{
    private readonly ZipArchive _zipArchive;
    
    internal FileBlob(Guid projectId, string blobUri, ZipArchive zipArchive) : this()
    {
        ProjectId = projectId;
        Uri = new Uri(blobUri);
        
        _zipArchive = zipArchive;
    }

    private Guid ProjectId { get; set; }
    private Uri Uri { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (stream != null) await stream.DisposeAsync();
    }

    public void Dispose()
    {
        stream?.Dispose();
    }

    internal void SetProjectId(Guid projectId)
    {
        ProjectId = projectId;
    }

    public Stream Read()
    { 
        var filePath = GetFilePath(Uri.AbsolutePath);
        var entry = _zipArchive.GetEntry(filePath);

        if (entry == null)
        {
            throw new FileNotFoundException("File not found in archive");
        }
        
        var deflateStream = entry.Open();
        
        var memoryStream = new MemoryStream();
        
        deflateStream.CopyTo(memoryStream);
        deflateStream.Close();

        memoryStream.Position = 0;
        
        return memoryStream;
    }

    internal string Write(string filename)
    {
        var tempPath = PathUtils.GetTemporaryPath(ProjectId);
        if (!Directory.Exists(Path.Combine(tempPath, "assets")))
        {
            Directory.CreateDirectory(Path.Combine(tempPath, "assets"));
        }
        
        var internalPath = $"assets/{filename}"; 
        using var fileStream = new FileStream($"{tempPath}/{internalPath}", FileMode.Create, FileAccess.Write);
        stream!.Position = 0;
        stream!.CopyTo(fileStream);
        
        Uri = new Uri($"blob://{internalPath}");

        return Uri.ToString();
    }

    private static string GetFilePath(string filename)
    {
        return $"assets{filename}";
    }
}