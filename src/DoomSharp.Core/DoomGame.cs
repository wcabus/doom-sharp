using DoomSharp.Core.Data;
using DoomSharp.Core.Graphics;
using DoomSharp.Core.Input;
using DoomSharp.Core.UI;

namespace DoomSharp.Core;

public class DoomGame : IDisposable
{
    private const int Version = 166;
    private static IConsole _console = new NullConsole();
    private static IGraphics _graphics = new NullGraphics();

    public static readonly DoomGame Instance = new();

    private readonly List<string> _wadFileNames = new();
    private WadFileCollection? _wadFiles;
    private readonly Video _video = new(_graphics);
    private readonly Zone _zone = new();

    private readonly GameController _game = new();
    private MenuController? _menu;
    private HudController? _hud;

    private bool _singleTics = false; // debug flag to cancel adaptiveness

    private int[] _frametics = new int[4];
    private int _frameOn;
    private int[] _frameskip = new int[4];
    private int _oldnettics;
    private int _oldEnterTics = 0;
    
    private long _baseTime = 0;
    private int _skiptics = 0;
    private int _gametime;

    private Queue<InputEvent> _events = new(Constants.MaxEvents);

    // Flag: true only if started as net deathmatch.
    // An enum might handle altdeath/cooperative better.
    private bool _deathmatch = false;

    private int _demoSequence;
    private bool _advancedemo;
    private int _demoPageTic = 0;
    private string _demoPageName = "";

    private GameState _oldDisplayGameState = GameState.Wipe;
    

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
            _wadFiles = WadFileCollection.InitializeMultipleFiles(_wadFileNames);

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
            _menu = new MenuController();
            
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
            _hud = new HudController();

            _console.WriteLine("ST_Init: Init status bar.");
            // ST_Init();

            if (GameAction != GameAction.LoadGame)
            {
                if (AutoStart || NetGame)
                {
                    // G_InitNew(startskill, startepisode, startmap);
                }
                else
                {
                    StartTitle(); // start up intro loop
                }
            }

            DoomLoop();  // never returns
        }
        catch (DoomErrorException)
        {
            // TODO Cleanup?
        }
    }

    public GameMode GameMode { get; private set; } = GameMode.Indetermined;
    public GameAction GameAction { get; set; } = GameAction.Nothing;
    public GameState GameState { get; set; } = GameState.Wipe;
    public GameState WipeGameState { get; private set; } = GameState.Wipe;
    public GameLanguage Language { get; private set; } = GameLanguage.English;

    public SkillLevel StartSkill { get; set; } = SkillLevel.Medium;
    public int StartEpisode { get; set; } = 1;
    public int StartMap { get; set; } = 1;
    public bool AutoStart { get; set; } = false;
    public bool NetGame { get; set; } = false; // Netgame? Only true if >1 player.

    /// <summary>
    /// gametic is the tic about to (or currently being) run
    /// </summary>
    public int GameTic { get; private set; } = 0;

    public int TicDup { get; private set; } = 1; // tic duplication // 1 = no duplication, 2-5 = dup for slow nets

    /// <summary>
    /// maketic is the tick that hasn't had control made for it yet
    /// </summary>
    public int MakeTic { get; private set; } = 0;

    public int[] NetTics { get; } = new int[Constants.MaxNetNodes];

    public bool StatusBarActive { get; set; } = false;

    public bool AutoMapActive { get; set; } = false;   // In AutoMap mode?
    public bool MenuActive { get; set; } = false;  // Menu overlayed?
    public bool InHelpScreensActive { get; set; } = false;
    public bool Paused { get; set; } = false;		// Game Pause?

    public Video Video => _video;
    public HudController Hud => _hud!;
    public WadFileCollection WadData => _wadFiles!;

    public static void SetConsole(IConsole console)
    {
        _console = console;
    }

    public static void SetOutputRenderer(IGraphics renderer)
    {
        _graphics = renderer;
        Instance.Video.SetOutputRenderer(renderer);
    }

    public static IConsole Console => _console;

    private void Display()
    {
        var y = 0;

        switch (GameState)
        {
            case GameState.Level:
                if (GameTic == 0)
                {
                    break;
                }

                if (AutoMapActive)
                {
                    // AM_Drawer();
                }

                //if (wipe || (viewheight != 200 && fullscreen))
                //    redrawsbar = true;
                
                //if (inhelpscreensstate && !inhelpscreens)
                //    redrawsbar = true;              // just put away the help screen
                
                //ST_Drawer(viewheight == 200, redrawsbar);
                //fullscreen = viewheight == 200;
                break;

            case GameState.Intermission:
                // WI_Drawer();
                break;
            case GameState.Finale:
                // F_Drawer();
                break;
            case GameState.DemoScreen:
                PageDrawer();
                break;
        }

        // draw buffered stuff to screen
        // I_UpdateNoBlit(); // Doesn't do anything

        // draw the view directly
        if (GameState == GameState.Level && !AutoMapActive && GameTic != 0)
        {
            // R_RenderPlayerView(&players[displayplayer]);
        }

        if (GameState == GameState.Level && GameTic != 0)
        {
            // HU_Drawer();
        }

        // clean up border stuff
        if (GameState != _oldDisplayGameState && GameState != GameState.Level)
        {
            _video.SetPalette("PLAYPAL");
        }

        // see if the border needs to be initially drawn
        if (GameState == GameState.Level && _oldDisplayGameState != GameState.Level)
        {
            // viewactivestate = false;        // view was not active
            // R_FillBackScreen();    // draw the pattern into the back screen
        }

        // see if the border needs to be updated to the screen
        if (GameState == GameState.Level && !AutoMapActive /*&& scaledviewwidth != 320*/)
        {
            if (_menu!.IsActive || MenuActive /* || !viewactivestate*/)
            {
                // borderdrawcount = 3;
            }
            //if (borderdrawcount)
            //{
            //    R_DrawViewBorder();    // erase old menu stuff
            //    borderdrawcount--;
            //}
        }

        MenuActive = _menu!.IsActive;
        // viewactivestate = viewactive;
        InHelpScreensActive = _menu!.InHelpScreens;
        _oldDisplayGameState /*= wipegamestate*/ = GameState;

        // draw pause pic
        if (Paused)
        {
            if (AutoMapActive)
            {
                y = 4;
            }
            else
            {
                y = /*viewwindowy +*/ 4;
            }

            _video.DrawPatchDirect(/*viewwindowx + (scaledviewwidth - 68) / 2*/ 0, y, 0, WadData.GetLumpName("M_PAUSE", PurgeTag.Cache));
        }

        // menus go directly to the screen
        _menu!.Drawer(); // menu is drawn even on top of everything
        NetUpdate(); // send out any new accumulation

        // normal update
        //if (!wipe)
        //{
            _video.SignalOutputReady(); // page flip or blit buffer
            return;
        //}

        //// wipe update
        //wipe_EndScreen(0, 0, SCREENWIDTH, SCREENHEIGHT);

        //wipestart = I_GetTime() - 1;

        //do
        //{
        //    do
        //    {
        //        nowtime = I_GetTime();
        //        tics = nowtime - wipestart;
        //    } while (!tics);
        //    wipestart = nowtime;
        //    done = wipe_ScreenWipe(wipe_Melt
        //        , 0, 0, SCREENWIDTH, SCREENHEIGHT, tics);
        //    I_UpdateNoBlit();
        //    M_Drawer();                            // menu is drawn even on top of wipes
        //    I_FinishUpdate();                      // page flip or blit buffer
        //} while (!done);
    }

    private void DoomLoop()
    {
        // demo recording
        // debug
        _graphics.Initialize();

        while (true)
        {
            //// frame syncronous IO operations
            //I_StartFrame(); // not needed

            //// process one or more tics
            if (_singleTics)
            {
                //I_StartTic();
                //D_ProcessEvents();
                //G_BuildTiccmd(&netcmds[consoleplayer][MakeTic % BACKUPTICS]);
                //if (_advancedemo)
                //{
                //    DoAdvanceDemo();
                //}

                //_menu!.Ticker();

                _game.Ticker();
                GameTic++;
                MakeTic++;
            }
            else
            {
                TryRunTics(); // will run at least one tic
            }

            //S_UpdateSounds(players[consoleplayer].mo);// move positional sounds

            // Update display, next frame, with current state.
            Display();

            //# ifndef SNDSERV
            //            // Sound mixing for the buffer is snychronous.
            //            I_UpdateSound();
            //#endif
            //            // Synchronous sound output is explicitly called.
            //# ifndef SNDINTR
            //            // Update sound output.
            //            I_SubmitSound();
            //#endif
        }
    }

    private void TryRunTics()
    {
        var enterTic = GetTime() / TicDup;
        var realTics = enterTic - _oldEnterTics;
        _oldEnterTics = enterTic;

        NetUpdate();

        var lowTic = int.MaxValue;
        var numplaying = 0;
        //for (var i = 0; i < doomcom->numnodes; i++)
        //{
        //    if (nodeingame[i])
        //    {
        //        numplaying++;
        //        if (nettics[i] < lowtic)
        //            lowtic = nettics[i];
        //    }
        //}
        var availableTics = lowTic - GameTic / TicDup;

        // decide how many tics to run
        var counts = 0;
        if (realTics < availableTics - 1)
        {
            counts = realTics + 1;
        }
        else if (realTics < availableTics)
        {
            counts = realTics;
        }
        else
        {
            counts = availableTics;
        }

        if (counts < 1)
        {
            counts = 1;
        }

        _frameOn++;

        //if (debugfile)
        //    fprintf(debugfile,
        //        "=======real: %i  avail: %i  game: %i\n",
        //        realtics, availabletics, counts);

        //if (!demoplayback)
        //{
        //    // ideally nettics[0] should be 1 - 3 tics above lowtic
        //    // if we are consistantly slower, speed up time
        //    for (i = 0; i < MAXPLAYERS; i++)
        //        if (playeringame[i])
        //            break;
        //    if (consoleplayer == i)
        //    {
        //        // the key player does not adapt
        //    }
        //    else
        //    {
        //        if (nettics[0] <= nettics[nodeforplayer[i]])
        //        {
        //            _gametime--;
        //            // printf ("-");
        //        }
        //        frameskip[frameon & 3] = (oldnettics > nettics[nodeforplayer[i]]);
        //        oldnettics = nettics[0];
        //        if (frameskip[0] && frameskip[1] && frameskip[2] && frameskip[3])
        //        {
        //            skiptics = 1;
        //            // printf ("+");
        //        }
        //    }
        //}// demoplayback

        // wait for new tics if needed
        while (lowTic < (GameTic / TicDup + counts))
        {
            NetUpdate();
            lowTic = int.MaxValue;

            //for (var i = 0; i < doomcom->numnodes; i++)
            //{
            //    if (nodeingame[i] && nettics[i] < lowtic)
            //    {
            //        lowtic = nettics[i];
            //    }
            //}

            if (lowTic < (GameTic / TicDup))
            {
                Error("TryRunTics: lowtic < gametic");
            }

            // don't stay in here forever -- give the menu a chance to work
            if ((GetTime() / TicDup - enterTic) >= 20)
            {
                _menu!.Ticker();
                return;
            }
        }

        // run the count * ticdup dics
        while (counts-- > 0)
        {
            for (var i = 0; i < TicDup; i++)
            {
                if ((GameTic / TicDup) > lowTic)
                {
                    Error("gametic>lowtic");
                }

                if (_advancedemo)
                {
                    DoAdvanceDemo();
                }

                _menu!.Ticker();
                _game.Ticker();
                GameTic++;

                // modify command for duplicated tics
                if (i != TicDup - 1)
                {
                    //ticcmd_t* cmd;
                    //int buf;
                    //int j;

                    //buf = (gametic / ticdup) % BACKUPTICS;
                    //for (j = 0; j < MAXPLAYERS; j++)
                    //{
                    //    cmd = &netcmds[j][buf];
                    //    cmd->chatchar = 0;
                    //    if (cmd->buttons & BT_SPECIAL)
                    //        cmd->buttons = 0;
                    //}
                }
            }
            NetUpdate();    // check for new console commands
        }
    }

    private void NetUpdate()
    {
        // check time
        var nowtime = GetTime() / TicDup;
        var newtics = nowtime - _gametime;
        _gametime = nowtime;

        if (newtics <= 0) // nothing new to update
        {
            // listen for other packets
            GetPackets();
            return;
        }

        if (_skiptics <= newtics)
        {
            newtics -= _skiptics;
            _skiptics = 0;
        }
        else
        {
            _skiptics -= newtics;
            newtics = 0;
        }

        // netbuffer->player = consoleplayer;

        // build new ticcmds for console player
        var gameTicDiv = GameTic / TicDup;
        for (var i = 0; i < newtics; i++)
        {
            _graphics.StartTic();
            ProcessEvents();
            if (MakeTic - gameTicDiv >= (Constants.BackupTics / 2 - 1))
            {
                break;          // can't hold any more
            }

            // G_BuildTiccmd(&localcmds[maketic % BACKUPTICS]);
            MakeTic++;
        }

        if (_singleTics)
        {
            return; // singletic update is synchronous
        }

        // send the packet to the other nodes
        // UNSUPPORTED

        // listen for other packets
        GetPackets();
    }

    private void GetPackets()
    {
        // UNSUPPORTED?
    }

    public void PostEvent(InputEvent ev)
    {
        _events.Enqueue(ev);
    }

    private void ProcessEvents()
    {
        // process events and dispatch them to the menu and the game logic (the latter only if the menu didn't eat the event)

        // IF STORE DEMO, DO NOT ACCEPT INPUT
        if (GameMode == GameMode.Commercial && WadData.GetNumForName("map01") < 0)
        {
            return;
        }

        while (_events.TryDequeue(out var currentEvent))
        {
            if (_menu!.HandleEvent(currentEvent))
            {
                continue;
            }

            // _game.HandleEvent(currentEvent);
        }
    }

    public int GetTime()
    {
        var timeSinceEpoch = DateTime.UtcNow - DateTime.UnixEpoch;
        var secondsSinceEpoch = (int)timeSinceEpoch.TotalSeconds;
        if (_baseTime == 0)
        {
            _baseTime = secondsSinceEpoch;
        }

        return (int)((secondsSinceEpoch - _baseTime) * Constants.TicRate + timeSinceEpoch.Milliseconds * Constants.TicRate / 1000f);
    }

    private void StartTitle()
    {
        GameAction = GameAction.Nothing;
        _demoSequence = -1;
        AdvanceDemo();
    }

    private void AdvanceDemo()
    {
        _advancedemo = true;
    }

    private void DoAdvanceDemo()
    {
        // players[consoleplayer].playerstate = PST_LIVE;  // not reborn
        _advancedemo = false;
        // usergame = false;               // no save / end game here
        Paused = false;
        GameAction = GameAction.Nothing;

        if (GameMode == GameMode.Retail)
        {
            _demoSequence = (_demoSequence + 1) % 7;
        }
        else
        {
            _demoSequence = (_demoSequence + 1) % 6;
        }

        switch (_demoSequence)
        {
            case 0:
                if (GameMode == GameMode.Commercial)
                {
                    _demoPageTic = 35 * 11;
                }
                else
                {
                    _demoPageTic = 170;
                }
                GameState = GameState.DemoScreen;
                _demoPageName = "TITLEPIC";
                if (GameMode == GameMode.Commercial)
                {
                    // S_StartMusic(mus_dm2ttl);
                }
                else
                {
                    // S_StartMusic(mus_intro);
                }
                break;
            case 1:
                _demoPageTic = 200; // todo remove
                _game.DeferedPlayDemo("demo1");
                break;
            case 2:
                _demoPageTic = 200;
                GameState = GameState.DemoScreen;
                _demoPageName = "CREDIT";
                break;
            case 3:
                _demoPageTic = 200; // todo remove
                _game.DeferedPlayDemo("demo2");
                break;
            case 4:
                GameState = GameState.DemoScreen;
                if (GameMode == GameMode.Commercial)
                {
                    _demoPageTic = 35 * 11;
                    _demoPageName = "TITLEPIC";
                    // S_StartMusic(mus_dm2ttl);
                }
                else
                {
                    _demoPageTic = 200;

                    if (GameMode == GameMode.Retail)
                    {
                        _demoPageName = "CREDIT";
                    }
                    else
                    {
                        _demoPageName = "HELP2";
                    }
                }
                break;
            case 5:
                _demoPageTic = 200; // todo remove
                _game.DeferedPlayDemo("demo3");
                break;
            // THE DEFINITIVE DOOM Special Edition demo
            case 6:
                _demoPageTic = 200; // todo remove
                _game.DeferedPlayDemo("demo4");
                break;
        }
    }

    private void PageDrawer()
    {
        _video.DrawPatch(0, 0, 0, WadData.GetLumpName(_demoPageName, PurgeTag.Cache));
    }

    public void PageTicker()
    {
        if (--_demoPageTic < 0)
        {
            AdvanceDemo();
        }
    }

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