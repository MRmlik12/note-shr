using System;
using System.IO;
using Avalonia.Media.Imaging;

namespace NoteSHR.Core.Helpers;

public static class ImageHelper
{
    public static Bitmap LoadFromFileSystem(Uri resource)
    {
        return new Bitmap(new FileStream(resource.LocalPath, FileMode.Open));
    }
}