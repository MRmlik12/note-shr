using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NoteSHR.Components.Check;
using NoteSHR.Components.Image;
using NoteSHR.Components.List;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class BoardViewModel : ViewModelBase
{
    private const double Tolerance = 0.5d;

    public BoardViewModel()
    {
        CreateNoteCommand = ReactiveCommand.Create((PointerPressedEventArgs args) =>
        {
            LastMousePosition = args.GetPosition(null);
            if (args.Source is not Canvas) return;

            if (!args.GetCurrentPoint(null).Properties.IsLeftButtonPressed) return;

            var position = args.GetPosition(null);

            var note = new Note(Guid.NewGuid(), position.X, position.Y);
            Notes = [..Notes, note];
        });

        RemoveNote = ReactiveCommand.Create((Guid id) => { Notes = Notes.Where(note => note.Id != id).ToList(); });

        UpdateNoteLocation = ReactiveCommand.Create((PointerReleasedEventArgs args) =>
        {
            var id = ((args.Source as StackPanel)?.DataContext as Note)?.Id;
            if (id == null) return;

            var pointerPoint = args.GetCurrentPoint(null);
            if (pointerPoint.Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased) return;

            if (Math.Abs(pointerPoint.Position.X - LastMousePosition.X) < Tolerance
                && Math.Abs(pointerPoint.Position.Y - LastMousePosition.Y) < Tolerance)
                return;


            var noteIndex = Notes.FindIndex(x => x.Id == id);
            Notes[noteIndex].X = pointerPoint.Position.X;
            Notes[noteIndex].Y = pointerPoint.Position.Y;

            Notes = [..Notes];
        });

        AddNoteNodeCommand = ReactiveCommand.Create(((Guid, NodeType) item) =>
        {
            var noteIndex = Notes.FindIndex(x => x.Id == item.Item1);

            var (componentType, componentVm) = item.Item2 switch
            {
                NodeType.Text => (typeof(TextComponentControl), new TextComponentViewModel()),
                NodeType.Image => (typeof(ImageComponent), new ImageComponentViewModel()),
                NodeType.Check => (typeof(CheckComponent), new CheckComponentViewModel()),
                NodeType.List => (typeof(ListComponent), (ViewModelBase)new ListComponentViewModel()),
                _ => throw new ArgumentOutOfRangeException()
            };

            Notes[noteIndex].Nodes.Add((Guid.NewGuid(), componentType, componentVm));

            Notes = [..Notes];
        });

        ChangeDeleteModeStateCommand = ReactiveCommand.Create(() =>
        {
            DeleteMode = !DeleteMode;
            Notes = [..Notes];
        });

        ChangeEditModeStateCommand = ReactiveCommand.Create(() =>
        {
            EditMode = !EditMode;
            Notes = [..Notes];
        });

        DeleteNoteNodeCommand = ReactiveCommand.Create((DeleteNodeEventArgs args) =>
        {
            var noteIndex = Notes.FindIndex(x => x.Id == args.NoteId);
            Notes[noteIndex].Nodes = Notes[noteIndex].Nodes.Where(x => x.Item1 != args.NodeId).ToList();
            Notes = [..Notes];
        });

        MoveNoteNodeCommand = ReactiveCommand.Create((MoveNodeEventArgs args) =>
        {
            var sourceNoteIndex = Notes.FindIndex(x => x.Id == args.NoteId);
            var nodeToMoveIndex = Notes[sourceNoteIndex].Nodes.FindIndex(x => x.Item1 == args.NodeToMoveId);
            var sourceNodeIndex = nodeToMoveIndex + (int)args.MoveOptions;

            if (sourceNodeIndex > Notes[sourceNoteIndex].Nodes.Count - 1 || sourceNodeIndex < 0) return;

            var sourceNode = Notes[sourceNoteIndex].Nodes[sourceNodeIndex];
            Notes[sourceNoteIndex].Nodes[sourceNodeIndex] = Notes[sourceNoteIndex].Nodes[nodeToMoveIndex];
            Notes[sourceNoteIndex].Nodes[nodeToMoveIndex] = sourceNode;

            Notes = [..Notes];
        });
    }

    [Reactive] public List<Note> Notes { get; set; } = [];
    // [Reactive] public double ZoomX { get; set; } = 1d;
    // [Reactive] public double ZoomY { get; set; } = 1d;
    [Reactive] public bool DeleteMode { get; set; }
    [Reactive] public bool EditMode { get; set; }

    private Point LastMousePosition { get; set; }

    public ReactiveCommand<PointerPressedEventArgs, Unit> CreateNoteCommand { get; set; }
    public ReactiveCommand<Guid, Unit> RemoveNote { get; set; }
    public ReactiveCommand<PointerReleasedEventArgs, Unit> UpdateNoteLocation { get; set; }
    public ReactiveCommand<(Guid, NodeType), Unit> AddNoteNodeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeDeleteModeStateCommand { get; set; }
    public ReactiveCommand<DeleteNodeEventArgs, Unit> DeleteNoteNodeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeEditModeStateCommand { get; set; }
    public ReactiveCommand<MoveNodeEventArgs, Unit> MoveNoteNodeCommand { get; set; }
}