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
using Avalonia.Media;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.NoteNode;

public class NoteNodeComponent : UserControl
{
    private const string DeleteButtonName = "DeleteButton";
    private const string EditButtonName = "EditModeButton";
    private const string MoveUpButtonName = "MoveUpButton";
    private const string MoveDownButtonName = "MoveDownButton";

    public static readonly StyledProperty<ObservableCollection<Node>> NodesProperty =
        AvaloniaProperty.Register<NoteNodeComponent, ObservableCollection<Node>>(nameof(Nodes),
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

    private readonly List<Grid> _nodeGrids = new();

    private readonly StackPanel _stackPanel;

    private bool _firstInitialized = false;

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

    private ObservableCollection<Node> Nodes
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
        Console.WriteLine(change.Property.Name);
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
        else if (change.Property.Name == nameof(Nodes) && change.NewValue != null && !_firstInitialized)
        {
            foreach (var nodeVm in change.NewValue as ObservableCollection<Node>)
            {
                var grid = InitializeNodeGrid(nodeVm);
                _stackPanel.Children.Add(grid);
            }
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

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var nodeVm in e.OldItems.Cast<Node>())
            {
                var grid = _nodeGrids.Single(x => (Guid)x.DataContext == nodeVm.Id);
                _nodeGrids.Remove(grid);
                _stackPanel.Children.Remove(grid);
            }
        }

        if (!(e.NewItems?.Count > 0)) return;

        foreach (var nodeVm in e.NewItems.Cast<Node>())
        {
            var grid = InitializeNodeGrid(nodeVm);
            _stackPanel.Children.Add(grid);
        }
    }

    private Grid InitializeNodeGrid(Node nodeViewModel)
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

        _firstInitialized = true;
        
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
            },
            VerticalAlignment = VerticalAlignment.Center
        };

        var moveUpButton = new StackPanel
        {
            Name = MoveUpButtonName,
            Width = 16,
            Height = 16
        };

        var caretUpIcon = new Avalonia.Svg.Skia.Svg(new Uri("avares://NoteSHR/Assets/Icons/caret-up-outline.svg"))
        {
            Name = MoveUpButtonName,
            Path = "avares://NoteSHR/Assets/Icons/caret-up-outline.svg",
            DataContext = id
        };

        Avalonia.Svg.Skia.Svg.SetCss(caretUpIcon, ".foreground { fill: #FFFFFF; }");
        moveUpButton.Children.Add(caretUpIcon);

        var moveDownButton = new StackPanel
        {
            Name = MoveDownButtonName,
            Width = 16,
            Height = 16
        };

        var caretDownIcon = new Avalonia.Svg.Skia.Svg(new Uri("avares://NoteSHR/Assets/Icons/caret-down-outline.svg"))
        {
            Name = MoveDownButtonName,
            Path = "avares://NoteSHR/Assets/Icons/caret-down-outline.svg",
            DataContext = id
        };

        Avalonia.Svg.Skia.Svg.SetCss(caretDownIcon, ".foreground { fill: #FFFFFF; }");
        moveDownButton.Children.Add(caretDownIcon);

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
            Background = Brushes.Transparent,
            DataContext = id
        };

        var deleteIcon = new Avalonia.Svg.Skia.Svg(new Uri("avares://NoteSHR/Assets/Icons/trash-outline.svg"))
        {
            Path = "avares://NoteSHR/Assets/Icons/trash-outline.svg",
            Width = 16,
            Height = 16
        };
        Avalonia.Svg.Skia.Svg.SetCss(deleteIcon, ".foreground { stroke: #FF0000; }");
        deleteButton.Content = deleteIcon;

        Grid.SetColumn(deleteButton, 0);

        deleteButton.Click += (_, _) =>
            RaiseEvent(new DeleteNodeEventArgs(DeleteNodeEvent, NoteId, id));

        return deleteButton;
    }

    private void EditModeButtonClicked(object? sender, PointerPressedEventArgs e)
    {
        var control = e.Source as Control;
        var sourceNode = Nodes.SingleOrDefault(x => x.Id == (Guid)control.DataContext);
        var moveOptions = control?.Name == MoveUpButtonName ? NodeMoveOptions.Up : NodeMoveOptions.Down;

        RaiseEvent(new MoveNodeEventArgs(MoveNodeEvent, NoteId, sourceNode.Id, moveOptions));
    }
}