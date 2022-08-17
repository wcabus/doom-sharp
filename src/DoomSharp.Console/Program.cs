using DoomSharp.Console;
using DoomSharp.Core;

DoomGame.SetConsole(new WinConsole());

try
{
    await DoomGame.Instance.RunAsync();
}
finally
{
    DoomGame.Instance.Dispose();
}

Console.ReadLine();