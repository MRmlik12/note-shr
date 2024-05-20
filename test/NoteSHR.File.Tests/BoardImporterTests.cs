using FluentAssertions;
using NoteSHR.Components.Check;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;

namespace NoteSHR.File.Tests;

public class BoardImporterTests
{
    private const string BoardName = "import-test";

    [Fact]
    public async Task TryImportBoardFromFile_VerifyProperties()
    {
        var notes = new List<Note>
        {
            new(100, 200, "#ffffff"),
            new(200, 600, "#000000"),
            new(0, 0, "#ffffff")
        };

        notes[0].Nodes.Add(new Node(Guid.NewGuid(), typeof(TextComponentControl), new TextComponentViewModel
        {
            Text = "Hello, World!"
        }));
        notes[0].Nodes.Add(new Node(Guid.NewGuid(), typeof(TextComponentControl), new TextComponentViewModel
        {
            Text = "What's up?"
        }));

        notes[1].Nodes.Add(new Node(Guid.NewGuid(), typeof(CheckComponent), new CheckComponentViewModel
        {
            Text = "Prepare meal",
            Checked = true
        }));

        var outputPath = await BoardExporter.ExportToFile(notes, BoardName, ".");
        var importedNotes = await BoardImporter.ImportFromFile(outputPath);

        importedNotes.Notes.Should().HaveCount(notes.Count);
    }
}