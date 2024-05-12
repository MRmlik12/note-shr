namespace NoteSHR.File.Schemes;

public class NodeScheme
{
    public Guid Id { get; set; }
    public string Assembly { get; set; }
    public string Component { get; set; }
    public string ViewModelType { get; set; }
    public object Data { get; set; }
}