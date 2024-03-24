using Avalonia.Controls;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.Check;

public partial class CheckComponent : UserControl, INode
{
    public CheckComponent(CheckComponentViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}