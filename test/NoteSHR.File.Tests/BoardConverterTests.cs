using FluentAssertions;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;
using NoteSHR.File.Schemes;

namespace NoteSHR.File.Tests;

public class BoardConverterTests
{
    [Fact]
    public void ExportBoardToScheme_VerifyProperties()
    {
        var notes = new List<Note>
        {
            new (100, 200, "#ffffff"),
            new (200, 600, "#000000"),
            new (0, 0, "#ffffff")
        };
        
        notes[0].Nodes.Add(new Node(Guid.NewGuid(), typeof(TextComponentControl), new TextComponentViewModel
        {
            Text = "Hello, World!"
        }));
        notes[0].Nodes.Add(new Node(Guid.NewGuid(), typeof(TextComponentControl), new TextComponentViewModel
        {
            Text = "What's up?"
        }));
        
        const string boardName = "Hello";
        
        var scheme = BoardConverter.ConvertToScheme(boardName, notes);

        scheme.LastModifiedAt.Should().BeSameDateAs(DateTime.Now);
        scheme.Name.Should().Be(boardName);
        scheme.Notes.Count().Should().Be(3);

        foreach (var item in scheme.Notes.Select((value, i) => new {i, value }))
        {
            item.value.Id.Should().Be(notes[item.i].Id);
            item.value.X.Should().Be(notes[item.i].X);
            item.value.Y.Should().Be(notes[item.i].Y);
            item.value.BackgroundColor.Should().Be(notes[item.i].BackgroundColor);
            item.value.HeaderColor.Should().Be(notes[item.i].HeaderColor);
            item.value.Width.Should().Be(notes[item.i].Width);
            item.value.Nodes.Count().Should().Be(notes[item.i].Nodes.Count);
        }
    }
    
    [Fact]
    public void ExportBoardToScheme_ShouldNotesBeEmpty()
    {
        var notes = new List<Note>();
        
        const string boardName = "empty";
        var scheme = BoardConverter.ConvertToScheme(boardName, notes);

        scheme.Name.Should().Be(boardName);
        scheme.LastModifiedAt.Should().BeSameDateAs(DateTime.Today);
        scheme.Notes.Should().BeEmpty();
    }

    [Fact]
    public void ImportBoardFromScheme_VerifyProperties()
    {
        var scheme = new BoardScheme
        {
            Id = Guid.NewGuid(),
            Name = "Hello",
            LastModifiedAt = DateTime.Now.AddDays(-2),
            Notes = new List<NoteScheme>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    X = 100,
                    Y = 200,
                    HeaderColor = "#ffffff",
                    BackgroundColor = "#000000",
                    Width = 200,
                    Nodes = new List<NodeScheme>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Component = typeof(TextComponentControl).FullName,
                            Assembly = nameof(NoteSHR),
                            ViewModelType = typeof(TextComponentViewModel).FullName,
                            Data = new
                            {
                                Text = "Hello, World!"
                            }
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Component = typeof(TextComponentControl).FullName,
                            Assembly = nameof(NoteSHR),
                            ViewModelType = typeof(TextComponentViewModel).FullName,
                            Data = new 
                            {
                                Text = "What's up?"
                            }
                        }
                    }
                }
            }
        };

        var board = BoardConverter.ConvertBack(scheme);

        board.Name.Should().Be(scheme.Name);
        board.Id.Should().Be(scheme.Id);
        board.Notes.Count.Should().Be(scheme.Notes.Count());
        
        foreach (var item in scheme.Notes.Select((value, i) => new { i, value }))
        {
            var note = board.Notes[item.i];
            var noteScheme = item.value;

            note.Id.Should().Be(noteScheme.Id);
            note.X.Should().Be(noteScheme.X);
            note.Y.Should().Be(noteScheme.Y);
            note.HeaderColor.Should().Be(noteScheme.HeaderColor);
            note.BackgroundColor.Should().Be(noteScheme.BackgroundColor);
            note.Width.Should().Be(noteScheme.Width);
            note.Nodes.Count.Should().Be(noteScheme.Nodes.Count());
        }
    }
}