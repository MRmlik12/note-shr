using System;

namespace NoteSHR.Core.Helpers;

public static class ColorHelper
{
    private static readonly string[] NoteColors = { "0F528C", "9ADE86", "FF6162", "FFC000", "47729A", "7D64AD" };
    
    public static string GenerateColor()
    {
        var random = new Random();
        var color = $"#{NoteColors[random.Next(0, NoteColors.Length - 1)]}";
        
        return color;
    }
}