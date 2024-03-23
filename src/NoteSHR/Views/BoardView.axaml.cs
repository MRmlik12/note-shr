using System.Reactive.Disposables;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using NoteSHR.ViewModels;
using ReactiveUI;

namespace NoteSHR.Views;

public partial class BoardView : ReactiveUserControl<BoardViewModel>
{
    private readonly BoardViewModel _vm;

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