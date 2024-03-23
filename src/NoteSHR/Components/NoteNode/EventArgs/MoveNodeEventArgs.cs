using System;
using Avalonia.Interactivity;

namespace NoteSHR.Components.NoteNode.EventArgs;

public class MoveNodeEventArgs(RoutedEvent? routedEvent, Guid noteId, Guid nodeToMoveId, NodeMoveOptions moveOptions)
    : RoutedEventArgs(routedEvent)
{
    public Guid NodeToMoveId { get; } = nodeToMoveId;
    public Guid NoteId { get; } = noteId;
    public NodeMoveOptions MoveOptions { get; } = moveOptions;
}