using DoomSharp.Core;
using Spectre.Console;

namespace DoomSharp.AnsiConsole;

public class DoomConsole : IConsole
{
    //private readonly Panel _panel;
    //private readonly LiveDisplay _liveUpdate;

    public DoomConsole(/*Layout zone*/)
    {
        //Spectre.Console.AnsiConsole.Live().Start();
        //_liveUpdate = new LiveDisplay(Spectre.Console.AnsiConsole.Console, zone);
        //// _panel = new Panel(_liveUpdate).Expand();
        //zone.Update(_liveUpdate.);
    }

    public void Write(string message)
    {
        //_liveUpdate.Update(message);
    }

    public void SetTitle(string title)
    {
        //_panel.Header(title, Justify.Center);
    }

    public void Shutdown()
    {
    }
}