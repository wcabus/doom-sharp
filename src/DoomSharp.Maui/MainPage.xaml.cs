#if ANDROID
using Android.Views;
#endif
#if WINDOWS
using Microsoft.UI.Input;
#endif
using DoomSharp.Core;
using DoomSharp.Core.Input;
using DoomSharp.Maui.Controls;
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
        Microsoft.Maui.Handlers.ImageHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {
            if (view is GameControl control)
            {
#if WINDOWS
                handler.PlatformView.PointerPressed += (s, e) => MainViewModel.Instance.OnKeyAction(control.Key, EventType.KeyDown);
                handler.PlatformView.PointerReleased += (s, e) => MainViewModel.Instance.OnKeyAction(control.Key, EventType.KeyUp);
                

                handler.PlatformView.Holding += (sender, args) =>
             {
                 switch (args.HoldingState)
                 {
                     case HoldingState.Started:
                         MainViewModel.Instance.OnKeyAction(control.Key, EventType.KeyDown);
                         break;
                     case HoldingState.Completed:
                         MainViewModel.Instance.OnKeyAction(control.Key, EventType.KeyUp);
                         break;
                 }
             };

#endif
#if ANDROID
                handler.PlatformView.Touch += (sender, args) =>
                {
                    switch (args.Event?.ActionMasked)
                    {
                        case MotionEventActions.Down:
                            MainViewModel.Instance.OnKeyAction(control.Key, EventType.KeyDown);
                            break;
                        case MotionEventActions.Up:
                            MainViewModel.Instance.OnKeyAction(control.Key, EventType.KeyUp);
                            break;
                    }
                };

#endif
#if IOS

             //handler.PlatformView.UserInteractionEnabled = true;
             //handler.PlatformView.AddGestureRecognizer(new UILongPressGestureRecognizer(HandleLongClick));
#endif
            }
        });

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