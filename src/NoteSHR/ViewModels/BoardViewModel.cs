using System;
using System.Collections.ObjectModel;
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
            Notes.Add(note);
        });

        RemoveNote = ReactiveCommand.Create((Guid id) =>
        {
            Notes.Remove(Notes.Where(note => note.Id == id).Single());
        });

        UpdateNoteLocation = ReactiveCommand.Create((PointerReleasedEventArgs args) =>
        {
            var id = ((args.Source as StackPanel)?.DataContext as Note)?.Id;
            if (id == null) return;

            var pointerPoint = args.GetCurrentPoint(null);
            if (pointerPoint.Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased) return;

            if (Math.Abs(pointerPoint.Position.X - LastMousePosition.X) < Tolerance
                && Math.Abs(pointerPoint.Position.Y - LastMousePosition.Y) < Tolerance)
                return;


            var note = Notes.SingleOrDefault(x => x.Id == id);
            if (note != null)
            {
                note.X = pointerPoint.Position.X;
                note.Y = pointerPoint.Position.Y;
            }
        });

        AddNoteNodeCommand = ReactiveCommand.Create(((Guid, NodeType) item) =>
        {
            var note = Notes.Where(x => x.Id == item.Item1).SingleOrDefault();

            var (componentType, componentVm) = item.Item2 switch
            {
                NodeType.Text => (typeof(TextComponentControl), new TextComponentViewModel()),
                NodeType.Image => (typeof(ImageComponent), new ImageComponentViewModel()),
                NodeType.Check => (typeof(CheckComponent), new CheckComponentViewModel()),
                NodeType.List => (typeof(ListComponent), (ViewModelBase)new ListComponentViewModel()),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (note == null) return;

            note.Nodes.Add(new NodeViewModel(Guid.NewGuid(), componentType, componentVm));
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
            var noteIndex = Notes.IndexOf(Notes.Where(x => x.Id == args.NoteId).Single());
            // Notes[noteIndex].Nodes = Notes[noteIndex].Nodes.Where(x => x.Id != args.NodeId).ToList();
        });

        MoveNoteNodeCommand = ReactiveCommand.Create((MoveNodeEventArgs args) =>
        {
            // var sourceNoteIndex = Notes.IndexOf(Notes.Where(x => x.Id == args.NoteId).Single());
            // var nodeToMoveIndex = Notes[sourceNoteIndex].Nodes.FindIndex(x => x.Id == args.NodeToMoveId);
            // var sourceNodeIndex = nodeToMoveIndex + (int)args.MoveOptions;
            //
            // if (sourceNodeIndex > Notes[sourceNoteIndex].Nodes.Count - 1 || sourceNodeIndex < 0) return;
            //
            // var sourceNode = Notes[sourceNoteIndex].Nodes[sourceNodeIndex];
            // Notes[sourceNoteIndex].Nodes[sourceNodeIndex] = Notes[sourceNoteIndex].Nodes[nodeToMoveIndex];
            // Notes[sourceNoteIndex].Nodes[nodeToMoveIndex] = sourceNode;
        });
    }

    [Reactive] public ObservableCollection<Note> Notes { get; set; } = [];

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