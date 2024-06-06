using NoteSHR.File.Utils;

namespace NoteSHR.File;

public class FileBlob(Stream? stream = null) : IDisposable, IAsyncDisposable
{
    internal FileBlob(Guid projectId, string blobUri) : this()
    {
        ProjectId = projectId;
        Uri = new Uri(blobUri);
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

    public Stream Read()
    { 
        var filePath = GetFilePath(Uri.AbsolutePath);
        
        return System.IO.File.OpenRead(filePath);
    }

    private string GetFilePath(string filename)
    {
        return $"{PathUtils.GetTemporaryPath(ProjectId)}/assets/{filename}";
    }
}