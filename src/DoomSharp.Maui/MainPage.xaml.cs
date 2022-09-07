using DoomSharp.Core;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace DoomSharp.Maui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = App.Locator.MainViewModel;
    }

    protected override void OnAppearing()
    {
        _ = Task.Run(async () =>
        {
            (GameMode, string) doomVersion = await IdentifyVersion();
            await DoomGame.Instance.RunAsync(doomVersion.Item1, doomVersion.Item2);
        });
        App.Locator.MainViewModel.PropertyChanged += Output_PropertyChanged;
    }

    private void Output_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(App.Locator.MainViewModel.Output))
        {
            GameSurface.InvalidateSurface();
        }
    }

    private void SKCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (App.Locator.MainViewModel.Output.ByteCount == 0)
        {
            return;
        }
        
        SKImageInfo resizeInfo = new SKImageInfo((int)GameSurface.Width, (int)GameSurface.Height);
        SKBitmap resizedBitmap = new(resizeInfo);
        App.Locator.MainViewModel.Output.ScalePixels(resizedBitmap, SKFilterQuality.High);
        
        e.Surface.Canvas.DrawBitmap(resizedBitmap, 0, 0);
    }
    
    private async Task<(GameMode, string)> IdentifyVersion()
    {
        (GameMode, string) version = new();
        
        // Commercial
        var wadFile = "doom2.wad";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Commercial;
            version.Item2 = wadFile;
            return version;
        }

        // Retail
        wadFile = "doomu.wad";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Retail;
            version.Item2 = wadFile;
            return version;
        }

        // Registered
        wadFile = "doom.wad";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Registered;
            version.Item2 = wadFile;
            return version;
        }

        // Shareware
        wadFile = "doom1.wad";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Shareware;
            version.Item2 = wadFile;
            return version;
        }

        return version;
    }
}