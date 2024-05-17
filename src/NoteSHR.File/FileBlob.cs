﻿using NoteSHR.File.Utils;

namespace NoteSHR.File;

public class FileBlob(Stream? stream = null) : IDisposable, IAsyncDisposable
{
    private Guid ProjectId { get; set; }
    private Uri Uri { get; }
    
    internal void SetProjectId(Guid projectId)
    {
        ProjectId = projectId;
    }

    internal async Task<string> Write(string filename)
    {
        var internalPath = $"/assets/{filename}"; 
        await using var fileStream = System.IO.File.Create($"{PathUtils.GetTemporaryPath(ProjectId)}/{internalPath}");
        await stream!.CopyToAsync(fileStream);

        return $"blob://{internalPath}";
    }

    public Stream Read()
    {
        return System.IO.File.OpenRead(GetFilePath(Uri.AbsolutePath));
    }

    private string GetFilePath(string filename)
    {
        return $"{PathUtils.GetTemporaryPath(ProjectId)}/assets/{filename}";
    }

    public void Dispose()
    {
        stream?.Dispose();
    }
    
    internal FileBlob(Guid projectId, string blobUri) : this()
    {
        ProjectId = projectId;
        Uri = new Uri(blobUri);
    }

    public async ValueTask DisposeAsync()
    {
        if (stream != null)
        {
            await stream.DisposeAsync();
        }
    }
}