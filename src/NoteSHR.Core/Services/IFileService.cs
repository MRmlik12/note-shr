namespace NoteSHR.Core.Services;

public interface IFileService
{
    Task<string?> GetFileUrl();
    Task SaveFile(string fileName, byte[] content);
}