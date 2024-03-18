using System;
using Avalonia.Interactivity;

namespace NoteSHR.Components.Node.Add.EventArgs;

public class CreateNodeEventArgs(RoutedEvent? routedEvent, Guid noteId, Type nodeType) : RoutedEventArgs(routedEvent)
{
    public Guid NoteId { get; } = noteId;

    public Type NodeType { get; } = nodeType;
}