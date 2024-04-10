using System;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class NodeViewModel(Guid id, Type type, ViewModelBase viewModel) : ViewModelBase
{
    [Reactive] public Guid Id { get; set; } = id;
    [Reactive] public Type Type { get; set; } = type;
    [Reactive] public ViewModelBase ViewModel { get; set; } = viewModel;
}