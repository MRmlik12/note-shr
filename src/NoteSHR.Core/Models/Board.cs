namespace NoteSHR.Core.Models;

public class Board
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Note> Notes { get; set; }
}