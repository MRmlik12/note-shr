using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.ViewModels;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : UserControl
{
    private const string DeleteButtonName = "DeleteButton";
    private const string EditButtonName = "EditModeButton";
    private const string MoveUpButtonName = "MoveUpButton";
    private const string MoveDownButtonName = "MoveDownButton";

    public static readonly StyledProperty<ObservableCollection<NodeViewModel>> NodesProperty =
        AvaloniaProperty.Register<NoteNodeComponent, ObservableCollection<NodeViewModel>>(nameof(Nodes),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Guid> NoteIdProperty =
        AvaloniaProperty.Register<NoteNodeComponent, Guid>(nameof(NoteId), defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<bool> DeleteModeProperty =
        AvaloniaProperty.Register<NoteNodeComponent, bool>(nameof(DeleteMode), defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<bool> EditModeProperty =
        AvaloniaProperty.Register<NoteNodeComponent, bool>(nameof(EditMode), defaultBindingMode: BindingMode.OneWay);

    public static readonly RoutedEvent<DeleteNodeEventArgs> DeleteNodeEvent =
        RoutedEvent.Register<NoteNodeComponent, DeleteNodeEventArgs>(nameof(DeleteNode), RoutingStrategies.Direct);

    public static readonly RoutedEvent<MoveNodeEventArgs> MoveNodeEvent =
        RoutedEvent.Register<NoteNodeComponent, MoveNodeEventArgs>(nameof(MoveNodeEvent), RoutingStrategies.Direct);

    private readonly StackPanel _stackPanel;
    private readonly List<Grid> _nodeGrids = new();

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

    private ObservableCollection<NodeViewModel> Nodes
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property.Name == nameof(DeleteMode))
        {
            var deleteMode = (bool)change.NewValue;
            _nodeGrids.ForEach(x =>
            {
                if (deleteMode)
                {
                    var deleteButton = GetDeleteButtonControl((Guid)x.DataContext); 

                    x.Children.Add(deleteButton);
                }
                else
                {
                    x.Children.Remove(x.Children.FirstOrDefault(x => x.Name == DeleteButtonName));
                }
            });
        }
        else if (change.Property.Name == nameof(EditMode))
        {
            _nodeGrids.ForEach(x =>
            {
                if (EditMode)
                {
                    var editModeGrid = GetEditModeControl((Guid)x.DataContext);
                    x.Children.Add(editModeGrid);
                }
                else
                {
                    x.Children.Remove(x.Children.FirstOrDefault(x => x.Name == EditButtonName));
                }
            });
        }

        base.OnPropertyChanged(change);
    }

    protected override void OnInitialized()
    {
        Nodes.CollectionChanged += NodesOnCollectionChanged;

        Content = _stackPanel;
    }

    private void NodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Move)
        {
            _stackPanel.Children.RemoveAll(_nodeGrids);
            _nodeGrids.Clear();
            foreach (var nodeVm in Nodes)
            {
                var grid = InitializeNodeGrid(nodeVm); 
                _stackPanel.Children.Add(grid);
            }

            return;
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var nodeVm in e.OldItems.Cast<NodeViewModel>())
            {
                var grid = _nodeGrids.Single(x => (Guid)x.DataContext == nodeVm.Id);
                _nodeGrids.Remove(grid);
                _stackPanel.Children.Remove(grid);
            }
        }

        if (!(e.NewItems?.Count > 0)) return;

        foreach (var nodeVm in e.NewItems.Cast<NodeViewModel>())
        {
            var grid = InitializeNodeGrid(nodeVm);
            _stackPanel.Children.Add(grid);
        }
    }

    private Grid InitializeNodeGrid(NodeViewModel nodeViewModel)
    {
        var grid = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            DataContext = nodeViewModel.Id,
            ColumnDefinitions = new ColumnDefinitions
            {
                new(20.0, GridUnitType.Auto),
                new(GridLength.Star)
            },
            RowDefinitions = new RowDefinitions
            {
                new(GridLength.Auto)
            }
        };

        if (DeleteMode)
        {
            var deleteButton = GetDeleteButtonControl(nodeViewModel.Id);
            grid.Children.Add(deleteButton);
        }

        var node = (Control)Activator.CreateInstance(nodeViewModel.Type, nodeViewModel.ViewModel);
        if (EditMode)
        {
            var editModeGrid = GetEditModeControl(nodeViewModel.Id);

            grid.Children.Add(editModeGrid);
        }

        node.HorizontalAlignment = HorizontalAlignment.Stretch;
        node.VerticalAlignment = VerticalAlignment.Stretch;
        Grid.SetColumn(node, 1);

        _nodeGrids.Add(grid);
        grid.Children.Add(node);
        return grid;
    }

    private Grid GetEditModeControl(Guid id)
    {
        var editModeGrid = new Grid
        {
            Name = EditButtonName,
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Star)
            },
            RowDefinitions = new RowDefinitions
            {
                new(GridLength.Auto),
                new(GridLength.Auto)
            }
        };

        var moveUpButton = new TextBlock
        {
            Name = MoveUpButtonName,
            Text = "\u25b2",
            DataContext = id
        };

        var moveDownButton = new TextBlock
        {
            Name = MoveDownButtonName,
            Text = "\u25bc",
            DataContext = id 
        };

        moveUpButton.PointerPressed += EditModeButtonClicked;
        moveDownButton.PointerPressed += EditModeButtonClicked;

        Grid.SetRow(moveUpButton, 0);
        Grid.SetColumn(moveUpButton, 0);
        Grid.SetRow(moveDownButton, 1);
        Grid.SetColumn(moveDownButton, 0);

        Grid.SetColumn(editModeGrid, 0);

        editModeGrid.Children.Add(moveUpButton);
        editModeGrid.Children.Add(moveDownButton);
        
        return editModeGrid;
    }

    private Button GetDeleteButtonControl(Guid id) 
    {
        var deleteButton = new Button
        {
            Name = DeleteButtonName,
            Content = "D",
            DataContext = id 
        };
        Grid.SetColumn(deleteButton, 0);

        deleteButton.Click += (_, _) =>
            RaiseEvent(new DeleteNodeEventArgs(DeleteNodeEvent, NoteId, id));
        
        return deleteButton;
    }

    private void EditModeButtonClicked(object? sender, PointerPressedEventArgs e)
    {
        var control = e.Source as TextBlock;
        var sourceNode = Nodes.SingleOrDefault(x => x.Id == (Guid)control.DataContext);
        var moveOptions = control?.Name == MoveUpButtonName ? NodeMoveOptions.Up : NodeMoveOptions.Down;

        RaiseEvent(new MoveNodeEventArgs(MoveNodeEvent, NoteId, sourceNode.Id, moveOptions));
    }
}