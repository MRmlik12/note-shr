using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Text;

public class TextComponentViewModel
{
    [Reactive]
    public string? Text { get; set; }
}