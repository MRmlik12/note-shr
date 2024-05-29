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

    public async Task<string?> GetFileUrl()
    {
        var url = await FileUtilsEmbed.OpenFilePicker();

        return url;
    }
    
    public async Task SaveFile(string fileName, string content)
    {
        await FileUtilsEmbed.SaveFile(fileName, content);
    }
}

internal partial class FileUtilsEmbed
{
    [JSImport("openFilePicker", "FileUtils")]
    public static partial Task<string?> OpenFilePicker();
    
    [JSImport("saveFile", "FileUtils")]
    public static partial Task SaveFile(string fileName, string contents); 
}