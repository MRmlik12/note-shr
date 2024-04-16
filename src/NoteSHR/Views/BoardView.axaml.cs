using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using NoteSHR.Core.Models;
using NoteSHR.ViewModels;
using ReactiveUI;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace NoteSHR.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private readonly BoardViewModel _vm;
    private bool _changeNoteRightSide;

    public BoardView()
    {
        InitializeComponent();
        _vm = new BoardViewModel();
        DataContext = _vm;

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.Notes, v => v.NoteItemsControl.ItemsSource)
                .DisposeWith(disposable);
        });
    }

    private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        var nodeType = Enum.Parse<NodeType>(menuItem.CommandParameter as string);
        var argumentsToCommand = ((menuItem.DataContext as Note)!.Id, nodeType);

        ViewModel!.AddNoteNodeCommand.Execute(argumentsToCommand).Subscribe();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _changeNoteRightSide = true;
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _changeNoteRightSide = false;
        
        var rectangle = (Rectangle)e.Source!;
        var grid = (Grid)rectangle.Parent!;
        
        var p = e.GetPosition(rectangle);
        grid!.Children[0].Width += p.X;
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_changeNoteRightSide)
        {
            return;
        }
        
        var rectangle = (Rectangle)e.Source!;
        var grid = (Grid)rectangle.Parent!;
        
        var p = e.GetPosition(rectangle);
        grid!.Children[0].Width += p.X;
    }
}