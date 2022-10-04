using DoomSharp.Core;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using DoomSharp.Maui.ViewModels;
namespace DoomSharp.Maui;

public partial class MainPage : ContentPage
{
    private SKBitmap? _lastOutput;

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

        App.Locator.MainViewModel.BitmapRendered += OnBitmapRendered;
    }

    private void OnBitmapRendered(object sender, BitmapRenderedEventArgs e)
    {
        _lastOutput = e.Bitmap;
        GameSurface.InvalidateSurface();
    }

    private void SKCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        try
        {
            if (_lastOutput is null)
            {
                return;
            }

            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear();

            var scale = Math.Min((float)info.Width / _lastOutput.Width, (float)info.Height / _lastOutput.Height);
            var x = (info.Width - scale * _lastOutput.Width) / 2;
            var y = (info.Height - scale * _lastOutput.Height) / 2;
            var dest = new SKRect(x, y, x + scale * _lastOutput.Width, y + scale * _lastOutput.Height);

            e.Surface.Canvas.DrawBitmap(_lastOutput, dest);
        }
        finally
        {
            _lastOutput?.Dispose();
            _lastOutput = null;
        }
    }

    private async Task<(GameMode, string)> IdentifyVersion()
    {
        (GameMode, string) version = new();

        // Shareware
        var wadFile = "DOOM1.WAD";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Shareware;
            version.Item2 = wadFile;
            return version;
        }

        // Commercial
        wadFile = "DOOM2.WAD";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Commercial;
            version.Item2 = wadFile;
            return version;
        }

        // Retail
        wadFile = "DOOMU.WAD";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Retail;
            version.Item2 = wadFile;
            return version;
        }

        // Registered
        wadFile = "DOOM.WAD";
        if (await FileSystem.Current.AppPackageFileExistsAsync(wadFile))
        {
            version.Item1 = GameMode.Registered;
            version.Item2 = wadFile;
            return version;
        }

        return version;
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}