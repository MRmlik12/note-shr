using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.ViewModels;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : UserControl
{
    public static readonly StyledProperty<List<(Guid, Type, ViewModelBase)>> NodesProperty = AvaloniaProperty.Register<NoteNodeComponent, List<(Guid, Type, ViewModelBase)>>(nameof(Nodes), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    public static readonly StyledProperty<Guid> NoteIdProperty = AvaloniaProperty.Register<NoteNodeComponent, Guid>(nameof(NoteId), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
    public static readonly StyledProperty<bool> DeleteModeProperty = AvaloniaProperty.Register<NoteNodeComponent, bool>(nameof(DeleteMode), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
    public static readonly StyledProperty<bool> EditModeProperty = AvaloniaProperty.Register<NoteNodeComponent, bool>(nameof(EditMode), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
    
    public static readonly RoutedEvent<DeleteNodeEventArgs> DeleteNodeEvent = RoutedEvent.Register<NoteNodeComponent, DeleteNodeEventArgs>(nameof(DeleteNode), RoutingStrategies.Direct);
    public static readonly RoutedEvent<MoveNodeEventArgs> MoveNodeEvent = RoutedEvent.Register<NoteNodeComponent, MoveNodeEventArgs>(nameof(MoveNodeEvent), RoutingStrategies.Direct);

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

    private bool EditMode
    {
        get => GetValue(EditModeProperty);
        set => SetValue(EditModeProperty, value);
    }
    
    public event EventHandler<DeleteNodeEventArgs> DeleteNode
    {
        add => AddHandler(DeleteNodeEvent, value);
        remove => RemoveHandler(DeleteNodeEvent, value);
    }
    
    public event EventHandler<MoveNodeEventArgs> MoveNode
    {
        add => AddHandler(MoveNodeEvent, value);
        remove => RemoveHandler(MoveNodeEvent, value);
    }

    private StackPanel _stackPanel;
    private Guid? _nodeToMoveId;

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
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = id,
                ColumnDefinitions = new ()
                {
                    new (20.0, GridUnitType.Auto),
                    new (GridLength.Star)
                },
                RowDefinitions = new ()
                {
                    new (GridLength.Auto)
                }
            };
            
            if (DeleteMode)
            {
                var deleteButton = new Button()
                {
                    Content = "D",
                    DataContext = (id, type, vm)
                };
                
                Grid.SetColumn(deleteButton, 0);
                
                deleteButton.Click += (sender, args) => RaiseEvent(new DeleteNodeEventArgs(DeleteNodeEvent, NoteId, id));
                grid.Children.Add(deleteButton);
            }
            
            if (EditMode)
            {
                var editTextBlock = new TextBlock
                {
                    Text = "M",
                    Foreground = Brushes.DeepPink,
                    DataContext = id
                };

                Grid.SetColumn(editTextBlock, 0);
                
                grid.Children.Add(editTextBlock);
            }

            var node = (Control)Activator.CreateInstance(type, args: new[] { vm });
            node.HorizontalAlignment = HorizontalAlignment.Stretch;
            node.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(node, 1);
            
            grid.Children.Add(node);
            _stackPanel.Children.Add(grid);
        }
        
        _stackPanel.PointerPressed += (sender, args) =>
        {
            if (!EditMode)
            {
                return;
            }
                    
            _nodeToMoveId = (Guid)(args.Source as Control)?.DataContext;
            _stackPanel.PointerMoved += StackPanelOnPointerMoved;
            Console.WriteLine(args.GetPosition(null).X + " " + args.GetPosition(null).Y);
        };
        
        _stackPanel.PointerReleased += (sender, args) =>
        {
            PointerMoved -= StackPanelOnPointerMoved;
            if (_nodeToMoveId == null)
            {
                return;
            }

            var sourceNode = Nodes.SingleOrDefault(x => x.Item1 == (Guid)(args.Source as Control).DataContext);
            RaiseEvent(new MoveNodeEventArgs(MoveNodeEvent, NoteId, sourceNode.Item1, _nodeToMoveId.Value));
        };
        
        Content = _stackPanel;
    }

    private void StackPanelOnPointerMoved(object? sender, PointerEventArgs e)
    {
        var control = e.Source;
        if (control is TextBlock source)
        {
            _nodeToMoveId = (Guid)source.DataContext;
            Console.WriteLine(_nodeToMoveId);
        }
    }
}