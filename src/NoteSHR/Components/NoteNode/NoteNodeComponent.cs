﻿using System;
using System.Collections.Generic;
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
    private const string MoveUpButtonName = "MoveUpButton";
    private const string MoveDownButtonName = "MoveDownButton";

    public static readonly StyledProperty<List<(Guid, Type, ViewModelBase)>> NodesProperty =
        AvaloniaProperty.Register<NoteNodeComponent, List<(Guid, Type, ViewModelBase)>>(nameof(Nodes),
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

    protected override void OnInitialized()
    {
        foreach (var (id, type, vm) in Nodes)
        {
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = id,
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
                var deleteButton = new Button
                {
                    Content = "D",
                    DataContext = (id, type, vm)
                };

                Grid.SetColumn(deleteButton, 0);

                deleteButton.Click += (sender, args) =>
                    RaiseEvent(new DeleteNodeEventArgs(DeleteNodeEvent, NoteId, id));
                grid.Children.Add(deleteButton);
            }

            if (EditMode)
            {
                var editModeGrid = new Grid
                {
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

                grid.Children.Add(editModeGrid);
            }

            var node = (Control)Activator.CreateInstance(type, vm);
            node.HorizontalAlignment = HorizontalAlignment.Stretch;
            node.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(node, 1);

            grid.Children.Add(node);
            _stackPanel.Children.Add(grid);
        }

        Content = _stackPanel;
    }

    private void EditModeButtonClicked(object? sender, PointerPressedEventArgs e)
    {
        var control = e.Source as TextBlock;
        var sourceNode = Nodes.SingleOrDefault(x => x.Item1 == (Guid)control.DataContext);
        var moveOptions = control?.Name == MoveUpButtonName ? NodeMoveOptions.Up : NodeMoveOptions.Down;

        RaiseEvent(new MoveNodeEventArgs(MoveNodeEvent, NoteId, sourceNode.Item1, moveOptions));
    }
}