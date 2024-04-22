using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using NoteSHR.Core.Services;

namespace NoteSHR.Browser;

public class BrowserFilePicker : IFilePicker
{
    public async Task Open()
    {
        await JSHost.ImportAsync("FilePickerEmbed", "filePicker.js");
        FilePickerEmbed.GetFile(); 
    }
}

internal partial class FilePickerEmbed
{
    [JSImport("openFilePicker", "filePicker.js")]
    public static partial void GetFile();
}