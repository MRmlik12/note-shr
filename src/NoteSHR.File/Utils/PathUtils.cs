namespace NoteSHR.File.Utils;

public static class PathUtils
{
    public static string GetTemporaryPath(Guid guid) 
        => Path.Combine(Path.GetTempPath(), $"NoteSHR/{guid}");
}