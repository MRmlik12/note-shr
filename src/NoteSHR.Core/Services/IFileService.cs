namespace NoteSHR.Core.Services;

public interface IFileService
{
    Task<string?> GetFileUrl(string[] fileTypes);
    Task SaveFile(string fileName, byte[] content);
}