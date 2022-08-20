using DoomSharp.Console;
using DoomSharp.Core;

DoomGame.SetConsole(new WinConsole());
DoomGame.SetOutputRenderer(new BitmapGraphics());

try
{
    await DoomGame.Instance.RunAsync();
}
finally
{
    DoomGame.Instance.Dispose();
}

Console.ReadLine();