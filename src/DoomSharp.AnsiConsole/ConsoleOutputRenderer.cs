using DoomSharp.Core;
using DoomSharp.Core.Graphics;
using Spectre.Console;

namespace DoomSharp.AnsiConsole;

public class ConsoleOutputRenderer : IGraphics
{
    private Canvas? _canvas;
    private Color[]? _palette;

    public void Initialize()
    {
        _canvas = new Canvas(Constants.ScreenWidth, Constants.ScreenHeight)
        {
            PixelWidth = 1,
            Scale = true
        };

        Spectre.Console.AnsiConsole.Write(_canvas);
    }

    public void UpdatePalette(byte[] palette)
    {
        var colors = new List<Color>(256);
        for (var i = 0; i < 256 * 3; i += 3)
        {
            colors.Add(new Color(palette[i], palette[i + 1], palette[i + 2]));
        }

        _palette = colors.ToArray();
    }

    public void ScreenReady(byte[] output)
    {
        if (_canvas is null || _palette is null)
        {
            return;
        }

        var x = 0;
        var y = 0;
        foreach (var colorIndex in output)
        {
            _canvas.SetPixel(x++, y, _palette[colorIndex]);

            if (x >= Constants.ScreenWidth)
            {
                x = 0;
                y++;
            }
        }

        Spectre.Console.AnsiConsole.Clear();
        Spectre.Console.AnsiConsole.Write(_canvas);
    }

    public void StartTic()
    {
        // TODO: hook up console input and dispatch events to DoomGame
    }
}