using NoteSHR.File.Utils;

namespace NoteSHR.File;

public class Blob(Stream stream, string filename, Guid projectId) : IDisposable, IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await stream.DisposeAsync();
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public async Task Write()
    {
        await using var fileStream =
            System.IO.File.Create($"{PathUtils.GetTemporaryPath(projectId)}/assets/{filename}");
        await stream.CopyToAsync(fileStream);
    }

    // public async Task<Stream> Read()
    // {
    //     return System.IO.File.OpenRead(GetFilePath());
    // }

    private string GetFilePath()
    {
        return $"{PathUtils.GetTemporaryPath(projectId)}/assets/{filename}";
    }
}