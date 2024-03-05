using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using NoteSHR.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class BoardViewModel : ViewModelBase
{
    [Reactive] public List<Note> Notes { get; set; } = [];
    
    public ReactiveCommand<PointerPressedEventArgs, Unit> CreateNoteCommand { get; set; }
    public ReactiveCommand<Guid, Unit> RemoveNote { get; set; }

    public BoardViewModel()
    {
        CreateNoteCommand = ReactiveCommand.Create((PointerPressedEventArgs args) =>
        {
            if (args.Source is Grid)
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
    }
}