using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : UserControl
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
        _stackPanel = new StackPanel
        {
            Name = "Node",
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        
        Content = _stackPanel;
    }

    protected override void OnInitialized()
    {
        _stackPanel.Children.AddRange(Nodes.Select(x => (Control)x).ToList());
        Content = _stackPanel;
    }
}