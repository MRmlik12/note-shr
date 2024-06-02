using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Input;
using Newtonsoft.Json.Linq;
using NoteSHR.Core.Models;
using NoteSHR.Core.ViewModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.List;

public class ListComponentViewModel : ViewModelBase, IDataPersistence
{
    public ListComponentViewModel()
    {
        AddRowCommand = ReactiveCommand.Create((KeyEventArgs args) =>
        {
            if (args.Key != Key.Enter) return;

            Rows = [..Rows, new ListItem(Rows.Count(), string.Empty, PrefixType)];
        });

        ChangePrefixTypeCommand = ReactiveCommand.Create((string type) =>
        {
            // TODO: Implement prefix logic better
            if (!Enum.TryParse<PrefixType>(type, out var parsedType)) return;

            PrefixType = parsedType;
            Rows = Rows.Select((row, index) => new ListItem(index, row.Text, PrefixType)).ToList();
        });
    }

    [Reactive] public IEnumerable<ListItem> Rows { get; set; } = new List<ListItem> { new(0, string.Empty, default) };
    [Reactive] public PrefixType PrefixType { get; set; } = PrefixType.Bullet;

    public ReactiveCommand<KeyEventArgs, Unit> AddRowCommand { get; set; }
    public ReactiveCommand<string, Unit> ChangePrefixTypeCommand { get; set; }

    public object ExportValues()
    {
        return new
        {
            Rows,
            PrefixType
        };
    }

    public void ConvertValues(Dictionary<string, object> data)
    {
        if (data.TryGetValue(nameof(PrefixType), out var prefixType))
        {
            PrefixType = Enum.Parse<PrefixType>(prefixType.ToString());
        }
        
        foreach (var key in data.Keys.ToList())
        {
            if (data[key] is JArray jArray && key == nameof(Rows))
            {
                Rows = jArray.ToObject<List<ListItem>>();
            }
        } 
    }
}