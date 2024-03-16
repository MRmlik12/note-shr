using System;
using Avalonia.Interactivity;

namespace NoteSHR.Components.NoteNode.EventArgs;

public class MoveNodeEventArgs(RoutedEvent? routedEvent, Guid noteId, Guid sourceNodeId, Guid nodeToMoveId) : RoutedEventArgs(routedEvent)
{
    public Guid NodeToMoveId { get; } = nodeToMoveId;
    public Guid SourceNodeId { get; } = sourceNodeId;
    public Guid NoteId { get; } = noteId;
}