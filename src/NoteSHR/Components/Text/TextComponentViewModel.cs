using NoteSHR.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Text;

public class TextComponentViewModel : ViewModelBase
{
    [Reactive] public string? Text { get; set; }
}