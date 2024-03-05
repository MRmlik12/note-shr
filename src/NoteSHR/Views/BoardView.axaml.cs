using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using NoteSHR.ViewModels;
using ReactiveUI;

namespace NoteSHR.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    public BoardView()
    {
        InitializeComponent();
        DataContext = new BoardViewModel();
        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.Notes, v => v.NoteItemsControl.ItemsSource)
                .DisposeWith(disposable);
        });
    }
    
    private bool isDragging;
    private Point startDragPoint;

    public void NoteItemsControl_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (e.Source is not Grid)
        {
            return;
        }
        
        isDragging = true;
        startDragPoint = e.GetPosition(NoteItemsControl);
    }

    public void NoteItemsControl_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!isDragging) 
            return;
        
        var currentPoint = e.GetPosition(NoteItemsControl);
        var delta = currentPoint - startDragPoint;

        Canvas.SetLeft(NoteItemsControl, Canvas.GetLeft(NoteItemsControl) + delta.X);
        Canvas.SetTop(NoteItemsControl, Canvas.GetTop(NoteItemsControl) + delta.Y);

        startDragPoint = currentPoint;
    }

    public void NoteItemsControl_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        isDragging = false;
    }
}