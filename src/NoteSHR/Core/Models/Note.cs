using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DynamicData;
using NoteSHR.ViewModels;

namespace NoteSHR.Core.Models;

public class Note(Guid id, double x, double y) : INotifyPropertyChanged
{
    private double _x = x;
    private double _y = y;
    private List<(Guid, Type, ViewModelBase)> _nodes = [];
    
    public Guid Id { get; set; } = id;

    public double X
    {
        get { return _x; }
        set
        {
            _x = value;
            OnPropertyChanged(nameof(X));
        }
    }
    public double Y
    {
        get { return _y; }
        set
        {
            _y = value;
            OnPropertyChanged(nameof(Y));
        }
    }

    public List<(Guid, Type, ViewModelBase)> Nodes
    {
        get { return _nodes; }
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