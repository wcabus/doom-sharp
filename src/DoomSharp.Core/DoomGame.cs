using DoomSharp.Core.Data;
using DoomSharp.Core.Graphics;
using DoomSharp.Core.UI;

namespace DoomSharp.Core;

public class DoomGame : IDisposable
{
    private const int Version = 166;
    private static IConsole _console = new NullConsole();

    public static readonly DoomGame Instance = new();

    private readonly List<string> _wadFileNames = new();
    private WadFileCollection? _wadFiles;
    private readonly Video _video = new();
    private readonly Zone _zone = new();
    private Menu? _menu;

    private DoomGame()
    {
        
    }

    public async Task RunAsync()
    {
        try
        {
            IdentifyVersion();

            //_modifiedGame = false;

            var titleFormat = GameMode switch
            {
                GameMode.Retail => "The Ultimate DOOM Startup v{0}.{1}",
                GameMode.Shareware => "DOOM Shareware Startup v{0}.{1}",
                GameMode.Registered => "DOOM Registered Startup v{0}.{1}",
                GameMode.Commercial => "DOOM 2: Hell on Earth v{0}.{1}",
                _ => "Public DOOM - v{0}.{1}"
            };

            _console.SetTitle(string.Format(titleFormat, Version / 100, Version % 100));

            // init subsystems
            _console.WriteLine("V_Init: allocate screens.");
            _video.Initialize();

            _console.WriteLine("M_LoadDefaults: Load system defaults.");
            // M_LoadDefaults();              // load before initing other systems

            _console.WriteLine("Z_Init: Init zone memory allocation daemon.");
            _zone.Initialize();

            _console.WriteLine("W_Init: Init WADfiles.");
            _wadFiles = await WadFileCollection.InitializeMultipleFilesAsync(_wadFileNames);

            switch (GameMode)
            {
                case GameMode.Shareware:
                case GameMode.Indetermined:
                    _console.Write(
                        "===========================================================================" +
                        Environment.NewLine +
                        "                                Shareware!" + Environment.NewLine +
                        "===========================================================================" +
                        Environment.NewLine
                    );
                    break;
                case GameMode.Registered:
                case GameMode.Retail:
                case GameMode.Commercial:
                    _console.Write(
                        "===========================================================================" +
                        Environment.NewLine +
                        "                 Commercial product - do not distribute!" + Environment.NewLine +
                        "         Please report software piracy to the SPA: 1-800-388-PIR8" + Environment.NewLine +
                        "===========================================================================" +
                        Environment.NewLine
                    );
                    break;

                default:
                    // Ouch.
                    break;
            }

            _console.WriteLine("M_Init: Init miscellaneous info.");
            _menu = new Menu();
            // M_Init ();

            _console.Write("R_Init: Init DOOM refresh daemon - ");
            // R_Init ();

            _console.WriteLine(Environment.NewLine + "P_Init: Init Playloop state.");
            // P_Init ();

            _console.WriteLine("I_Init: Setting up machine state.");
            // I_Init ();

            _console.WriteLine("D_CheckNetGame: Checking network game status.");
            // D_CheckNetGame ();

            _console.WriteLine("S_Init: Setting up sound.");
            // S_Init (snd_SfxVolume, snd_MusicVolume);

            _console.WriteLine("HU_Init: Setting up heads up display.");
            // HU_Init();

            _console.WriteLine("ST_Init: Init status bar.");
            // ST_Init();

            if (GameAction != GameAction.LoadGame)
            {
                if (AutoStart /* || NetGame */)
                {
                    // G_InitNew(startskill, startepisode, startmap);
                }
                else
                {
                    // D_StartTitle();                // start up intro loop
                }
            }

            // D_DoomLoop();  // never returns
        }
        catch (DoomErrorException)
        {
            // TODO Cleanup?
        }
    }

    public GameMode GameMode { get; private set; } = GameMode.Indetermined;
    public GameAction GameAction { get; private set; } = GameAction.Nothing;

    public SkillLevel StartSkill { get; set; } = SkillLevel.Medium;
    public int StartEpisode { get; set; } = 1;
    public int StartMap { get; set; } = 1;
    public bool AutoStart { get; set; } = false;

    public static void SetConsole(IConsole console)
    {
        _console = console;
    }

    public static IConsole Console => _console;

    private void IdentifyVersion()
    {
        var doomWadDir = Environment.GetEnvironmentVariable("DOOMWADDIR") ?? ".";

        // Commercial
        var wadFile = Path.Combine(doomWadDir, "doom2.wad");
        if (File.Exists(wadFile))
        {
            GameMode = GameMode.Commercial;
            _wadFileNames.Add(wadFile);
            return;
        }

        // Retail
        wadFile = Path.Combine(doomWadDir, "doomu.wad");
        if (File.Exists(wadFile))
        {
            GameMode = GameMode.Retail;
            _wadFileNames.Add(wadFile);
            return;
        }

        // Registered
        wadFile = Path.Combine(doomWadDir, "doom.wad");
        if (File.Exists(wadFile))
        {
            GameMode = GameMode.Registered;
            _wadFileNames.Add(wadFile);
            return;
        }

        // Shareware
        wadFile = Path.Combine(doomWadDir, "doom1.wad");
        if (File.Exists(wadFile))
        {
            GameMode = GameMode.Shareware;
            _wadFileNames.Add(wadFile);
        }
    }

    public static void Error(string message)
    {
        _console.WriteLine("Error: " + message);
        throw new DoomErrorException();
    }

    public void Dispose()
    {
        _wadFiles?.Dispose();
    }
}