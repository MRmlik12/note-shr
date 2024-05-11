using NoteSHR.Core.Models;
using NoteSHR.File.Models;

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
                    Component = node.Type.Name,
                    Data = data 
                };
            })
        });
        
        return scheme;
    }
}