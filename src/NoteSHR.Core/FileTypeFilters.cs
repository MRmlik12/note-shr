using Avalonia.Platform.Storage;

namespace NoteSHR.Core;

public class FileTypeFilters
{
    public static FilePickerFileType Board = new ("NoteSHR Board File")
    {
        Patterns = new[] { "*.shr" }
    };
}