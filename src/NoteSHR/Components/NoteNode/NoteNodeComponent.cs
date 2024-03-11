using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using NoteSHR.ViewModels;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : UserControl
{
    public static readonly StyledProperty<List<(Type, ViewModelBase)>> NodesProperty = AvaloniaProperty.Register<NoteNodeComponent, List<(Type, ViewModelBase)>>(nameof(Nodes), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    
    public List<(Type, ViewModelBase)> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    private StackPanel _stackPanel;

    public NoteNodeComponent()
    {
        MinHeight = 200;
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
        foreach (var (type, vm) in Nodes)
        {
            _stackPanel.Children.Add((Control)Activator.CreateInstance(type, args: new [] { vm }));
        }
        
        Content = _stackPanel;
    }
}