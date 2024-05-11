namespace NoteSHR.File.Models;

public class NoteScheme
{
    public Guid Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public string HeaderColor { get; set; }
    public string BackgroundColor { get; set; }
    public IEnumerable<NodeScheme> Nodes { get; set; }
}