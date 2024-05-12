using System.Reflection;
using NoteSHR.Core.Models;
using NoteSHR.Core.ViewModel;
using NoteSHR.File.Schemes;

namespace NoteSHR.File;

internal static class BoardConverter
{
    public static BoardScheme ConvertToScheme(string boardName, List<Note> notes)
    {
        var scheme = new BoardScheme
        {
            Name = boardName,
            LastModifiedAt = DateTime.Now,
        };

        scheme.Notes = notes.Select(note => new NoteScheme
        {
            Id = note.Id,
            BackgroundColor = note.BackgroundColor,
            HeaderColor = note.HeaderColor,
            Width = note.Width,
            X = note.X,
            Y = note.Y,
            Nodes = note.Nodes.Select(node =>
            {
                object? data = null;
                
                if (node.ViewModel is IDataPersistence)
                {
                    data = ((IDataPersistence)node.ViewModel).ExportValues();
                }
                
                return new NodeScheme
                {
                    Id = node.Id,
                    Assembly = node.Type.Assembly.GetName().Name,
                    Component = node.Type.FullName,
                    ViewModelType = node.ViewModel.GetType().Name,
                    Data = data
                };
            })
        });
        
        return scheme;
    }

    public static Board ConvertBack(BoardScheme scheme)
    {
        var notes = new List<Note>();
        
        foreach (var noteScheme in scheme.Notes)
        {
            var note = new Note(noteScheme.X, noteScheme.Y, noteScheme.HeaderColor)
            {
                Id = noteScheme.Id,
                BackgroundColor = noteScheme.BackgroundColor,
                Width = noteScheme.Width,
            };
            
            foreach (var nodeScheme in noteScheme.Nodes)
            {
                var assembly = Assembly.Load($"{nodeScheme.Assembly}");
                
                var type = assembly?.GetType(nodeScheme.Component);
                if (type == null)
                {
                    continue;
                }

                var viewModelType = assembly?.GetType(nodeScheme.ViewModelType);
                if (viewModelType == null)
                {
                    continue;
                }

                var viewModel = (ViewModelBase)Activator.CreateInstance(viewModelType);
                if (viewModel is IDataPersistence persistence)
                {
                    persistence?.ConvertValues(nodeScheme.Data);
                }
                
                note.Nodes.Add(new Node(Guid.NewGuid(), type, viewModel));
            }
            
            notes.Add(note);
        }

        return new Board
        {
            Id = scheme.Id,
            Name = scheme.Name,
            Notes = notes
        };
    }
}