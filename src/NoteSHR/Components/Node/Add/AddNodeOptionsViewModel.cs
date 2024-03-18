using System.Collections.Generic;
using NoteSHR.Components.Text;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.Node.Add;

public class AddNodeOptionsViewModel
{
    public List<ComponentType> Components { get; } = new()
    {
        new (typeof(TextComponentControl), "Text")
    };
}