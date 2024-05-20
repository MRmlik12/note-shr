namespace NoteSHR.File.Utils;

public static class PathUtils
{
    public static string GetTemporaryPath(Guid guid)
    {
        return Path.Combine(Path.GetTempPath(), $"NoteSHR/{guid}");
    }
}