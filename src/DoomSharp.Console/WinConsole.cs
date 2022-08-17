using DoomSharp.Core;
using Con=System.Console;

namespace DoomSharp.Console;

public class WinConsole : IConsole
{
    public void Write(string message)
    {
        Con.Write(message);
    }

    public void SetTitle(string title)
    {
        Con.Title = title;
        Write(title);
        Write(Environment.NewLine);
    }
}