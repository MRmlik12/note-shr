using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NoteSHR.ViewModels;

namespace NoteSHR.Core.Models;

public class Note(Guid id, double x, double y) : INotifyPropertyChanged
{
    private ObservableCollection<NodeViewModel> _nodes = new();
    private double _x = x;
    private double _y = y;

    public Guid Id { get; set; } = id;

    public double X
    {
        get => _x;
        set
        {
            _x = value;
            OnPropertyChanged(nameof(X));
        }
    }

    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            OnPropertyChanged(nameof(Y));
        }
    }

    public ObservableCollection<NodeViewModel> Nodes
    {
        get => _nodes;
        set
        {
            _nodes = value;
            OnPropertyChanged(nameof(Nodes));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}