using System;
using Avalonia.Interactivity;

namespace NoteSHR.Components.NoteNode.EventArgs;

public class DeleteNodeEventArgs(RoutedEvent? routedEvent, Guid noteId, Guid nodeId) : RoutedEventArgs(routedEvent)
{
    public Guid NoteId { get; } = noteId;
    public Guid NodeId { get; } = nodeId;
}