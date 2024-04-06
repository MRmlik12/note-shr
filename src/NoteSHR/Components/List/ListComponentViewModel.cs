using System;
using System.Collections.Generic;
using System.Linq;
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

            Rows = [..Rows, new ListItem(Rows.Count(), string.Empty)];
        });

        ChangePrefixTypeCommand = ReactiveCommand.Create((string type) =>
        {
            if (Enum.TryParse<PrefixType>(type, out var parsedType))
            {
                PrefixType = parsedType;
            }
        });
    }

    [Reactive] public IEnumerable<ListItem> Rows { get; set; } = new List<ListItem> { new(0, string.Empty) };
    [Reactive] public PrefixType PrefixType { get; set; } = PrefixType.Bullet;
    
    public ReactiveCommand<KeyEventArgs, Unit> AddRowCommand { get; set; }
    public ReactiveCommand<string, Unit> ChangePrefixTypeCommand { get; set; }
}