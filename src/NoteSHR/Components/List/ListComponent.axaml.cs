using Avalonia.Controls;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.List;

public partial class ListComponent : UserControl, INode
{
    public ListComponent(ListComponentViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}