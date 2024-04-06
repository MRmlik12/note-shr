using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NoteSHR.Core.Models;

namespace NoteSHR.Components.List;

public partial class ListComponent : UserControl, INode
{
    public ListComponent(ListComponentViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        var prefixType = menuItem.CommandParameter as string;

        (DataContext as ListComponentViewModel)?.ChangePrefixTypeCommand.Execute(prefixType!).Subscribe();
    }
}