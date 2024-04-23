using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoteSHR.Core.Services;

namespace NoteSHR.Browser;

public class BrowserFilePicker : IFilePicker
{
    
    public BrowserFilePicker()
    {
        Task.Run(async () => await JSHost.ImportAsync("FilePicker", "./filePicker.js"));
    }
    
    public string? GetFileUrl()
    {
        var url = FilePickerEmbed.OpenFilePicker();
        
        return url!;
    }
}

internal partial class FilePickerEmbed
{
    [JSImport("openFilePicker", "FilePicker")]
    public static partial string? OpenFilePicker();
}