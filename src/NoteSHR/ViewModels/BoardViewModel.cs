using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class BoardViewModel : ViewModelBase
{
    [Reactive] public List<Note> Notes { get; set; } = [];
    [Reactive] public double ZoomX { get; set; } = 1d;
    [Reactive] public double ZoomY { get; set; } = 1d;
    [Reactive] public bool DeleteMode { get; set; }
    [Reactive] public bool EditMode { get; set; }
    
    public ReactiveCommand<PointerPressedEventArgs, Unit> CreateNoteCommand { get; set; }
    public ReactiveCommand<Guid, Unit> RemoveNote { get; set; }
    public ReactiveCommand<PointerReleasedEventArgs, Unit> UpdateNoteLocation { get; set; }
    public ReactiveCommand<Guid, Unit> AddNoteNodeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeDeleteModeStateCommand { get; set; }
    public ReactiveCommand<DeleteNodeEventArgs, Unit> DeleteNoteNodeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeEditModeStateCommand { get; set; }
    public ReactiveCommand<MoveNodeEventArgs, Unit> MoveNoteNodeCommand { get; set; }
    
    public BoardViewModel()
    {
        CreateNoteCommand = ReactiveCommand.Create((PointerPressedEventArgs args) =>
        {
            if (args.Source is not Canvas)
            {
                return;
            }
            
            if (!args.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                return;
            }

            var position = args.GetPosition(null);
            
            var note = new Note(Guid.NewGuid(), position.X, position.Y);
            Notes = [..Notes, note];
        });
        
        RemoveNote = ReactiveCommand.Create((Guid id) =>
        {
            Notes = Notes.Where(note => note.Id != id).ToList();
        });

        UpdateNoteLocation = ReactiveCommand.Create((PointerReleasedEventArgs args) =>
        {
            var id = ((args.Source as StackPanel)?.DataContext as Note)?.Id;
            if (id == null)
            {
                return;
            }
            
            if (!args.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                return;
            }
            
            var position = args.GetPosition(null);

            var noteIndex = Notes.FindIndex(x => x.Id == id);
            Notes[noteIndex].X = position.X;
            Notes[noteIndex].Y = position.Y;
            
            Notes = [..Notes];
        });

        AddNoteNodeCommand = ReactiveCommand.Create((Guid id) =>
        {
            var noteIndex = Notes.FindIndex(x => x.Id == id);
            Notes[noteIndex].Nodes.Add((Guid.NewGuid(), typeof(TextComponentControl), new TextComponentViewModel()));

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
            
            if (sourceNodeIndex > Notes[sourceNoteIndex].Nodes.Count - 1 || sourceNodeIndex < 0)
            {
                return;
            }
            
            var sourceNode = Notes[sourceNoteIndex].Nodes[sourceNodeIndex];
            Notes[sourceNoteIndex].Nodes[sourceNodeIndex] = Notes[sourceNoteIndex].Nodes[nodeToMoveIndex];
            Notes[sourceNoteIndex].Nodes[nodeToMoveIndex] = sourceNode;

            Notes = [..Notes];
        });
    }
}