using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace NoteSHR.Core.Helpers;

public static class ImageHelper
{
    public static Bitmap LoadFromFileSystem(Uri resource)
    {
        return new Bitmap(new FileStream(resource.LocalPath, FileMode.Open));
    }
}