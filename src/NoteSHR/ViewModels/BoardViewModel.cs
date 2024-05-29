using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NoteSHR.Components.Check;
using NoteSHR.Components.Image;
using NoteSHR.Components.List;
using NoteSHR.Components.NoteNode.EventArgs;
using NoteSHR.Components.Text;
using NoteSHR.Core.Helpers;
using NoteSHR.Core.Models;
using NoteSHR.Core.ViewModel;
using NoteSHR.File;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.ViewModels;

public class BoardViewModel : ViewModelBase
{
    private bool _noteMoveState;

    public BoardViewModel()
    {
        CreateNoteCommand = ReactiveCommand.Create((PointerPressedEventArgs args) =>
        {
            LastMousePosition = args.GetPosition(args.Source as Canvas);
            if (args.Source is not Canvas)
            {
                if (args.Source is not Grid header)
                    return;

                if (header?.Name == "Header")
                {
                    _noteMoveState = true;
                }

                return;
            }

            if (!args.GetCurrentPoint(args.Source as Canvas).Properties.IsLeftButtonPressed) return;

            var position = args.GetPosition(args.Source as Canvas);

            var note = new Note(position.X, position.Y, ColorHelper.GenerateColor());

            Notes.Add(note);
        });

        RemoveNote = ReactiveCommand.Create((Guid id) =>
        {
            Notes.Remove(Notes.Where(note => note.Id == id).Single());
        });

        MoveNoteCommand = ReactiveCommand.Create((PointerEventArgs args) =>
        {
            if (!_noteMoveState) return;
            
            var noteHeader = (Grid)args.Source!;
            var noteComponent = (Grid)noteHeader.Parent;
            var p = args.GetPosition(null);
            var note = Notes.SingleOrDefault(x => x.Id == ((Note)noteComponent.DataContext).Id);

            if (note != null)
            {
                note.X = p.X;
                note.Y = p.Y;
            }
        });

        UpdateNoteLocation = ReactiveCommand.Create((PointerReleasedEventArgs args) =>
        {
            if (!_noteMoveState) return;

            var id = ((args.Source as Grid)?.DataContext as Note)?.Id;
            if (id == null) return;

            var pointerPoint = args.GetCurrentPoint(args.Source as Canvas);
            if (pointerPoint.Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased) return;

            _noteMoveState = false;

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

            note.Nodes.Add(new Node(Guid.NewGuid(), componentType, componentVm));
        });

        ChangeDeleteModeStateCommand = ReactiveCommand.Create(() => { DeleteMode = !DeleteMode; });

        ChangeEditModeStateCommand = ReactiveCommand.Create(() => { EditMode = !EditMode; });

        DeleteNoteNodeCommand = ReactiveCommand.Create((DeleteNodeEventArgs args) =>
        {
            var note = Notes.Single(x => x.Id == args.NoteId);
            note.Nodes.Remove(note.Nodes.Single(x => x.Id == args.NodeId));
        });

        MoveNoteNodeCommand = ReactiveCommand.Create((MoveNodeEventArgs args) =>
        {
            var sourceNoteIndex = Notes.IndexOf(Notes.Where(x => x.Id == args.NoteId).Single());
            var nodeToMoveIndex = Notes[sourceNoteIndex].Nodes
                .IndexOf(Notes[sourceNoteIndex].Nodes.Single(x => x.Id == args.NodeToMoveId));
            var sourceNodeIndex = nodeToMoveIndex + (int)args.MoveOptions;

            if (sourceNodeIndex > Notes[sourceNoteIndex].Nodes.Count - 1 || sourceNodeIndex < 0) return;

            Notes[sourceNoteIndex].Nodes.Move(sourceNodeIndex, nodeToMoveIndex);
        });

        ExportBoardCommand = ReactiveCommand.CreateFromTask(async (RoutedEventArgs args) =>
        {
            if (OperatingSystem.IsBrowser())
            {
                await BoardExporter.ExportToFile(Guid.NewGuid(), Notes.ToList(), Name, string.Empty, App.FileService);
                return;
            }
            
            var topLevel = TopLevel.GetTopLevel(args.Source as Visual);
            var saveFilePickerOptions = new FolderPickerOpenOptions
            {
                Title = "Select path to export board",
                AllowMultiple = false
            };

            var folderPicker = await topLevel.StorageProvider.OpenFolderPickerAsync(saveFilePickerOptions);

            if (folderPicker.Count == 0) return;

            await BoardExporter.ExportToFile(Guid.NewGuid(), Notes.ToList(), Name, folderPicker[0].Path.AbsolutePath);
        });

        ImportBoardCommand = ReactiveCommand.CreateFromTask(async (RoutedEventArgs args) =>
        {
            string path;
            if (OperatingSystem.IsBrowser())
            {
                path = await App.FileService.GetFileUrl();
                if (string.IsNullOrEmpty(path)) return;
            }
            else
            {
                var topLevel = TopLevel.GetTopLevel(args.Source as Visual);
                var openFilePickerOptions = new FilePickerOpenOptions()
                {
                    Title = "Select file",
                    AllowMultiple = false
                };

                var selectedFiles = await topLevel.StorageProvider.OpenFilePickerAsync(openFilePickerOptions);
                if (selectedFiles.Count == 0) return;

                path = selectedFiles[0].Path.AbsolutePath;
            }

            var importedBoard = await BoardImporter.ImportFromFile(path);

            Name = importedBoard.Name;
            Notes = new ObservableCollection<Note>(importedBoard.Notes);
        });
    }

    [Reactive] public string Name { get; set; }
    [Reactive] public ObservableCollection<Note> Notes { get; set; } = [];
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
    public ReactiveCommand<PointerEventArgs, Unit> MoveNoteCommand { get; set; }
    public ReactiveCommand<RoutedEventArgs, Unit> ExportBoardCommand { get; set; }
    public ReactiveCommand<RoutedEventArgs, Unit> ImportBoardCommand { get; set; }
}