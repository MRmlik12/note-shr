using NoteSHR.Core.ViewModel;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Core.Models;

public class Node(Guid id, Type type, ViewModelBase viewModel) : ViewModelBase
{
    [Reactive] public Guid Id { get; set; } = id;
    [Reactive] public Type Type { get; set; } = type;
    [Reactive] public ViewModelBase ViewModel { get; set; } = viewModel;
}