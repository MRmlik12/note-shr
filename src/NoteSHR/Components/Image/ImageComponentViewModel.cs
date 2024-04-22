using System;
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
    public ImageComponentViewModel()
    {
        SetImageCommand = ReactiveCommand.CreateFromTask(async (RoutedEventArgs args) =>
        {
            if (OperatingSystem.IsBrowser())
            {
                await App.FilePicker.Open();
                return;
            }
            
            var topLevel = TopLevel.GetTopLevel(args.Source as Button);
            var fileOptions = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    FilePickerFileTypes.ImageAll
                }
            };

            var file = await topLevel!.StorageProvider.OpenFilePickerAsync(fileOptions);
            if (file.Count == 0) return;

            Image = await ImageHelper.LoadFromFileSystem(file[0]);
            ImageSelected = true;
        });
    }

    [Reactive] public Bitmap Image { get; set; }
    [Reactive] public bool ImageSelected { get; set; }

    public ReactiveCommand<RoutedEventArgs, Unit> SetImageCommand { get; set; }
}