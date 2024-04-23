using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace NoteSHR.Core.Helpers;

public static class HttpHelper
{
    public static async Task<Bitmap> GetBitmatFromUrl(string url)
    {
        using var client = new HttpClient();
       
        var response = await client.GetAsync(url);
        await using var stream = await response.Content.ReadAsStreamAsync();
        
        return new Bitmap(stream);
    }
}