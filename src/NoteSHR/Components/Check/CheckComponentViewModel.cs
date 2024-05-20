using NoteSHR.Core.Models;
using NoteSHR.Core.ViewModel;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Check;

public class CheckComponentViewModel : ViewModelBase, IDataPersistence
{
    [Reactive] public bool Checked { get; set; }
    [Reactive] public string? Text { get; set; }

    public object ExportValues()
    {
        return new
        {
            Checked, Text
        };
    }
}