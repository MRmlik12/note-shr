using System.Collections.Generic;
using System.Reactive;
using Avalonia.Input;
using NoteSHR.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.List;

public class ListComponentViewModel : ViewModelBase
{
    [Reactive]
    public List<string> Rows { get; set; } = new () { string.Empty };
    
    public ReactiveCommand<KeyEventArgs, Unit> AddRowCommand { get; set; }
    
    public ListComponentViewModel()
    {
        AddRowCommand = ReactiveCommand.Create((KeyEventArgs args) =>
        {
            if (args.Key != Key.Enter)
            {
                return;
            } 
            
            Rows = [..Rows, ""];
        });
    }
}