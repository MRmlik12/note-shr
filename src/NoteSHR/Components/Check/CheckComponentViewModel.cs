using NoteSHR.Core.ViewModel;
using NoteSHR.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Check;

public class CheckComponentViewModel : ViewModelBase
{
    [Reactive] public bool Checked { get; set; }
    [Reactive] public string? Text { get; set; }
}