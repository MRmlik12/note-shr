using Avalonia.Controls;

namespace NoteSHR.Components.Image;

public partial class ImageComponent : UserControl
{
    public ImageComponent(ImageComponentViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}