using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : ContentControl
{
    public static readonly StyledProperty<List<INode?>> NodesProperty = AvaloniaProperty.Register<NoteNodeComponent, List<INode?>>(nameof(Nodes), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    
    public List<INode?> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    private StackPanel _stackPanel;
    
    public NoteNodeComponent()
    {
        Height = 100;
        _stackPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Height = 100
        };
        
        Content = _stackPanel;
    }
    
    protected override void OnInitialized()
    {
        // var stackPanel = new StackPanel
        // {
        //     Orientation = Orientation.Vertical,
        //     HorizontalAlignment = HorizontalAlignment.Stretch,
        //     VerticalAlignment = VerticalAlignment.Stretch
        // };
        //
        // stackPanel.Children.AddRange(Nodes.Select(x => x.GetXamlCode()).ToList());
        // Content = stackPanel;
        _stackPanel.Children.AddRange(Nodes.Select(x => (Control)x).ToList());
        Content = _stackPanel;
    }
}