using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace NoteSHR.Views;

public partial class BoardToolbar : UserControl
{
    public static readonly RoutedEvent<RoutedEventArgs> ExportBoardEvent =
        RoutedEvent.Register<object, RoutedEventArgs>(nameof(ExportBoard), RoutingStrategies.Direct);

    public static readonly StyledProperty<string> BoardNameProperty =
        AvaloniaProperty.Register<BoardToolbar, string>(nameof(BoardName), defaultBindingMode: BindingMode.TwoWay);

    public BoardToolbar()
    {
        InitializeComponent();
    }

    public string BoardName
    {
        get => GetValue(BoardNameProperty);
        set => SetValue(BoardNameProperty, value);
    }

    public event EventHandler<RoutedEventArgs> ExportBoard
    {
        add => AddHandler(ExportBoardEvent, value);
        remove => RemoveHandler(ExportBoardEvent, value);
    }

    private void ExportBoardButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ExportBoardEvent));
    }
}