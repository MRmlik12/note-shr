using Avalonia.Controls;
using Avalonia.Input;

namespace NoteSHR.Core.EventArgs;

public class BoardPointerEventArgs
{
    public Canvas Source { get; set; }
    public PointerEventArgs Args { get; set; }
}