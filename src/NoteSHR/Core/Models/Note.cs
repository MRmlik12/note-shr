using System;

namespace NoteSHR.Core.Models;

public class Note(Guid id, double x, double y)
{
    public Guid Id { get; set; } = id;
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
}
