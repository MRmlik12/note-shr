using System.Collections.Generic;
using System.Reactive;
using Avalonia.Input;
using NoteSHR.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.List;

public class ListComponentViewModel : ViewModelBase
{
    public ListComponentViewModel()
    {
        AddRowCommand = ReactiveCommand.Create((KeyEventArgs args) =>
        {
            if (args.Key != Key.Enter) return;

            Rows = [..Rows, new ListItem(string.Empty)];
        });
    }

    [Reactive] public IEnumerable<ListItem> Rows { get; set; } = new List<ListItem> { new(string.Empty) };

    public ReactiveCommand<KeyEventArgs, Unit> AddRowCommand { get; set; }
}