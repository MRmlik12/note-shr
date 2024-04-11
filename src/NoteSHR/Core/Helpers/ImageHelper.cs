using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace NoteSHR.Core.Helpers;

public static class ImageHelper
{
    public static async Task<Bitmap> LoadFromFileSystem(IStorageFile resource)
    {
        var stream = await resource.OpenReadAsync();

        return new Bitmap(stream);
    }
}