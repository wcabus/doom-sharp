using DoomSharp.AnsiConsole;
using DoomSharp.AnsiConsole.Data;
using DoomSharp.Core;
using DoomSharp.Core.Data;
using DoomSharp.Core.Internals;
using Spectre.Console;
using System;
using System.Text;

//const string GamePanel = "Game";
//const string ConsolePanel = "Console";

//var layout = new Layout();
//layout.SplitRows(
//    new Layout(GamePanel),
//    new Layout(ConsolePanel)
//);

//var gamePanel = layout[GamePanel];
//var consolePanel = layout[ConsolePanel];

//gamePanel.Ratio(3);
//consolePanel.Ratio(1);
//AnsiConsole.Write(layout);

var soundDriver = new SoundDriver();

try
{
    //DoomGame.SetConsole(new DoomConsole());
    DoomGame.SetOutputRenderer(new ConsoleOutputRenderer());
    DoomGame.SetSoundDriver(soundDriver);
    WadFileCollection.Init(new WadStreamProvider());

    Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;
    AnsiConsole.Profile.Width = Constants.ScreenWidth/2;
    AnsiConsole.Profile.Height = Constants.ScreenHeight/2;
    
    await Task.Run(DoomGame.Instance.RunAsync);
}
finally
{
    soundDriver.Dispose();
}