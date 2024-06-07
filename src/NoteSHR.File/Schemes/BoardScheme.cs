namespace NoteSHR.File.Schemes;

public class BoardScheme
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string? Name { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public IEnumerable<NoteScheme> Notes { get; set; }
}