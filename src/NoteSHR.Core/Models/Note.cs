﻿using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NoteSHR.Core.Models;

public class Note(double x, double y, string headerColor) : INotifyPropertyChanged
{
    private string _backgroundColor = "#222222";
    private string _headerColor = headerColor;
    private double _width = 200.0d;

    public Guid Id { get; init; } = Guid.NewGuid();

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

    public ObservableCollection<Node> Nodes { get; private set; } = new();

    public string HeaderColor
    {
        get => _headerColor;
        set
        {
            _headerColor = value;
            OnPropertyChanged(nameof(HeaderColor));
        }
    }

    public string BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            OnPropertyChanged(nameof(BackgroundColor));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}