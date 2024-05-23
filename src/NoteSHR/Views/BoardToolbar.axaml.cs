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

    public static readonly RoutedEvent<RoutedEventArgs> ImportBoardEvent =
        RoutedEvent.Register<object, RoutedEventArgs>(nameof(ImportBoard), RoutingStrategies.Direct);
    
    public static readonly StyledProperty<string> BoardNameProperty =
        AvaloniaProperty.Register<BoardToolbar, string>(nameof(BoardName), defaultBindingMode: BindingMode.TwoWay);

    public BoardToolbar()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        var boardNameTextBox = this.FindControl<TextBox>("BoardNameTextBox");
        boardNameTextBox?.Bind(TextBox.TextProperty, new Binding
        {
            Source = this,
            Path = "BoardName",
            Mode = BindingMode.TwoWay
        });
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

    public event EventHandler<RoutedEventArgs> ImportBoard 
    {
        add => AddHandler(ImportBoardEvent, value);
        remove => RemoveHandler(ImportBoardEvent, value);
    }
    
    private void ExportBoardButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ExportBoardEvent));
    }

    private void ImportBoardButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ImportBoardEvent));
    }
}