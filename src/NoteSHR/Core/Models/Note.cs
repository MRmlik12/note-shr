using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NoteSHR.ViewModels;

namespace NoteSHR.Core.Models;

public class Note(double x, double y) : INotifyPropertyChanged
{
    private ObservableCollection<NodeViewModel> _nodes = new ();
    private double _width = 200.0d;
    
    public Guid Id { get; } = Guid.NewGuid();

    public double X
    {
        get => x;
        set
        {
            x = value;
            OnPropertyChanged(nameof(X));
        }
    }

    public double Y
    {
        get => y;
        set
        {
            y = value;
            OnPropertyChanged(nameof(Y));
        }
    }

    public double Width
    {
        get => _width;
        set
        {
            _width = value;
            OnPropertyChanged(nameof(Width));
        }
    }

    public ObservableCollection<NodeViewModel> Nodes
    {
        get => _nodes;
        private set
        {
            _nodes = value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}