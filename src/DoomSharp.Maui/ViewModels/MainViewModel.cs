using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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

        VirtualKeyDownCommand = new Command<string>(OnVirtualKeyDown);
    }

    private string _title = "DooM#";

    private readonly SKColor[] _palette = new SKColor[256];

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

    public Command<string> VirtualKeyDownCommand { get; private set; }

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
#if ANDROID
            _palette[i] = new SKColor(palette[i * 3 + 2], palette[i * 3 + 1], palette[i * 3]);
#else
            _palette[i] = new SKColor(palette[i * 3], palette[i * 3 + 1], palette[i * 3 + 2]);
#endif
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

    private void OnVirtualKeyDown(string key)
    {
        AddEvent(new InputEvent(EventType.KeyDown, TranslateKey(key), 0, 0));
    }

    private int TranslateKey(string keyIndex)
    {
        Key key = Enum.Parse<Key>(keyIndex);

        switch (key)
        {
            case Key.Left:
                return (int)Keys.LeftArrow;
            case Key.Right:
                return (int)Keys.RightArrow;
            case Key.Down:
                return (int)Keys.DownArrow;
            case Key.Up:
                return (int)Keys.UpArrow;
            case Key.Escape:
                return (int)Keys.Escape;
            case Key.Enter:
                return (int)Keys.Enter;
            case Key.Tab:
                return (int)Keys.Tab;

            case Key.F1:
                return (int)Keys.F1;
            case Key.F2:
                return (int)Keys.F2;
            case Key.F3:
                return (int)Keys.F3;
            case Key.F4:
                return (int)Keys.F4;
            case Key.F5:
                return (int)Keys.F5;
            case Key.F6:
                return (int)Keys.F6;
            case Key.F7:
                return (int)Keys.F7;
            case Key.F8:
                return (int)Keys.F8;
            case Key.F9:
                return (int)Keys.F9;
            case Key.F10:
                return (int)Keys.F10;
            case Key.F11:
                return (int)Keys.F11;
            case Key.F12:
                return (int)Keys.F12;

            case Key.Back:
            case Key.Delete:
                return (int)Keys.Backspace;

            case Key.Pause:
                return (int)Keys.Pause;

            case Key.OemPlus:
                return (int)Keys.Equals;

            case Key.OemMinus:
                return (int)Keys.Minus;

            case Key.LeftShift:
            case Key.RightShift:
                return (int)Keys.RShift;

            case Key.LeftCtrl:
            case Key.RightCtrl:
                return (int)Keys.RCtrl;

            case Key.RightAlt:
            case Key.LeftAlt:
                return (int)Keys.RAlt;

            //case >= Key.D0 and <= Key.D9:
            //    return '0' + ((int)keyIndex - (int)Key.D0);

            //case >= Key.NumPad0 and <= Key.NumPad9:
            //    return '0' + (keyIndex - (int)Key.NumPad0);

            //case >= Key.A and <= Key.Z:
            //    return 'A' + (keyIndex - (int)Key.A);

            case Key.Space:
                return ' ';

            default:
                return 0;
        }
    }
}