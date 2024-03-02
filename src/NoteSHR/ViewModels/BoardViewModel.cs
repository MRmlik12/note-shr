using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia.Input;
using NoteSHR.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class BoardViewModel : ViewModelBase
{
    [Reactive] public List<Note> Notes { get; set; } = [];
    
    public ReactiveCommand<PointerPressedEventArgs, Unit> CreateNoteCommand { get; set; }

    public BoardViewModel()
    {
        CreateNoteCommand = ReactiveCommand.Create((PointerPressedEventArgs args) =>
        {
            var note = new Note(Guid.NewGuid(), args.GetPosition(null).X, args.GetPosition(null).Y);
            Notes = [..Notes, note];
        });
    }
}