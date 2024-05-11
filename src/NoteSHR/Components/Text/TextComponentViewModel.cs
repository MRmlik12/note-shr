using NoteSHR.Core.Models;
using NoteSHR.Core.ViewModel;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Text;

public class TextComponentViewModel : ViewModelBase, IDataPersistence
{
    [Reactive] public string? Text { get; set; }
    
    public object ExportValues()
    {
        return new
        {
            Text
        };
    }
}