using Avalonia.Media.Imaging;

namespace NoteSHR.Core.Utils;

public class HttpUtils
{
    public static async Task<Bitmap> GetBitmapFromUrl(string url)
    {
        return new Bitmap(await GetStreamFromUrl(url));
    }
    
    public static async Task<Stream> GetStreamFromUrl(string url)
    {
        using var client = new HttpClient();

        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStreamAsync();
    }
}