using System.Reactive;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using NoteSHR.Core.Helpers;
using NoteSHR.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Image;

public class ImageComponentViewModel : ViewModelBase
{
    [Reactive] public Bitmap Image { get; set; }
    [Reactive] public bool ImageSelected { get; set; }

    public ReactiveCommand<RoutedEventArgs, Unit> SetImageCommand { get; set; }

    public ImageComponentViewModel()
    {
        SetImageCommand = ReactiveCommand.CreateFromTask(async (RoutedEventArgs args) =>
        {
            var topLevel = TopLevel.GetTopLevel(args.Source as Button);
            var fileOptions = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new []
                {
                    FilePickerFileTypes.ImageAll
                }
            };
        
            var file = await topLevel!.StorageProvider.OpenFilePickerAsync(fileOptions);
            if (file.Count == 0)
            {
                return;
            }
            
            Image = ImageHelper.LoadFromFileSystem(file[0].Path);
            ImageSelected = true;
        });
    }
}