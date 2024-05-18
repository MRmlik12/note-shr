using System;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using NoteSHR.ViewModels;

namespace NoteSHR.Views;

public partial class BoardToolbar : ReactiveUserControl<BoardToolbarViewModel> 
{
    public static readonly RoutedEvent<RoutedEventArgs> ExportBoardEvent =
        RoutedEvent.Register<object, RoutedEventArgs>(nameof(ExportBoard), RoutingStrategies.Direct);
    
   public event EventHandler<RoutedEventArgs> ExportBoard
   {
       add => AddHandler(ExportBoardEvent, value);
       remove => RemoveHandler(ExportBoardEvent, value);
   }
    
    public BoardToolbar()
    {
        InitializeComponent();
    }

    private void ExportBoardButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ExportBoardEvent));
    }
}