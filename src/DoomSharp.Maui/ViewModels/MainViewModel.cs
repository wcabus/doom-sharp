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
        var stride = (_rectangle.Width * 8 /* bpp */ + 7) / 8;
        _screenBuffer = new byte[_rectangle.Height * stride];
    }

    private string _title = "DooM#";

    private byte[] _palette;
    private SKBitmap _output = new();

    private readonly Int32Rect _rectangle = new(0, 0, Constants.ScreenWidth, Constants.ScreenHeight);
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

    public SKBitmap Output
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
        _palette = palette;
    }

    public void ScreenReady(byte[] output)
    {
        Array.Copy(output, 0, _screenBuffer, 0, output.Length);

        var switchOutput = false;

        byte[] palette = null;
        if (_palette is not null)
        {
            palette = _palette;
            _palette = null;
            switchOutput = true;
        }

        if (!switchOutput)
        {
            return;
        }

        SKBitmap bitmap = new(new SKImageInfo(Constants.ScreenWidth, Constants.ScreenHeight));
        for (var x = 0; x < Constants.ScreenWidth; x++)
        {
            for (var y = 0; y < Constants.ScreenHeight; y++)
            {
                var pixelIdx = y * Constants.ScreenWidth + x;
                byte r = palette[_screenBuffer[pixelIdx] * 3];
                byte g = palette[_screenBuffer[pixelIdx] * 3 + 1];
                byte b = palette[_screenBuffer[pixelIdx] * 3 + 2];

                bitmap.SetPixel(x, y, new SKColor(r, g, b));
            }
        }

        App.Current.Dispatcher.Dispatch(() =>
        {
            Output = bitmap;
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