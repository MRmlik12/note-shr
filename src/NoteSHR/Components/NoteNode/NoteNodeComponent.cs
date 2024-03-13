using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.ViewModels;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : UserControl
{
    public static readonly StyledProperty<List<(Guid, Type, ViewModelBase)>> NodesProperty = AvaloniaProperty.Register<NoteNodeComponent, List<(Guid, Type, ViewModelBase)>>(nameof(Nodes), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    public static readonly StyledProperty<bool> DeleteModeProperty = AvaloniaProperty.Register<NoteNodeComponent, bool>(nameof(DeleteMode), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
    public static readonly StyledProperty<Guid> NoteIdProperty = AvaloniaProperty.Register<NoteNodeComponent, Guid>(nameof(NoteId), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
    
    public static readonly RoutedEvent<DeleteNodeEventArgs> DeleteNodeEvent = RoutedEvent.Register<NoteNodeComponent, DeleteNodeEventArgs>(nameof(DeleteNode), RoutingStrategies.Direct);
    
    private List<(Guid, Type, ViewModelBase)> Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }
    
    private Guid NoteId
    {
        get => GetValue(NoteIdProperty);
        set => SetValue(NoteIdProperty, value);
    }

    private bool DeleteMode
    {
        get => GetValue(DeleteModeProperty);
        set => SetValue(DeleteModeProperty, value);
    }
    
    public event EventHandler<DeleteNodeEventArgs> DeleteNode
    {
        add => AddHandler(DeleteNodeEvent, value);
        remove => RemoveHandler(DeleteNodeEvent, value);
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
        foreach (var (id, type, vm) in Nodes)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            
            if (DeleteMode)
            {
                var deleteButton = new Button()
                {
                    Content = "D",
                    DataContext = (id, type, vm)
                };
                
                deleteButton.Click += (sender, args) => RaiseEvent(new DeleteNodeEventArgs(DeleteNodeEvent, NoteId, id));
                stackPanel.Children.Add(deleteButton);
            }

            var node = (Control)Activator.CreateInstance(type, args: new[] { vm });
            
            node.VerticalAlignment = VerticalAlignment.Stretch;
            node.HorizontalAlignment = HorizontalAlignment.Stretch;
            
            stackPanel.Children.Add(node);
            _stackPanel.Children.Add(stackPanel);
        }
        
        Content = _stackPanel;
    }
}