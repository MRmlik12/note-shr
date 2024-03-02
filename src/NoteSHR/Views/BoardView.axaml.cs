using System.Reactive.Disposables;
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
}