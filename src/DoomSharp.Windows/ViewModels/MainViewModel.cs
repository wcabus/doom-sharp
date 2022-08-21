using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DoomSharp.Core;
using DoomSharp.Core.Graphics;
using DoomSharp.Core.Input;
using DoomSharp.Windows.Annotations;
using Microsoft.VisualBasic;
using Constants = DoomSharp.Core.Constants;

namespace DoomSharp.Windows.ViewModels;

public class MainViewModel : INotifyPropertyChanged, IGraphics
{
    public static readonly MainViewModel Instance = new();

    private MainViewModel()
    {
        _stride = (_rectangle.Width * 8 /* bpp */ + 7) / 8;
        _screenBuffer = new byte[_rectangle.Height * _stride];
    }

    private string _title = "DooM#";

    private WriteableBitmap? _output;
    private WriteableBitmap? _newPaletteOutput;

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

    public WriteableBitmap? Output
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

    [NotifyPropertyChangedInvocator]
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
        
        var bitmapPalette = new BitmapPalette(colors);

        Application.Current.Dispatcher.Invoke(() =>
        {
            _newPaletteOutput = new WriteableBitmap(Constants.ScreenWidth, Constants.ScreenHeight, 96, 96, PixelFormats.Indexed8, bitmapPalette);
        });
    }

    public void ScreenReady(byte[] output)
    {
        var bufferIdx = 0;
        foreach (var pixel in output)
        {
            _screenBuffer[bufferIdx++] = pixel;
        }

        Application.Current.Dispatcher.Invoke(() =>
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

            bitmap.WritePixels(_rectangle, _screenBuffer, _stride, 0);
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