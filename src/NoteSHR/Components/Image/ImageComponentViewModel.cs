using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using NoteSHR.Core.Helpers;
using NoteSHR.Core.Models;
using NoteSHR.Core.Utils;
using NoteSHR.Core.ViewModel;
using NoteSHR.File;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NoteSHR.Components.Image;

public class ImageComponentViewModel : ViewModelBase, IDataPersistence
{
    public ImageComponentViewModel()
    {
        SetImageCommand = ReactiveCommand.CreateFromTask(async (RoutedEventArgs args) =>
        {
            if (OperatingSystem.IsBrowser())
            {
                var url = await App.FileService.GetFileUrl(FilePickerFileTypes.ImageAll.Patterns!.Select(x => x.Remove(0, 1)).ToArray());
                if (string.IsNullOrEmpty(url)) return;

                Image = await HttpUtils.GetBitmapFromUrl(url);
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

        this.WhenAnyValue(vm => vm.Image)
            .Subscribe(image =>
            {
                if (image != null) ImageSelected = true;
            });
    }

    [Reactive] public Bitmap? Image { get; set; }
    [Reactive] public bool ImageSelected { get; set; }

    public ReactiveCommand<RoutedEventArgs, Unit> SetImageCommand { get; set; }

    public object ExportValues()
    {
        var stream = new MemoryStream();
        Image?.Save(stream);

        return new 
        {
            Image = new FileBlob(stream)
        };
    }

    public void ConvertValues(Dictionary<string, object> data)
    {
        if (data.TryGetValue("Image", out var image))
        {
            Image = new Bitmap((image as FileBlob).Read());
        }
    }
}