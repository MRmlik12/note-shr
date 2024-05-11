using FluentAssertions;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;

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
}