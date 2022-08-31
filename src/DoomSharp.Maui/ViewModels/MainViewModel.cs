using System.ComponentModel;
using System.Runtime.CompilerServices;
using DoomSharp.Core;
using DoomSharp.Core.Graphics;
using DoomSharp.Core.Input;
using DoomSharp.Maui.Model;
using SkiaSharp;
using Constants = DoomSharp.Core.Constants;

namespace DoomSharp.Maui.ViewModels;

public class MainViewModel : INotifyPropertyChanged, IGraphics
{
    public static readonly MainViewModel Instance = new();

    private MainViewModel()
    {
        _stride = (_rectangle.Width * 8 /* bpp */ + 7) / 8;
        _screenBuffer = new byte[_rectangle.Height * _stride];
    }

    private string _title = "DooM#";

    private SKBitmap? _output;
    private SKBitmap? _newPaletteOutput;

    private readonly Int32Rect _rectangle = new(0, 0, Constants.ScreenWidth, Constants.ScreenHeight);
    private readonly int _stride;
    private readonly byte[] _screenBuffer;
    private readonly Queue<InputEvent> _events = new();

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public SKBitmap? Output
    {
        get => _output;
        set
        {
            if (value is null)
            {
                return;
            }

            _output = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Initialize()
    {
        
    }

    public void UpdatePalette(byte[] palette)
    {
        var colors = new List<Color>(256);
        for (var i = 0; i < 256*3; i += 3)
        {
            colors.Add(Color.FromRgb(palette[i], palette[i + 1], palette[i + 2]));
        }
        //TODO
        //var bitmapPalette = new BitmapPalette(colors);

        //MainThread.BeginInvokeOnMainThread(() =>
        //{
        //    _newPaletteOutput = new SKBitmap(Constants.ScreenWidth, Constants.ScreenHeight, 96, 96, PixelFormats.Indexed8, bitmapPalette);
        //});
    }

    public void ScreenReady(byte[] output)
    {
        Array.Copy(output, 0, _screenBuffer, 0, output.Length);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var bitmap = _output;
            var switchOutput = false;

            if (_newPaletteOutput is not null)
            {
                bitmap = _newPaletteOutput;
                _newPaletteOutput = null;
                switchOutput = true;
            }

            if (bitmap is null)
            {
                return;
            }

            bitmap = SKBitmap.Decode(output);
            //bitmap.WritePixels(_rectangle, _screenBuffer, _stride, 0);
            if (switchOutput)
            {
                Output = bitmap;
            }
        });
    }

    public void StartTic()
    {
        while (_events.TryDequeue(out var ev)) // has events
        {
            DoomGame.Instance.PostEvent(ev);
        }
    }

    public void AddEvent(InputEvent ev)
    {
        _events.Enqueue(ev);
    }
}