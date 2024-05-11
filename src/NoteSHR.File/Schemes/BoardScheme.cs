namespace NoteSHR.File.Models;

public class BoardScheme
{
    public string? Name { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public IEnumerable<NoteScheme> Notes { get; set; }
}