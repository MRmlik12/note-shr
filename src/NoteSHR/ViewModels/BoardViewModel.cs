using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class BoardViewModel : ViewModelBase
{
    [Reactive] public List<Note> Notes { get; set; } = [];
    
    public ReactiveCommand<PointerPressedEventArgs, Unit> CreateNoteCommand { get; set; }
    public ReactiveCommand<Guid, Unit> RemoveNote { get; set; }
    public ReactiveCommand<PointerReleasedEventArgs, Unit> UpdateNoteLocation { get; set; }
    public ReactiveCommand<Guid, Unit> AddNoteNodeCommand { get; set; }

    public BoardViewModel()
    {
        CreateNoteCommand = ReactiveCommand.Create((PointerPressedEventArgs args) =>
        {
            if (args.Source is StackPanel)
            {
                return;
            }
            
            var note = new Note(Guid.NewGuid(), args.GetPosition(null).X, args.GetPosition(null).Y);
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

            var noteIndex = Notes.FindIndex(x => x.Id == id);
            Notes[noteIndex].X = args.GetPosition(null).X;
            Notes[noteIndex].Y = args.GetPosition(null).Y;
            
            Notes = [..Notes];
        });

        AddNoteNodeCommand = ReactiveCommand.Create((Guid id) =>
        {
            var noteIndex = Notes.FindIndex(x => x.Id == id);
            Notes[noteIndex].Nodes.Add(new TextComponentControl());

            Notes = [..Notes];
        });
    }
}