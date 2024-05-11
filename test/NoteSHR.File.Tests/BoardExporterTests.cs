using System.IO.Compression;
using FluentAssertions;
using NoteSHR.Components.Check;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;

namespace NoteSHR.File.Tests;

public class BoardExporterTests
{
    private const string BoardName = "test-board";
    
    [Fact]
    public async Task TryExportBoard_OK()
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
        
        notes[1].Nodes.Add(new Node(Guid.NewGuid(), typeof(CheckComponent), new CheckComponentViewModel
        {
            Text = "Prepare meal",
            Checked = true
        }));
        
        var outputPath = await new BoardExporter().ExportToFile(notes, BoardName, ".");
        
        var zip = ZipFile.Open(outputPath, ZipArchiveMode.Read);
        zip.Entries.Select(x => x.FullName).Count().Should().Be(1);
    }
}