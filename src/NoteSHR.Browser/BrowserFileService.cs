using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using NoteSHR.Core.Services;

namespace NoteSHR.Browser;

public class BrowserFileService : IFileService
{
    public BrowserFileService()
    {
        Task.Run(async () => await JSHost.ImportAsync("FileUtils", "./fileUtils.js"));
    }

    public async Task<string?> GetFileUrl(string[] fileTypes)
    {
        var url = await FileUtilsEmbed.OpenFilePicker(fileTypes);

        return url;
    }
    
    public async Task SaveFile(string fileName, byte[] content)
    {
        await FileUtilsEmbed.SaveFile(fileName, content);
    }
}

internal partial class FileUtilsEmbed
{
    [JSImport("openFilePicker", "FileUtils")]
    public static partial Task<string?> OpenFilePicker(string[] fileTypes);
    
    [JSImport("saveFile", "FileUtils")]
    public static partial Task SaveFile(string fileName, byte[] contents); 
}