using System;

namespace NoteSHR.Core.Helpers;

public static class ColorHelper
{
    private static readonly string[] NoteColors = { "0F528C", "9ADE86", "FFC000" };

    public static string GenerateColor()
    {
        var random = new Random();
        var color = $"#{NoteColors[random.Next(0, NoteColors.Length)]}";

        return color;
    }
}