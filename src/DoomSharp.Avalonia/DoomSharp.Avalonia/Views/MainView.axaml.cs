using System;
using Avalonia.Controls;
using DoomSharp.Avalonia.Model;
using DoomSharp.Avalonia.ViewModels;
using DoomSharp.Core;
using DoomSharp.Core.Input;
using SkiaSharp;
using System.Threading.Tasks;
using Avalonia.Controls.Skia;
using Avalonia.Platform.Storage;
using Avalonia.Platform;

namespace DoomSharp.Avalonia.Views
{
    public partial class MainView : UserControl
    {
        private SKBitmap? _lastOutput;

        public MainView()
        {
            InitializeComponent();
            DataContext = App.Locator.MainViewModel;
            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
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
            GameSurface.InvalidateVisual();
        }

        private void GameSurface_OnDraw(object? sender, SKCanvasEventArgs e)
        {
            try
            {
                if (_lastOutput is null)
                {
                    return;
                }
                
                var canvas = e.Canvas;
                canvas.Clear();

                e.Canvas.DrawBitmap(_lastOutput, canvas.LocalClipBounds);
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

            //var topLevel = TopLevel.GetTopLevel(this);
            //var storage = topLevel.StorageProvider;

            // Shareware
            await using var wadFile = AssetLoader.Open(new Uri("avares://DoomSharp.Avalonia/Assets/DOOM1.WAD"));
            //IStorageFile? wadFile = await storage.TryGetFileFromPathAsync(new Uri("file://DOOM1.WAD"));
            //if (wadFile != null)
            //{
                version.Item1 = GameMode.Shareware;
                version.Item2 = "DOOM1.WAD";
                return version;
            //}

            //// Commercial
            //wadFile = await storage.TryGetFileFromPathAsync(new Uri("file://DOOM2.WAD"));
            //if (wadFile != null)
            //{
            //    version.Item1 = GameMode.Shareware;
            //    version.Item2 = "file://DOOM2.WAD";
            //    return version;
            //}
           
            //// Retail
            //wadFile = await storage.TryGetFileFromPathAsync(new Uri("file://DOOMU.WAD"));
            //if (wadFile != null)
            //{
            //    version.Item1 = GameMode.Shareware;
            //    version.Item2 = "file://DOOMU.WAD";
            //    return version;
            //}

            //// Registered
            //wadFile = await storage.TryGetFileFromPathAsync(new Uri("file://DOOM.WAD"));
            //if (wadFile != null)
            //{
            //    version.Item1 = GameMode.Shareware;
            //    version.Item2 = "file://DOOM.WAD";
            //    return version;
            //}

            //return version;
        }

        private void KeyboardBehavior_OnKeyDown(object? sender, KeyDownEventArgs e)
        {
            MainViewModel.Instance.OnKeyAction(e.Key, EventType.KeyDown);
        }

        private void KeyboardBehavior_OnKeyUp(object? sender, KeyDownEventArgs e)
        {
            MainViewModel.Instance.OnKeyAction(e.Key, EventType.KeyUp);
        }
    }
}