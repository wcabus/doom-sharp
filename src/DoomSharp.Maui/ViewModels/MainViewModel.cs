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

    private SKColor[] _palette = new SKColor[256];

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

    public event PropertyChangedEventHandler? PropertyChanged;
    public event BitmapRenderedEventHandler BitmapRendered;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Initialize()
    {
    }

    public void UpdatePalette(byte[] palette)
    {
        for (var i = 0; i < 256; i++)
        {
            _palette[i] = new SKColor(palette[i * 3], palette[i * 3 + 1], palette[i * 3 + 2]);
        }
    }

    public void ScreenReady(byte[] output)
    {
        Array.Copy(output, 0, _screenBuffer, 0, output.Length);

        var bitmap = new SKBitmap(Constants.ScreenWidth, Constants.ScreenHeight);
        var pixelsPtr = bitmap.GetPixels();

        unsafe
        {
            uint* ptr = (uint*)pixelsPtr.ToPointer();
            for (var x = 0; x < Constants.ScreenWidth; x++)
            {
                for (var y = 0; y < Constants.ScreenHeight; y++)
                {
                    var pixelIdx = x * Constants.ScreenHeight + y;
                    *ptr++ = (uint)_palette[_screenBuffer[pixelIdx]];
                }
            }
        }

        App.Current.Dispatcher.Dispatch(() =>
        {
            BitmapRendered?.Invoke(this, new BitmapRenderedEventArgs(bitmap));
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