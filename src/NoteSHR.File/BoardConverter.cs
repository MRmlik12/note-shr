﻿using System.IO.Compression;
using System.Reflection;
using NoteSHR.Core.Models;
using NoteSHR.Core.ViewModel;
using NoteSHR.File.Schemes;

namespace NoteSHR.File;

internal static class BoardConverter
{
    private const string BlobUrlPattern = "blob://";
    
    internal static BoardScheme ConvertToScheme(Guid id, string boardName, List<Note> notes)
    {
        var scheme = new BoardScheme
        {
            Id = id,
            Version = 1,
            Name = boardName,
            LastModifiedAt = DateTime.Now
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
                var data = new Dictionary<string, object>();

                if (node.ViewModel is IDataPersistence)
                {
                    var nodeViewModelData = ((IDataPersistence)node.ViewModel).ExportValues();
                    
                    foreach (var property in nodeViewModelData.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (property.PropertyType == typeof(FileBlob))
                        {
                            var fileBlob = property.GetValue(nodeViewModelData) as FileBlob;

                            if (fileBlob != null)
                            {
                                var fileId = Guid.NewGuid();
                                fileBlob.SetProjectId(scheme.Id);
                                var uri = fileBlob.Write(fileId.ToString());
                                                                    
                                data.Add(property.Name, uri);
                            }
                            
                            continue;
                        }

                        data.Add(property.Name, property.GetValue(nodeViewModelData));
                    }
                }

                return new NodeScheme
                {
                    Id = node.Id,
                    Assembly = node.Type.Assembly.GetName().Name,
                    Component = node.Type.FullName,
                    ViewModelType = node.ViewModel.GetType().FullName,
                    Data = data
                };
            })
        });

        return scheme;
    }

    internal static Board ConvertBack(BoardScheme scheme, ZipArchive zipArchive)
    {
        var notes = new List<Note>();

        foreach (var noteScheme in scheme.Notes)
        {
            var note = new Note(noteScheme.X, noteScheme.Y, noteScheme.HeaderColor)
            {
                Id = noteScheme.Id,
                BackgroundColor = noteScheme.BackgroundColor,
                Width = noteScheme.Width
            };

            foreach (var nodeScheme in noteScheme.Nodes)
            {
                var assembly = Assembly.Load($"{nodeScheme.Assembly}");

                var type = assembly?.GetType(nodeScheme.Component);
                if (type == null) continue;

                var viewModelType = assembly?.GetType(nodeScheme.ViewModelType);
                if (viewModelType == null) continue;

                var viewModel = (ViewModelBase)Activator.CreateInstance(viewModelType);
                if (viewModel is IDataPersistence persistence)
                {
                    foreach (var data in nodeScheme.Data.Where(x => x.Value is string str && str.Contains(BlobUrlPattern)))
                    {
                        var blob = new FileBlob(scheme.Id, data!.Value as string, zipArchive);
                        nodeScheme.Data[data.Key] = blob;
                    }

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