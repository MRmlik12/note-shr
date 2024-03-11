using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
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

    private void Board_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Delta.Y > 0)
        {
            ViewModel!.ZoomX *= 1.2;
            ViewModel.ZoomY *= 1.2;
        }
        else
        {
            ViewModel!.ZoomX /= 1.2;
            ViewModel.ZoomY /= 1.2;
        }
    }
}