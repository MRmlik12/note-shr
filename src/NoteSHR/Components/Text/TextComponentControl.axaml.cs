using Avalonia.Controls;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.Text;

public partial class TextComponentControl : UserControl, INode
{
    public TextComponentControl(TextComponentViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}