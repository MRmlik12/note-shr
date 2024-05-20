using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using NoteSHR.Core.Services;

namespace NoteSHR.Browser;

public class BrowserFilePicker : IFilePicker
{
    public BrowserFilePicker()
    {
        Task.Run(async () => await JSHost.ImportAsync("FilePicker", "./filePicker.js"));
    }

    public async Task<string?> GetFileUrl()
    {
        var url = await FilePickerEmbed.OpenFilePicker();

        return url;
    }
}

internal partial class FilePickerEmbed
{
    [JSImport("openFilePicker", "FilePicker")]
    public static partial Task<string?> OpenFilePicker();
}