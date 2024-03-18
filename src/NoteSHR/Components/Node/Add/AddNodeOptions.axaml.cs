using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using NoteSHR.Components.Node.Add.EventArgs;
using NoteSHR.Components.NoteNode;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.Node.Add;

public partial class AddNodeOptions : UserControl
{
    public static readonly StyledProperty<Guid> NoteIdProperty =
        AvaloniaProperty.Register<NoteNodeComponent, Guid>(nameof(NoteId), defaultBindingMode: BindingMode.TwoWay);

    public static readonly RoutedEvent<CreateNodeEventArgs> CreateNodeEvent =
        RoutedEvent.Register<NoteNodeComponent, CreateNodeEventArgs>(nameof(CreateNodeEvent), RoutingStrategies.Direct);

    public AddNodeOptions()
    {
        InitializeComponent();
        DataContext = new AddNodeOptionsViewModel();
    }

    private Guid NoteId
    {
        get => GetValue(NoteIdProperty);
        set => SetValue(NoteIdProperty, value);
    }

    public event EventHandler<CreateNodeEventArgs> CreateNode
    {
        add => AddHandler(CreateNodeEvent, value);
        remove => RemoveHandler(CreateNodeEvent, value);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if ((e.Source as Button).DataContext is not ComponentType data) return;

        RaiseEvent(new CreateNodeEventArgs(CreateNodeEvent, NoteId, data.Type));
    }
}