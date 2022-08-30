using DoomSharp.Core.Data;
using DoomSharp.Core.Input;
using DoomSharp.Core.Networking;
using DoomSharp.Core.Graphics;
using System.Threading;

namespace DoomSharp.Core.GameLogic;

public class GameController
{
    public static readonly int[] MaxAmmo = { 200, 50, 300, 50 };
    public static readonly int[] ClipAmmo = { 10, 4, 20, 1 };

    private int _saveGameSlot = 0;
    private string _saveDescription = "";

    private byte[] _demoData = Array.Empty<byte>();
    private int _demoDataIdx = -1;
    private int _demoEnd = -1;
    private SkillLevel _d_skill;
    private int _d_episode;
    private int _d_map;

    private WorldMapInfo _wmInfo = new();

    private readonly short[][] _consistency = new short[Constants.MaxPlayers][];

    // 
    // controls (have defaults) 
    // 
    private int _keyRight = 'E';
    private int _keyLeft = 'Q';

    private int _keyUp = 'W';
    private int _keyDown = 'S';
    private int _keyStrafeLeft = 'A';
    private int _keyStrafeRight = 'D';
    private int _keyFire = ' ';
    private int _keyUse = (int)Keys.Enter;
    private int _keyStrafe = (int)Keys.LAlt;
    private int _keySpeed = (int)Keys.RShift;

    private int _mouseButtonFire = 0;
    private int _mouseButtonStrafe = 2;
    private int _mouseButtonForward = 1;

    private int _joyButtonFire = 0;
    private int _joyButtonStrafe = 1;
    private int _joyButtonUse = 2;
    private int _joyButtonSpeed = 3;

    public const int TurboThreshold = 0x32;

    private static readonly Fixed[] ForwardMove = { 0x19, 0x32 };
    private static readonly Fixed[] SideMove = { 0x18, 0x28 };
    private static readonly Fixed[] AngleTurn = { 640, 1280, 320 };

    public const int SlowTurnTics = 6;
    public const int NumKeys = 256;

    private readonly bool[] _gameKeyDown = new bool[NumKeys];
    private int _turnHeld;

    private readonly bool[] _mouseButtons = new bool[4];

    // mouse values are used once 
    private int _mouseX;
    private int _mouseY;

    private int _doubleClickTime;
    private bool _doubleClickState;
    private int _doubleClicks;
    private int _doubleClickTime2;
    private bool _doubleClickState2;
    private int _doubleClicks2;

    // joystick values are repeated 
    private int _joyXMove;
    private int _joyYMove;
    private bool[] _joyButtons = new bool[5];

    public int LevelTime { get; private set; }
    private LinkedList<Thinker> _thinkers = new();

    public const int BodyQueueSize = 32;
    private MapObject[] _bodyQueue = new MapObject[BodyQueueSize];
    private int _bodyQueueSlot = 0;

    //
    // MAP related Lookup tables.
    // Store VERTEXES, LINEDEFS, SIDEDEFS, etc.
    //
    private int _numVertices;
    private Vertex[] _vertices = Array.Empty<Vertex>();

    private int _numSegments;
    private Segment[] _segments = Array.Empty<Segment>();

    private int _numSectors;
    private Sector[] _sectors = Array.Empty<Sector>();

    private int _numSubSectors;
    private SubSector[] _subSectors = Array.Empty<SubSector>();

    private int _numNodes;
    private Node[] _nodes = Array.Empty<Node>();

    private int _numLines;
    private Line[] _lines = Array.Empty<Line>();

    private int _numSides;
    private SideDef[] _sides = Array.Empty<SideDef>();

    // BLOCKMAP
    // Created from axis aligned bounding box
    // of the map, a rectangular array of
    // blocks of size ...
    // Used to speed up collision detection
    // by spatial subdivision in 2D.
    //
    // Blockmap size.
    private int _blockMapWidth;
    private int _blockMapHeight; // size in map blocks
    private short[] _blockMap = Array.Empty<short>(); // int for larger maps TODO ????
    // offsets in blockmap are from here
    private short[] _blockMapLump = Array.Empty<short>();
    // origin of block map
    private Fixed _blockMapOriginX = Fixed.Zero;
    private Fixed _blockMapOriginY = Fixed.Zero;
    // For thing chains
    private MapObject?[] _blockLinks = Array.Empty<MapObject?>();

    // REJECT
    // For fast sight rejection.
    // Speeds up enemy AI by skipping detailed
    //  LineOf Sight calculation.
    // Without special effect, this could be
    //  used as a PVS lookup as well.
    //
    private byte[] _rejectMatrix = Array.Empty<byte>();

    // Maintain single and multi player starting spots.
    public const int MaxDeathMatchStarts = 10;

    private MapThing[] _deathMatchStarts = new MapThing[MaxDeathMatchStarts];
    private int _deathMatchStartIdx = 0;
    private MapThing[] _playerStarts = new MapThing[Constants.MaxPlayers];

    private readonly int[] _switchList = new int[Constants.MaxSwitches * 2];
    private int _numSwitches = -1;
    private readonly Button[] _buttonList = new Button[Constants.MaxButtons];

    private AnimatingItem[] _animations = new AnimatingItem[Constants.MaxAnimations];
    private AnimatingItem? _lastAnimation = null;

    private int _numLineSpecials;
    private Line[] _lineSpecialList = new Line[Constants.MaxLineAnimations];

    private Ceiling?[] _activeCeilings = new Ceiling?[Constants.MaxCeilings];
    private Platform?[] _activePlats = new Platform?[Constants.MaxPlats];

    public GameController()
    {
        for (var i = 0; i < Constants.MaxButtons; i++)
        {
            _buttonList[i] = new();
        }

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            InitPlayer(i);
            _consistency[i] = new short[Constants.BackupTics];
        }
    }

    public GameAction GameAction { get; set; } = GameAction.Nothing;
    public GameState GameState { get; set; } = GameState.Wipe;
    public GameState WipeGameState { get; set; } = GameState.DemoScreen;

    public SkillLevel GameSkill { get; set; }
    public bool RespawnMonsters { get; set; }
    public int GameEpisode { get; set; }
    public int GameMap { get; set; }

    public bool Paused { get; set; }
    public bool SendPause { get; set; }
    public bool SendSave { get; set; }
    public bool UserGame { get; set; }

    public bool TimingDemo { get; set; }
    public bool NoDrawers { get; set; }
    public bool NoBlit { get; set; }
    public int StartTime { get; set; }

    public bool ViewActive { get; set; }

    public bool DeathMatch { get; set; }
    public bool NetGame { get; set; } = false;
    public bool[] PlayerInGame { get; } = new bool[Constants.MaxPlayers];
    public Player[] Players { get; } = new Player[Constants.MaxPlayers];

    public int ConsolePlayer { get; set; } = 0;
    public int DisplayPlayer { get; set; } = 0;
    public int GameTic { get; set; } = 0;
    public int LevelStartTic { get; set; } = 0;
    public int TotalKills { get; set; } = 0;
    public int TotalItems { get; set; } = 0;
    public int TotalSecrets { get; set; } = 0;

    public string DemoName { get; set; } = "";
    public bool DemoRecording { get; set; }
    public bool DemoPlayback { get; set; }
    public bool NetDemo { get; set; }
    public bool SingleDemo { get; set; }

    /// <summary>
    /// If true, load all graphics at start
    /// </summary>
    public bool PreCache { get; set; } = true;

    public LinkedList<Thinker> Thinkers => _thinkers;

    public int NumSectors => _numSectors;
    public Sector[] Sectors => _sectors;

    public int NumSides => _numSides;
    public SideDef[] Sides => _sides;

    public int NumNodes => _numNodes;
    public Node[] Nodes => _nodes;

    public int NumSubSectors => _numSubSectors;
    public SubSector[] SubSectors => _subSectors;

    public int NumSegments => _numSegments;
    public Segment[] Segments => _segments;
    public bool AutomapActive { get; set; }

    public void Ticker()
    {
        var buf = 0;

        // do player reborns if needed
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i] && Players[i].PlayerState == PlayerState.Reborn)
            {
                DoReborn(i);
            }
        }

        // do things to change the game state
        while (GameAction != GameAction.Nothing)
        {
            switch (GameAction)
            {
                case GameAction.LoadLevel:
                    DoLoadLevel();
                    break;
                case GameAction.NewGame:
                    DoNewGame();
                    break;
                case GameAction.LoadGame:
                    // G_DoLoadGame();
                    break;
                case GameAction.SaveGame:
                    // G_DoSaveGame();
                    break;
                case GameAction.PlayDemo:
                    DoPlayDemo();
                    break;
                case GameAction.Completed:
                    // G_DoCompleted();
                    break;
                case GameAction.Victory:
                    // F_StartFinale();
                    break;
                case GameAction.WorldDone:
                    // G_DoWorldDone();
                    break;
                case GameAction.Screenshot:
                    // M_ScreenShot();
                    GameAction = GameAction.Nothing;
                    break;
                case GameAction.Nothing:
                    break;
            }
        }

        // get commands, check consistancy,
        // and build new consistancy check
        buf = GameTic / DoomGame.Instance.TicDup % Constants.BackupTics;

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i])
            {
                var cmd = Players[i].Command;
                DoomGame.Instance.NetCommands[i][buf] = cmd;

                if (DemoPlayback)
                {
                    ReadDemoTicCommand(cmd);
                }

                if (DemoRecording)
                {
                    WriteDemoTicCommand(cmd);
                }

                // check for turbo cheats
                if (cmd.ForwardMove > TurboThreshold && (GameTic & 31) == 0 && ((GameTic >> 5) & 3) == i)
                {
                    //static char turbomessage[80];
                    //extern char* player_names[4];
                    //sprintf(turbomessage, "%s is turbo!", player_names[i]);
                    Players[ConsolePlayer].Message = "Player is turbo!";
                }

                if (NetGame && !NetDemo && (GameTic % DoomGame.Instance.TicDup) == 0)
                {
                    if (GameTic > Constants.BackupTics && _consistency[i][buf] != cmd.Consistency)
                    {
                        DoomGame.Error($"consistency failure ({cmd.Consistency} should be {_consistency[i][buf]})");
                        return;
                    }

                    if (Players[i].MapObject != null)
                    {
                        _consistency[i][buf] = (short)Players[i].MapObject!.X;
                    }
                    else
                    {
                        _consistency[i][buf] = (short)DoomRandom.RandomIndex;
                    }
                }
            }
        }

        // check for special buttons
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i])
            {
                if ((Players[i].Command.Buttons & ButtonCode.Special) != 0)
                {
                    switch (Players[i].Command.Buttons & ButtonCode.SpecialMask)
                    {
                        case ButtonCode.Pause:
                            Paused = !Paused;
                            if (Paused)
                            {
                                // S_PauseSound();
                            }
                            else
                            {
                                // S_ResumeSound();
                            }

                            break;

                        case ButtonCode.SaveGame:
                            if (string.IsNullOrWhiteSpace(_saveDescription))
                            {
                                _saveDescription = "NET GAME";
                            }

                            _saveGameSlot = (int)(Players[i].Command.Buttons & ButtonCode.SaveMask) >> (int)ButtonCode.SaveShift;
                            GameAction = GameAction.SaveGame;
                            break;
                    }
                }
            }
        }

        // do main actions
        switch (GameState)
        {
            case GameState.Level:
                P_Ticker();
                DoomGame.Instance.StatusBar.Ticker();
                //AM_Ticker();
                DoomGame.Instance.Hud.Ticker();
                break;

            case GameState.Intermission:
                // WI_Ticker();
                break;

            case GameState.Finale:
                // F_Ticker();
                break;

            case GameState.DemoScreen:
                DoomGame.Instance.PageTicker();
                break;
        }
    }

    //
    // G_InitPlayer 
    // Called at the start.
    // Called by the game initialization functions.
    //
    public void InitPlayer(int player)
    {
        // set up the saved info         
        Players[player] = new Player();

        // clear everything else to defaults 
        PlayerReborn(player);
    }

    /// <summary>
    /// Called after a player dies 
    /// almost everything is cleared and initialized 
    /// </summary>
    private Player PlayerReborn(int player)
    {
        var frags = Players[player].Frags;
        var killcount = Players[player].KillCount;
        var itemcount = Players[player].ItemCount;
        var secretcount = Players[player].SecretCount;

        var p = new Player();
        Players[player] = p;
        p.Frags = frags;
        p.KillCount = killcount;
        p.ItemCount = itemcount;
        p.SecretCount = secretcount;

        p.UseDown = p.AttackDown = true;  // don't do anything immediately 
        p.PlayerState = PlayerState.Alive;
        p.Health = Constants.MaxHealth;
        p.ReadyWeapon = p.PendingWeapon = WeaponType.Pistol;
        p.WeaponOwned[(int)WeaponType.Fist] = true;
        p.WeaponOwned[(int)WeaponType.Pistol] = true;
        p.Ammo[(int)AmmoType.Clip] = 50;

        for (var i = 0; i < (int)AmmoType.NumAmmo; i++)
        {
            p.MaxAmmo[i] = MaxAmmo[i];
        }

        return p;
    }

    /// <summary>
    /// Returns false if the player cannot be respawned
    /// at the given mapthing_t spot  
    /// because something is occupying it 
    /// </summary>
    private bool CheckSpot(int playernum, MapThing mthing)
    {
        Fixed x;
        Fixed y;
        SubSector ss;
        uint an;

        if (Players[playernum].MapObject == null)
        {
            // first spawn of level, before corpses
            for (var i = 0; i < playernum; i++)
            {
                if (Players[i].MapObject.X == (mthing.X << Constants.FracBits) && Players[i].MapObject.Y == (mthing.Y << Constants.FracBits))
                {
                    return false;
                } 
            }
            return true;
        }

        x = mthing.X << Constants.FracBits;
        y = mthing.Y << Constants.FracBits;

        //if (!P_CheckPosition(Players[playernum].MapObject, x, y))
        //{
        //    return false;
        //}

        // flush an old corpse if needed 
        if (_bodyQueueSlot >= BodyQueueSize)
        {
            P_RemoveMapObject(_bodyQueue[_bodyQueueSlot % BodyQueueSize]);
        }
        _bodyQueue[_bodyQueueSlot % BodyQueueSize] = Players[playernum].MapObject;
        _bodyQueueSlot++;

        // spawn a teleport fog 
        ss = DoomGame.Instance.Renderer.PointInSubSector(x, y);
        an = (uint)((RenderEngine.Angle45 * (mthing.Angle / 45)) >> RenderEngine.AngleToFineShift);

        P_SpawnMapObject(x + 20 * RenderEngine.FineCosine[an], y + 20 * RenderEngine.FineSine[an], ss.Sector.FloorHeight, MapObjectType.MT_TFOG);

        if (Players[ConsolePlayer].ViewZ != 1)
        {
            // S_StartSound(mo, sfx_telept);   // don't start sound on first frame 
        }

        return true;
    }

    /// <summary>
    /// Spawns a player at one of the random death match spots 
    /// called at level load and each death 
    /// </summary>
    private void DeathMatchSpawnPlayer(int playerNum)
    {
        var selections = _deathMatchStartIdx;
        if (selections < 4)
        {
            DoomGame.Error($"Only {selections} deathmatch spots, 4 required");
            return;
        }

        for (var j = 0; j < 20; j++)
        {
            var i = DoomRandom.P_Random() % selections;
            if (CheckSpot(playerNum, _deathMatchStarts[i]))
            {
                _deathMatchStarts[i].Type = (short)(playerNum + 1);
                P_SpawnPlayer(_deathMatchStarts[i]);
                return;
            }
        }

        // no good spot, so the player will probably get stuck 
        P_SpawnPlayer(_playerStarts[playerNum]);
    }

    private void DoReborn(int player)
    {
        if (!NetGame)
        {
            GameAction = GameAction.LoadLevel;
        }
        else
        {
            // respawn at the start

            // first dissasociate the corpse 
            Players[player].MapObject!.Player = null;

            // spawn at random spot if in death match 
            if (DeathMatch)
            {
                DeathMatchSpawnPlayer(player);
                return;
            }

            //if (G_CheckSpot(playernum, &playerstarts[playernum]))
            //{
            //    P_SpawnPlayer(&playerstarts[playernum]);
            //    return;
            //}

            // try to spawn at one of the other players spots 
            for (var i = 0; i < Constants.MaxPlayers; i++)
            {
                //if (G_CheckSpot(playernum, &playerstarts[i]))
                //{
                //    playerstarts[i].type = playernum + 1;   // fake as other player 
                //    P_SpawnPlayer(&playerstarts[i]);
                //    playerstarts[i].type = i + 1;       // restore 
                //    return;
                //}
                // he's going to be inside something.  Too bad.
            }
            // P_SpawnPlayer(&playerstarts[playernum]);
        }
    }

    public void DeferedPlayDemo(string demo)
    {
        DemoName = demo;
        GameAction = GameAction.PlayDemo;
    }

    private void DoPlayDemo()
    {
        GameAction = GameAction.Nothing;
        _demoDataIdx = 0;
        _demoData = DoomGame.Instance.WadData.GetLumpName(DemoName, PurgeTag.Static) ?? Array.Empty<byte>();
        if (_demoData.Length == 0 || _demoData[_demoDataIdx++] != DoomGame.Version)
        {
            DoomGame.Console.WriteLine("Demo is from a different game version!");
            GameAction = GameAction.Nothing;
            return;
        }

        var skill = (SkillLevel)_demoData[_demoDataIdx++];
        var episode = _demoData[_demoDataIdx++];
        var map = _demoData[_demoDataIdx++];
        DeathMatch = _demoData[_demoDataIdx++] != 0;
        var respawnparm = _demoData[_demoDataIdx++];
        var fastparm = _demoData[_demoDataIdx++];
        var nomonsters = _demoData[_demoDataIdx++];
        ConsolePlayer = _demoData[_demoDataIdx++];

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            PlayerInGame[i] = _demoData[_demoDataIdx++] != 0;
        }
        if (PlayerInGame[1])
        {
            NetGame = true;
            NetDemo = true;
        }

        // don't spend a lot of time in loadlevel 
        PreCache = false;
        InitNew(skill, episode, map);
        PreCache = true;

        UserGame = false;
        DemoPlayback = true;
    }

    public void DeferedInitNew(SkillLevel skill, int episode, int map)
    {
        _d_skill = skill;
        _d_episode = episode;
        _d_map = map;
        GameAction = GameAction.NewGame;
    }

    public void DoNewGame()
    {
        DemoPlayback = false;
        NetDemo = false;
        NetGame = false;
        DeathMatch = false;
        PlayerInGame[1] = PlayerInGame[2] = PlayerInGame[3] = false;
        //respawnparm = false;
        //fastparm = false;
        //nomonsters = false;
        ConsolePlayer = 0;
        InitNew(_d_skill, _d_episode, _d_map);
        GameAction = GameAction.Nothing;
    }

    public void InitNew(SkillLevel skill, int episode, int map)
    {
        if (Paused)
        {
            Paused = false;
            // S_ResumeSound();
        }

        // This was quite messy with SPECIAL and commented parts.
        // Supposedly hacks to make the latest edition work.
        // It might not work properly.
        if (episode < 1)
        {
            episode = 1;
        }

        if (DoomGame.Instance.GameMode == GameMode.Retail)
        {
            if (episode > 4)
            {
                episode = 4;
            }
        }
        else if (DoomGame.Instance.GameMode == GameMode.Shareware)
        {
            if (episode > 1)
            {
                episode = 1;    // only start episode 1 on shareware
            }
        }
        else
        {
            if (episode > 3)
            {
                episode = 3;
            }
        }

        if (map < 1)
        {
            map = 1;
        }

        if (map > 9 && DoomGame.Instance.GameMode != GameMode.Commercial)
        {
            map = 9;
        }

        DoomRandom.ClearRandom();

        //if (skill == sk_nightmare || respawnparm)
        //    respawnmonsters = true;
        //else
        //    respawnmonsters = false;

        //if (fastparm || (skill == sk_nightmare && gameskill != sk_nightmare))
        //{
        //    for (i = S_SARG_RUN1; i <= S_SARG_PAIN2; i++)
        //        states[i].tics >>= 1;
        //    mobjinfo[MT_BRUISERSHOT].speed = 20 * FRACUNIT;
        //    mobjinfo[MT_HEADSHOT].speed = 20 * FRACUNIT;
        //    mobjinfo[MT_TROOPSHOT].speed = 20 * FRACUNIT;
        //}
        //else if (skill != sk_nightmare && gameskill == sk_nightmare)
        //{
        //    for (i = S_SARG_RUN1; i <= S_SARG_PAIN2; i++)
        //        states[i].tics <<= 1;
        //    mobjinfo[MT_BRUISERSHOT].speed = 15 * FRACUNIT;
        //    mobjinfo[MT_HEADSHOT].speed = 10 * FRACUNIT;
        //    mobjinfo[MT_TROOPSHOT].speed = 10 * FRACUNIT;
        //}


        // force players to be initialized upon first level load         
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            Players[i].PlayerState = PlayerState.Reborn;
        }

        UserGame = true;                // will be set false if a demo 
        Paused = false;
        DemoPlayback = false;
        // AutomapActive = false;
        ViewActive = true;
        GameEpisode = episode;
        GameMap = map;
        GameSkill = skill;

        ViewActive = true;

        var sky = DoomGame.Instance.Renderer.Sky;
        // set the sky map for the episode
        if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY3");
            if (GameMap < 12)
            {
                sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY1");
            }
            else if (GameMap < 21)
            {
                sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY2");
            }
        }
        else
        {
            switch (episode)
            {
                case 1:
                    sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY1");
                    break;
                case 2:
                    sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY2");
                    break;
                case 3:
                    sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY3");
                    break;
                case 4: // Special Edition sky
                    sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY4");
                    break;
            }
        }

        DoLoadLevel();
    }

    private void ReadDemoTicCommand(TicCommand cmd)
    {
        if (_demoData[_demoDataIdx] == Constants.DemoMarker)
        {
            // end of demo data stream 
            CheckDemoStatus();
            return;
        }

        cmd.ForwardMove = (sbyte)_demoData[_demoDataIdx++];
        cmd.SideMove = (sbyte)_demoData[_demoDataIdx++];
        cmd.AngleTurn = (short)(_demoData[_demoDataIdx++] << 8);
        cmd.Buttons = (ButtonCode)_demoData[_demoDataIdx++];
    }

    private void WriteDemoTicCommand(TicCommand cmd)
    {
        if (_gameKeyDown['q']) // press q to end demo recording 
        {
            CheckDemoStatus();
        }

        _demoData[_demoDataIdx++] = (byte)cmd.ForwardMove;
        _demoData[_demoDataIdx++] = (byte)cmd.SideMove;
        _demoData[_demoDataIdx++] = (byte)(cmd.AngleTurn + 128 >> 8);
        _demoData[_demoDataIdx++] = (byte)cmd.Buttons;

        _demoDataIdx -= 4;
        if (_demoDataIdx > _demoEnd - 16)
        {
            // no more space 
            CheckDemoStatus();
            return;
        }

        ReadDemoTicCommand(cmd);         // make SURE it is exactly the same 
    }

    private void RecordDemo(string name)
    {
        UserGame = false;
        DemoName = name + ".lmp";
        var maxSize = 0x20000;
        //i = M_CheckParm("-maxdemo");
        //if (i && i < myargc - 1)
        //    maxsize = atoi(myargv[i + 1]) * 1024;
        _demoData = new byte[maxSize];
        _demoEnd = maxSize;

        DemoRecording = true;
    }

    private void BeginRecording()
    {
        _demoDataIdx = 0;

        _demoData[_demoDataIdx++] = DoomGame.Version;
        _demoData[_demoDataIdx++] = (byte)GameSkill;
        _demoData[_demoDataIdx++] = (byte)GameEpisode;
        _demoData[_demoDataIdx++] = (byte)GameMap;
        _demoData[_demoDataIdx++] = (byte)(DeathMatch ? 1 : 0);
        _demoData[_demoDataIdx++] = 0; // respawnparam
        _demoData[_demoDataIdx++] = 0; // fastparm;
        _demoData[_demoDataIdx++] = 0; // nomonsters;
        _demoData[_demoDataIdx++] = (byte)ConsolePlayer;

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            _demoData[_demoDataIdx++] = (byte)(PlayerInGame[i] ? 1 : 0);
        }
    }

    private void DoLoadLevel()
    {
        // Set the sky map.
        // First thing, we have a dummy sky texture name,
        //  a flat. The data is in the WAD only because
        //  we look for an actual index, instead of simply
        //  setting one.
        var sky = DoomGame.Instance.Renderer.Sky;
        sky.FlatNum = DoomGame.Instance.Renderer.FlatNumForName(Sky.FlatName);
        
        // DOOM determines the sky texture to be used
        // depending on the current episode, and the game version.
        if (DoomGame.Instance.GameMode == GameMode.Commercial) // or Pack TNT/Plutonium
        {
            sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY3");
            if (GameMap < 12)
            {
                sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY1");
            }
            else if (GameMap < 21)
            {
                sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY2");
            }
        }

        LevelStartTic = GameTic; // for time calculation

        if (WipeGameState == GameState.Level)
        {
            WipeGameState = GameState.Wipe; // force a wipe 
        }

        GameState = GameState.Level;

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i] && Players[i].PlayerState == PlayerState.Dead)
            {
                Players[i].PlayerState = PlayerState.Reborn;
            }

            for (var j = 0; j < Players[i].Frags.Length; j++)
            {
                Players[i].Frags[j] = 0;
            }
        }

        SetupLevel(GameEpisode, GameMap, 0, GameSkill);
        DisplayPlayer = ConsolePlayer;		// view the guy you are playing    
        StartTime = DoomGame.Instance.GetTime();
        GameAction = GameAction.Nothing;
        //Z_CheckHeap();

        //// clear cmd building stuff
        //memset(gamekeydown, 0, sizeof(gamekeydown));
        //joyxmove = joyymove = 0; 
        //mousex = mousey = 0; 
        //sendpause = sendsave = paused = false; 
        //memset(mousebuttons, 0, sizeof(mousebuttons));
        //memset(joybuttons, 0, sizeof(joybuttons));
    }

    public bool CheckDemoStatus()
    {
        if (TimingDemo)
        {
            var endTime = DoomGame.Instance.GetTime();
            DoomGame.Error($"timed {GameTic} gametics in {endTime - StartTime} realtics");
            return true;
        }

        if (DemoPlayback)
        {
            if (SingleDemo)
            {
                DoomGame.Instance.Quit();
                return true;
            }

            // Z_ChangeTag(demobuffer, PU_CACHE);
            DemoPlayback = false;
            NetDemo = false;
            NetGame = false;
            DeathMatch = false;
            PlayerInGame[1] = PlayerInGame[2] = PlayerInGame[3] = false;
            //respawnparm = false;
            //fastparm = false;
            //nomonsters = false;
            ConsolePlayer = 0;
            DoomGame.Instance.AdvanceDemo();
            return true;
        }

        if (DemoRecording)
        {
            _demoData[_demoDataIdx++] = Constants.DemoMarker;
            //M_WriteFile(demoname, demobuffer, demo_p - demobuffer);
            //Z_Free(demobuffer);
            DemoRecording = false;
            DoomGame.Error($"Demo {DemoName} recorded");
        }

        return false;
    }

    public void InitThinkers()
    {
        _thinkers = new LinkedList<Thinker>();
    }

    /// <summary>
    /// Adds a new thinker at the end of the list
    /// </summary>
    /// <param name="thinker"></param>
    public void AddThinker(Thinker thinker)
    {
        _thinkers.AddLast(thinker);
    }

    /// <summary>
    /// Deallocation is lazy -- it will not actually be freed until its thinking turn comes up.
    /// </summary>
    /// <param name="thinker"></param>
    public void RemoveThinker(Thinker thinker)
    {
        thinker.Action = null;
    }

    private void RunThinkers()
    {
        var thinkerNode = _thinkers.First;

        while (thinkerNode != null)
        {
            var thinker = thinkerNode.Value;
            if (thinker.Action == null)
            {
                // Time to remove this thinker
                var next = thinkerNode.Next;
                _thinkers.Remove(thinkerNode);
                thinkerNode = next;
            }
            else
            {
                thinker.Action(new ActionParams(thinker as MapObject));
                thinkerNode = thinkerNode.Next;
            }
        }
    }

    public void P_Ticker()
    {
        // run the tic
        if (Paused)
        {
            return;
        }

        // pause if in menu and at least one tic has been run
        if (!NetGame
            && DoomGame.Instance.MenuActive
            && !DemoPlayback
            && Players[ConsolePlayer].ViewZ != 1)
        {
            return;
        }


        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i])
            {
                Players[i].Think();
            }
        }

        RunThinkers();
        //UpdateSpecials();
        //RespawnSpecials();

        // for par times
        LevelTime++;
    }

    private void P_LoadVertexes(int lump)
    {
        // Determine number of lumps:
        //  total lump length / vertex record length.
        _numVertices = DoomGame.Instance.WadData.LumpLength(lump) / 4; // two shorts

        // Allocate zone memory for buffer.
        _vertices = new Vertex[_numVertices];

        // Load data into cache.
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        // Copy and convert vertex coordinates,
        // internal representation as fixed.
        for (var i = 0; i < _numVertices; i++)
        {
            _vertices[i] = new Vertex(
                new Fixed(reader.ReadInt16() << Constants.FracBits),
                new Fixed(reader.ReadInt16() << Constants.FracBits)
            );
        }
    }

    private void P_LoadSegments(int lump)
    {
        const int sizeOfMapSeg = 2 + 2 + 2 + 2 + 2 + 2;
        _numSegments = DoomGame.Instance.WadData.LumpLength(lump) / sizeOfMapSeg;
        _segments = new Segment[_numSegments];
        
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < _numSegments; i++)
        {
            _segments[i] = Segment.ReadFromWadData(reader, _vertices, _sides, _lines);
        }
    }

    private void P_LoadSubSectors(int lump)
    {
        _numSubSectors = DoomGame.Instance.WadData.LumpLength(lump) / 4; // two shorts
        _subSectors = new SubSector[_numSubSectors];
        
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < _numSubSectors; i++)
        {
            _subSectors[i] = SubSector.ReadFromWadData(reader);
        }
    }

    private void P_LoadSectors(int lump)
    {
        const int sizeOfMapSector = 2 + 2 + 8 + 8 + 2 + 2 + 2;
        _numSectors = DoomGame.Instance.WadData.LumpLength(lump) / sizeOfMapSector;
        _sectors = new Sector[_numSectors];
        
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < _numSectors; i++)
        {
            _sectors[i] = Sector.ReadFromWadData(reader);
        }
    }

    private void P_LoadNodes(int lump)
    {
        const int sizeOfMapNode = 2 + 2 + 2 + 2 + (2 * 2 * 4) + 2 + 2;
        _numNodes = DoomGame.Instance.WadData.LumpLength(lump) / sizeOfMapNode;
        _nodes = new Node[_numNodes];
        
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < _numNodes; i++)
        {
            _nodes[i] = Node.ReadFromWadData(reader);
        }
    }

    private void P_LoadThings(int lump)
    {
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        var numThings = DoomGame.Instance.WadData.LumpLength(lump) / MapThing.SizeOfStruct;

        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < numThings; i++)
        {
            var spawn = true;
            var mt = MapThing.FromWadData(reader);

            // Do not spawn cool, new monsters if !commercial
            if (DoomGame.Instance.GameMode != GameMode.Commercial)
            {
                switch (mt.Type)
                {
                    case 68:    // Arachnotron
                    case 64:    // Archvile
                    case 88:    // Boss Brain
                    case 89:    // Boss Shooter
                    case 69:    // Hell Knight
                    case 67:    // Mancubus
                    case 71:    // Pain Elemental
                    case 65:    // Former Human Commando
                    case 66:    // Revenant
                    case 84:    // Wolf SS
                        spawn = false;
                        break;
                }
            }
            if (spawn == false)
                break;

            // Do spawn all other stuff.
            P_SpawnMapThing(mt);
        }
    }

    /// <summary>
    /// Called when a player is spawned on the level.
    /// Most of the player structure stays unchanged
    ///  between levels.
    /// </summary>
    private void P_SpawnPlayer(MapThing mthing)
    {
        Fixed x;
        Fixed y;
        Fixed z;

        int i;

        // not playing?
        if (!PlayerInGame[mthing.Type - 1])
        {
            return;
        }

        var p = Players[mthing.Type - 1];

        if (p.PlayerState == PlayerState.Reborn)
        {
            p = PlayerReborn(mthing.Type - 1);
        }

        x = mthing.X << Constants.FracBits;
        y = mthing.Y << Constants.FracBits;
        z = Constants.OnFloorZ;
        var mobj = P_SpawnMapObject(x, y, z, MapObjectType.MT_PLAYER);

        // set color translations for player sprites
        if (mthing.Type > 1)
        {
            mobj.Flags |= (MapObjectFlag)((mthing.Type - 1) << (int)MapObjectFlag.MF_TRANSSHIFT);
        }

        mobj.Angle = (uint)(RenderEngine.Angle45 * (mthing.Angle / 45));
        mobj.Player = p;
        mobj.Health = p.Health;

        p.MapObject = mobj;
        p.PlayerState = PlayerState.Alive;
        p.Refire = 0;
        p.Message = null;
        p.DamageCount = 0;
        p.BonusCount = 0;
        p.ExtraLight = 0;
        p.FixedColorMap = 0;
        p.ViewHeight = Constants.ViewHeight;

        // setup gun psprite
        P_SetupPlayerSprites(p);

        // give all cards in death match mode
        if (DeathMatch)
        {
            for (i = 0; i < (int)KeyCardType.NumberOfKeyCards; i++)
            {
                p.Cards[i] = true;
            }
        }

        if (mthing.Type - 1 == ConsolePlayer)
        {
            // wake up the status bar
            DoomGame.Instance.StatusBar.Start();
            // wake up the heads up text
            DoomGame.Instance.Hud.Start();
        }
    }

    private void P_SetupPlayerSprites(Player player)
    {
        // remove all psprites
        for (var i = 0; i < (int)PlayerSpriteType.NumPlayerSprites; i++)
        {
            player.PlayerSprites[i].State = null;
        }

        // spawn the gun
        player.PendingWeapon = player.ReadyWeapon;
        P_BringUpWeapon(player);
    }

    //
    // P_MovePsprites
    // Called every tic by player thinking routine.
    //
    public void P_MovePlayerSprites(Player player)
    {
        for (var i = 0; i < (int)PlayerSpriteType.NumPlayerSprites; i++)
        {
            var psp = player.PlayerSprites[i];

            // a null state means not active
            var state = psp.State;
            if (state != null)
            {
                // drop tic count and possibly change state

                // a -1 tic count never changes
                if (psp.Tics != -1)
                {
                    psp.Tics--;
                    if (psp.Tics == 0)
                    {
                        P_SetPlayerSprite(player, (PlayerSpriteType)i, state.NextState);
                    }
                }
            }
        }

        player.PlayerSprites[(int)PlayerSpriteType.Flash].SX = player.PlayerSprites[(int)PlayerSpriteType.Weapon].SX;
        player.PlayerSprites[(int)PlayerSpriteType.Flash].SY = player.PlayerSprites[(int)PlayerSpriteType.Weapon].SY;
    }

    public void P_SetPlayerSprite(Player player, PlayerSpriteType position, StateNum stnum)
    {
        var psp = player.PlayerSprites[(int)position];

        do
        {
            if (stnum == StateNum.S_NULL)
            {
                // object removed itself
                psp.State = null;
                break;
            }

            var state = State.GetSpawnState(stnum);
            psp.State = state;
            psp.Tics = state.Tics;    // could be 0

            if (state.Misc1 != 0)
            {
                // coordinate set
                psp.SX = state.Misc1 << Constants.FracBits;
                psp.SY = state.Misc2 << Constants.FracBits;
            }

            // Call action routine.
            // Modified handling.
            if (state.Action != null)
            {
                state.Action(new ActionParams(Player: player, PlayerSprite: psp));
                if (psp.State == null)
                {
                    break;
                }
            }

            stnum = psp.State.NextState;
        } while (psp.Tics == 0);
        // an initial state of 0 could cycle through
    }

    //
    // P_CalcSwing
    //	
    private Fixed _swingX;
    private Fixed _swingY;

    private void P_CalcSwing(Player player)
    {
        // OPTIMIZE: tablify this.
        // A LUT would allow for different modes,
        //  and add flexibility.

        var swing = player.Bob;

        var angle = (RenderEngine.FineAngles / 70 * LevelTime) & RenderEngine.FineMask;
        _swingX = swing * RenderEngine.FineSine[angle];

        angle = (RenderEngine.FineAngles / 70 * LevelTime + RenderEngine.FineAngles / 2) & RenderEngine.FineMask;
        _swingY = -(_swingX * RenderEngine.FineSine[angle]);
    }

    /// <summary>
    /// Starts bringing the pending weapon up
    /// from the bottom of the screen.
    /// Uses player
    /// </summary>
    public void P_BringUpWeapon(Player player)
    {
        if (player.PendingWeapon == WeaponType.NoChange)
        {
            player.PendingWeapon = player.ReadyWeapon;
        }

        if (player.PendingWeapon == WeaponType.Chainsaw)
        {
            //S_StartSound(player->mo, sfx_sawup);
        }

        var newstate = WeaponInfo.GetByType(player.PendingWeapon).UpState;

        player.PendingWeapon = WeaponType.NoChange;
        player.PlayerSprites[(int)PlayerSpriteType.Weapon].SY = WeaponInfo.WeaponBottom;

        P_SetPlayerSprite(player, PlayerSpriteType.Weapon, newstate);
    }

    public bool P_CheckAmmo(Player player)
    {
        var ammo = WeaponInfo.GetByType(player.ReadyWeapon).Ammo;

        // Minimal amount for one shot varies.
        var count = 1;
        if (player.ReadyWeapon == WeaponType.Bfg)
        {
            count = WeaponInfo.BFGCells;
        }
        else if (player.ReadyWeapon == WeaponType.SuperShotgun)
        {
            count = 2;
        }

        // Some do not need ammunition anyway.
        // Return if current ammunition sufficient.
        if (ammo == AmmoType.NoAmmo || player.Ammo[(int)ammo] >= count)
        {
            return true;
        }

        // Out of ammo, pick a weapon to change to.
        // Preferences are set here.
        do
        {
            if (player.WeaponOwned[(int)WeaponType.Plasma]
                && player.Ammo[(int)AmmoType.Cell] != 0
                && (DoomGame.Instance.GameMode != GameMode.Shareware))
            {
                player.PendingWeapon = WeaponType.Plasma;
            }
            else if (player.WeaponOwned[(int)WeaponType.SuperShotgun]
                 && player.Ammo[(int)AmmoType.Shell] > 2
                 && (DoomGame.Instance.GameMode == GameMode.Commercial))
            {
                player.PendingWeapon = WeaponType.SuperShotgun;
            }
            else if (player.WeaponOwned[(int)WeaponType.Chaingun]
                 && player.Ammo[(int)AmmoType.Clip] != 0)
            {
                player.PendingWeapon = WeaponType.Chaingun;
            }
            else if (player.WeaponOwned[(int)WeaponType.Shotgun]
                 && player.Ammo[(int)AmmoType.Shell] != 0)
            {
                player.PendingWeapon = WeaponType.Shotgun;
            }
            else if (player.Ammo[(int)AmmoType.Clip] != 0)
            {
                player.PendingWeapon = WeaponType.Pistol;
            }
            else if (player.WeaponOwned[(int)WeaponType.Chainsaw])
            {
                player.PendingWeapon = WeaponType.Chainsaw;
            }
            else if (player.WeaponOwned[(int)WeaponType.Missile]
                 && player.Ammo[(int)AmmoType.Missile] != 0)
            {
                player.PendingWeapon = WeaponType.Missile;
            }
            else if (player.WeaponOwned[(int)WeaponType.Bfg]
                 && player.Ammo[(int)AmmoType.Cell] > 40
                 && (DoomGame.Instance.GameMode != GameMode.Shareware))
            {
                player.PendingWeapon = WeaponType.Bfg;
            }
            else
            {
                // If everything fails.
                player.PendingWeapon = WeaponType.Fist;
            }

        } while (player.PendingWeapon == WeaponType.NoChange);

        // Now set appropriate weapon overlay.
        P_SetPlayerSprite(player, PlayerSpriteType.Weapon, WeaponInfo.GetByType(player.ReadyWeapon).DownState);

        return false;
    }

    public void P_FireWeapon(Player player)
    {
        if (!P_CheckAmmo(player))
        {
            return;
        }

        player.MapObject!.SetState(StateNum.S_PLAY_ATK1);
        var newState = WeaponInfo.GetByType(player.ReadyWeapon).AttackState;
        P_SetPlayerSprite(player, PlayerSpriteType.Weapon, newState);
        // TODO P_NoiseAlert(player->mo, player->mo);
    }

    private void P_SpawnMapThing(MapThing mthing)
    {
        int i;
        int bit;
        Fixed x;
        Fixed y;
        Fixed z;

        // count deathmatch start positions
        if (mthing.Type == 11)
        {
            if (_deathMatchStartIdx < 10)
            {
                _deathMatchStarts[_deathMatchStartIdx++] = mthing;
            }
            return;
        }

        // check for players specially
        if (mthing.Type <= 4)
        {
            // save spots for respawning in network games
            _playerStarts[mthing.Type - 1] = mthing;
            if (!DeathMatch)
            {
                P_SpawnPlayer(mthing);
            }

            return;
        }

        // check for appropriate skill level
        if (!NetGame && (mthing.Options & 16) != 0)
        {
            return;
        }

        bit = GameSkill switch
        {
            SkillLevel.Baby => 1,
            SkillLevel.Nightmare => 4,
            _ => 1 << ((int)GameSkill - 1)
        };

        if ((mthing.Options & bit) == 0)
        {
            return;
        }

        // find which type to spawn
        var moInfo = MapObjectInfo.FindByDoomedNum(mthing.Type);
        if (moInfo is null)
        {
            DoomGame.Error($"P_SpawnMapThing: Unknown type {mthing.Type} at ({mthing.X}, {mthing.Y})");
            return;
        }

        // don't spawn keycards and players in deathmatch
        if (DeathMatch && (moInfo.Value.Flags & MapObjectFlag.MF_NOTDMATCH) != 0)
        {
            return;
        }

        //// don't spawn any monsters if -nomonsters
        //if (NoMonsters
        //    && (i == MT_SKULL
        //        || (mobjinfo[i].flags & MF_COUNTKILL)))
        //{
        //    return;
        //}

        // spawn it
        x = new Fixed(mthing.X << Constants.FracBits);
        y = new Fixed(mthing.Y << Constants.FracBits);

        if ((moInfo.Value.Flags & MapObjectFlag.MF_SPAWNCEILING) != 0)
        {
            z = Constants.OnCeilingZ;
        }
        else
        {
            z = Constants.OnFloorZ;
        }

        var mobj = P_SpawnMapObject(x, y, z, moInfo.Value.Type);
        mobj.Spawnpoint = mthing;

        if (mobj.Tics > 0)
        {
            mobj.Tics = 1 + (DoomRandom.P_Random() % mobj.Tics);
        }
        if ((mobj.Flags & MapObjectFlag.MF_COUNTKILL) != 0)
        {
            TotalKills++;
        }

        if ((mobj.Flags & MapObjectFlag.MF_COUNTITEM) != 0)
        {
            TotalItems++;
        }

        mobj.Angle = (uint)(RenderEngine.Angle45 * (mthing.Angle / 45));
        if ((mthing.Options & (int)MapThingFlag.MTF_AMBUSH) != 0)
        {
            mobj.Flags |= MapObjectFlag.MF_AMBUSH;
        }
    }

    private void P_UnsetThingPosition(MapObject thing)
    {
        if ((thing.Flags & MapObjectFlag.MF_NOSECTOR) == 0)
        {
            // inert things don't need to be in blockmap?
            // unlink from subsector
            if (thing.SectorNext != null)
            {
                thing.SectorNext.SectorPrev = thing.SectorPrev;
            }

            if (thing.SectorPrev != null)
            {
                thing.SectorPrev.SectorNext = thing.SectorNext;
            }
            else
            {
                thing.SubSector!.Sector!.ThingList = thing.SectorNext;
            }
        }

        if ((thing.Flags & MapObjectFlag.MF_NOBLOCKMAP) == 0)
        {
            // inert things don't need to be in blockmap
            // unlink from block map
            if (thing.BlockNext != null)
            {
                thing.BlockNext.BlockPrev = thing.BlockPrev;
            }

            if (thing.BlockPrev != null)
            {
                thing.BlockPrev.BlockNext = thing.BlockNext;
            }
            else
            {
                var blockx = (thing.X - _blockMapOriginX) >> Constants.MapBlockShift;
                var blocky = (thing.Y - _blockMapOriginY) >> Constants.MapBlockShift;

                if (blockx >= 0 && blockx < _blockMapWidth && blocky >= 0 && blocky < _blockMapHeight)
                {
                    _blockLinks[blocky * _blockMapWidth + blockx] = thing.BlockNext;
                }
            }
        }
    }

    private void P_SetThingPosition(MapObject thing)
    {
        // link into subsector
        var ss = DoomGame.Instance.Renderer.PointInSubSector(thing.X, thing.Y);
        thing.SubSector = ss;

        if ((thing.Flags & MapObjectFlag.MF_NOSECTOR) == 0)
        {
            // invisible things don't go into the sector links
            var sec = ss.Sector!;

            thing.SectorPrev = null;
            thing.SectorNext = sec.ThingList;

            if (sec.ThingList != null)
            {
                sec.ThingList.SectorPrev = thing;
            }

            sec.ThingList = thing;
        }

        // link into blockmap
        if ((thing.Flags & MapObjectFlag.MF_NOBLOCKMAP) == 0)
        {
            // inert things don't need to be in blockmap		
            var blockx = (thing.X - _blockMapOriginX) >> Constants.MapBlockShift;
            var blocky = (thing.Y - _blockMapOriginY) >> Constants.MapBlockShift;

            if (blockx >= 0
                && blockx < _blockMapWidth
                && blocky >= 0
                && blocky < _blockMapHeight)
            {
                var link = _blockLinks[blocky * _blockMapWidth + blockx];
                thing.BlockPrev = null;
                thing.BlockNext = link;

                if (link != null)
                {
                    link.BlockPrev = thing;
                }

                _blockLinks[blocky * _blockMapWidth + blockx] = thing;
            }
            else
            {
                // thing is off the map
                thing.BlockNext = thing.BlockPrev = null;
            }
        }
    }

    private void P_MapObjectThinker(ActionParams actionParams)
    {
        var mobj = actionParams.MapObject;
        if (mobj is null)
        {
            return;
        }

        // momentum movement
        if (mobj.MomX != 0 || mobj.MomY != 0 || (mobj.Flags & MapObjectFlag.MF_SKULLFLY) != 0)
        {
            // P_XYMovement(mobj);

            if (mobj.Action == null)
            {
                return;     // mobj was removed
            }
        }

        if ((mobj.Z != mobj.FloorZ) || mobj.MomZ != 0)
        {
            // P_ZMovement(mobj);

            if (mobj.Action == null)
            {
                return;     // mobj was removed
            }
        }

        // cycle through states,
        // calling action functions at transitions
        if (mobj.Tics != -1)
        {
            mobj.Tics--;

            // you can cycle through multiple states in a tic
            if (mobj.Tics == 0)
            {
                if (!mobj.SetState(mobj.State!.NextState))
                {
                    return;     // freed itself
                }
            }
        }
        else
        {
            // check for nightmare respawn
            if ((mobj.Flags & MapObjectFlag.MF_COUNTKILL) == 0)
            {
                return;
            }

            // if (!respawnmonsters)
                // return;

            mobj.MoveCount++;

            if (mobj.MoveCount < 12 * 35)
            { 
                return; 
            }

            if ((LevelTime & 31) != 0)
            {
                return;
            }

            if (DoomRandom.P_Random() > 4)
            {
                return;
            }

            // P_NightmareRespawn(mobj);
        }
    }

    public MapObject P_SpawnMapObject(Fixed x, Fixed y, Fixed z, MapObjectType type)
    {
        var info = MapObjectInfo.GetByType(type);
        var mobj = new MapObject(info)
        {
            X = x,
            Y = y
        };

        if (GameSkill != SkillLevel.Nightmare)
        {
            mobj.ReactionTime = info.ReactionTime;
        }

        mobj.LastLook = DoomRandom.P_Random() % Constants.MaxPlayers;
        // do not set the state with P_SetMobjState,
        // because action routines can not be called yet
        var st = State.GetSpawnState(info.SpawnState);
        mobj.State = st;
        mobj.Tics = st.Tics;
        mobj.Sprite = st.Sprite;
        mobj.Frame = st.Frame;

        // set subsector and/or block links
        P_SetThingPosition(mobj);

        mobj.FloorZ = mobj.SubSector!.Sector!.FloorHeight;
        mobj.CeilingZ = mobj.SubSector.Sector.CeilingHeight;

        if (z == Constants.OnFloorZ)
        {
            mobj.Z = mobj.FloorZ;
        }
        else if (z == Constants.OnCeilingZ)
        {
            mobj.Z = mobj.CeilingZ - mobj.Info.Height;
        }
        else
        {
            mobj.Z = z;
        }

        mobj.Action = P_MapObjectThinker;

        AddThinker(mobj);

        return mobj;
    }

    public void P_RemoveMapObject(MapObject mobj)
    {
        if ((mobj.Flags & MapObjectFlag.MF_SPECIAL) != 0
            && (mobj.Flags & MapObjectFlag.MF_DROPPED) == 0
            && (mobj.Type != MapObjectType.MT_INV)
            && (mobj.Type != MapObjectType.MT_INS))
        {
            //itemrespawnque[iquehead] = mobj->spawnpoint;
            //itemrespawntime[iquehead] = leveltime;
            //iquehead = (iquehead + 1) & (ITEMQUESIZE - 1);

            //// lose one off the end?
            //if (iquehead == iquetail)
            //    iquetail = (iquetail + 1) & (ITEMQUESIZE - 1);
        }

        // unlink from sector and block lists
        P_UnsetThingPosition(mobj);

        //// stop any playing sound
        //S_StopSound(mobj);

        // free block
        RemoveThinker(mobj);
    }

    /// <summary>
    /// P_LoadLineDefs. Also counts secret lines for intermissions.
    /// </summary>
    private void P_LoadLineDefs(int lump)
    {
        const int sizeOfMapLineDef = 2 + 2 + 2 + 2 + 2 + 2 + 2;

        _numLines = DoomGame.Instance.WadData.LumpLength(lump) / sizeOfMapLineDef;
        _lines = new Line[_numLines];
        
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < _numLines; i++)
        {
            var ld = Line.ReadFromWadData(reader, _vertices);
            _lines[i] = ld;

            if (ld.Dx == 0)
            {
                ld.SlopeType = SlopeType.Vertical;
            }
            else if (ld.Dy != 0)
            {
                ld.SlopeType = SlopeType.Horizontal;
            }
            else
            {
                ld.SlopeType = ld.Dy / ld.Dx > 0 ? SlopeType.Positive : SlopeType.Negative;
            }

            if (ld.V1.X < ld.V2.X)
            {
                ld.BoundingBox[BoundingBox.BoxLeft] = ld.V1.X;
                ld.BoundingBox[BoundingBox.BoxRight] = ld.V2.X;
            }
            else
            {
                ld.BoundingBox[BoundingBox.BoxLeft] = ld.V2.X;
                ld.BoundingBox[BoundingBox.BoxRight] = ld.V1.X;
            }

            if (ld.V1.Y < ld.V2.Y)
            {
                ld.BoundingBox[BoundingBox.BoxBottom] = ld.V1.Y;
                ld.BoundingBox[BoundingBox.BoxTop] = ld.V2.Y;
            }
            else
            {
                ld.BoundingBox[BoundingBox.BoxBottom] = ld.V2.Y;
                ld.BoundingBox[BoundingBox.BoxTop] = ld.V1.Y;
            }

            if (ld.SideNum[0] != -1)
            {
                ld.FrontSector = _sides[ld.SideNum[0]].Sector;
            }

            if (ld.SideNum[1] != -1)
            {
                ld.BackSector = _sides[ld.SideNum[1]].Sector;
            }
        }
    }
    
    private void P_LoadSideDefs(int lump)
    {
        const int mapSideDefSize = 2 + 2 + 8 + 8 + 8 + 2; // size of mapsidedef_t struct
        _numSides = DoomGame.Instance.WadData.LumpLength(lump) / mapSideDefSize;
        _sides = new SideDef[_numSides];
        
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Static)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        for (var i = 0; i < _numSides; i++)
        {
            _sides[i] = SideDef.ReadFromWadData(reader, _sectors);
        }
    }

    private void P_LoadBlockMap(int lump)
    {
        var data = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Level)!;
        using var stream = new MemoryStream(data, false);
        using var reader = new BinaryReader(stream);

        var count = DoomGame.Instance.WadData.LumpLength(lump) / 2;
        _blockMapLump = new short[count];
        for (var i = 0; i < _blockMapLump.Length; i++)
        {
            _blockMapLump[i] = reader.ReadInt16();
        }

        // blockmap = blockmaplump+4;
        _blockMap = new short[count - 4];
        for (var i = 4; i < _blockMapLump.Length; i++)
        {
            _blockMap[i - 4] = _blockMapLump[i];
        }

        _blockMapOriginX = new Fixed(_blockMapLump[0] << Constants.FracBits);
        _blockMapOriginY = new Fixed(_blockMapLump[1] << Constants.FracBits);
        _blockMapWidth = _blockMapLump[2];
        _blockMapHeight = _blockMapLump[3];

        // clear out mobj chains
        //count = _blockMapWidth * _blockMapHeight;
        //_blockLinks = Z_Malloc(count, PurgeTag.Level, 0);
        //memset(_blockLinks, 0, count);
        _blockLinks = new MapObject?[_blockMapWidth * _blockMapHeight];
        //for (var i = 0; i < _blockMapWidth; i++)
        //{
        //    _blockLinks[i] = new MapObject[_blockMapHeight];
        //}
    }

    private void P_GroupLines()
    {
        // look up sector number for each subsector
        for (var i = 0; i < _numSubSectors; i++)
        {
            var subSector = _subSectors[i];
            var seg = _segments[subSector.FirstLine];
            subSector.Sector = seg.SideDef.Sector;
        }

        // count number of lines in each sector
        var total = 0;
        for (var i = 0; i < _numLines; i++)
        {
            total++;
            var line = _lines[i];
            line.FrontSector!.LineCount++;

            if (line.BackSector != null && line.BackSector != line.FrontSector)
            {
                line.BackSector.LineCount++;
                total++;
            }
        }

        // build line tables for each sector	
        var boundingBox = new Fixed[4];
        for (var i = 0; i < _numSectors; i++)
        {
            BoundingBox.ClearBox(boundingBox);

            var lineIdx = 0;
            var sector = _sectors[i];
            sector.Lines = new Line[sector.LineCount];

            for (var j = 0; j < _numLines; j++)
            {
                var line = _lines[j];
                if (line.FrontSector == sector || line.BackSector == sector)
                {
                    sector.Lines[lineIdx++] = line;
                    BoundingBox.AddToBox(boundingBox, line.V1.X, line.V1.Y);
                    BoundingBox.AddToBox(boundingBox, line.V2.X, line.V2.Y);
                }
            }

            if (lineIdx != sector.LineCount)
            {
                DoomGame.Error("P_GroupLines: miscounted");
                return;
            }

            // set the degenmobj_t to the middle of the bounding box
            sector.SoundOrigin.X = (boundingBox[BoundingBox.BoxRight] + boundingBox[BoundingBox.BoxLeft]) / 2;
            sector.SoundOrigin.Y = (boundingBox[BoundingBox.BoxTop] + boundingBox[BoundingBox.BoxBottom]) / 2;

            // adjust bounding box to map blocks
            var block = (boundingBox[BoundingBox.BoxTop] - _blockMapOriginY + Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block >= _blockMapHeight ? _blockMapHeight - 1 : block;
            sector.BlockBox[BoundingBox.BoxTop] = block;

            block = (boundingBox[BoundingBox.BoxBottom] - _blockMapOriginY - Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block < 0 ? 0 : block;
            sector.BlockBox[BoundingBox.BoxBottom] = block;

            block = (boundingBox[BoundingBox.BoxRight] - _blockMapOriginX + Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block >= _blockMapWidth ? _blockMapWidth - 1 : block;
            sector.BlockBox[BoundingBox.BoxRight] = block;

            block = (boundingBox[BoundingBox.BoxLeft] - _blockMapOriginX - Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block < 0 ? 0 : block;
            sector.BlockBox[BoundingBox.BoxLeft] = block;
        }
    }
    
    private void SetupLevel(int episode, int map, int playerMask, SkillLevel skillLevel)
    {
        TotalKills = TotalItems = TotalSecrets = _wmInfo.MaxFrags = 0;
        _wmInfo.ParTime = 180;

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            Players[i].KillCount = Players[i].SecretCount = Players[i].ItemCount = 0;
        }

        // Initial height of PointOfView
        // will be set by player think.
        Players[ConsolePlayer].ViewZ = 1;

        // Make sure all sounds are stopped before Z_FreeTags.
        // S_Start();

        // TODO Free up memory?
        // Z_FreeTags (PU_LEVEL, PU_PURGELEVEL-1);

        InitThinkers();

        // if working with a development map, reload it
        // W_Reload();

        // find map name
        var lumpName = DoomGame.Instance.GameMode == GameMode.Commercial ? $"map{map:00}" : $"E{episode}M{map}";
        var lumpNum = DoomGame.Instance.WadData.GetNumForName(lumpName);

        LevelTime = 0;

        // note: most of this ordering is important	
        P_LoadBlockMap(lumpNum + MapLumps.BlockMap);
        P_LoadVertexes(lumpNum + MapLumps.Vertices);
        P_LoadSectors(lumpNum + MapLumps.Sectors);
        P_LoadSideDefs(lumpNum + MapLumps.SideDefs);

        P_LoadLineDefs(lumpNum + MapLumps.LineDefs);
        P_LoadSubSectors(lumpNum + MapLumps.SubSectors);
        P_LoadNodes(lumpNum + MapLumps.Nodes);
        P_LoadSegments(lumpNum + MapLumps.Segs);

        _rejectMatrix = DoomGame.Instance.WadData.GetLumpNum(lumpNum + MapLumps.Reject, PurgeTag.Level)!;
        P_GroupLines();

        _bodyQueueSlot = 0;
        _deathMatchStartIdx = 0;
        P_LoadThings(lumpNum + MapLumps.Things);

        // if deathmatch, randomly spawn the active players
        if (DeathMatch)
        {
            for (var i = 0; i < Constants.MaxPlayers; i++)
            {
                if (PlayerInGame[i])
                {
                    Players[i].MapObject = null;
                    DeathMatchSpawnPlayer(i);
                }
            }
        }

        // clear special respawning que
        // iquehead = iquetail = 0;

        // set up world state
        P_SpawnSpecials();

        // build subsector connect matrix
        //	UNUSED P_ConnectSubsectors ();

        // preload graphics
        if (PreCache)
        {
            DoomGame.Instance.Renderer.PreCacheLevel();
        }
    }

    private void P_InitSwitchList()
    {
        var episode = 1;
        if (DoomGame.Instance.GameMode == GameMode.Registered)
        {
            episode = 2;
        }
        else if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            episode = 3;
        }

        for (int i = 0, index = 0; i < Constants.MaxSwitches; i++)
        {
            if (SwitchControl.PredefinedSwitchList[i].Episode == 0)
            {
                _numSwitches = index / 2;
                _switchList[index] = -1;
                break;
            }

            if (SwitchControl.PredefinedSwitchList[i].Episode <= episode)
            {
                _switchList[index++] = DoomGame.Instance.Renderer.TextureNumForName(SwitchControl.PredefinedSwitchList[i].Name1);
                _switchList[index++] = DoomGame.Instance.Renderer.TextureNumForName(SwitchControl.PredefinedSwitchList[i].Name2);
            }
        }
    }

    public void P_Init()
    {
        P_InitSwitchList();
        P_InitPicAnims();
        DoomGame.Instance.Renderer.InitSprites(Constants.SpriteNames);
    }

    /// <summary>
    /// Function that changes wall texture.
    /// Tell it if switch is ok to use again (1=yes, it's a button).
    /// </summary>
    private void P_ChangeSwitchTexture(Line line, bool useAgain)
    {
        if (!useAgain)
        {
            line.Special = 0;
        }

        var texTop = _sides[line.SideNum[0]].TopTexture;
        var texMid = _sides[line.SideNum[0]].MidTexture;
        var texBot = _sides[line.SideNum[0]].BottomTexture;

        var sound = 0; // TODO sfx_swtchon

        // EXIT SWITCH?
        if (line.Special == 11)
        {
            // sound = sfx_swtchx;
        }

        for (var i = 0; i < _numSwitches * 2; i++)
        {
            if (_switchList[i] == texTop)
            {
                // S_StartSound(_buttonList->soundorg, sound);
                _sides[line.SideNum[0]].TopTexture = _switchList[i ^ 1];

                if (useAgain)
                {
                    P_StartButton(line, ButtonWhere.Top, _switchList[i], Constants.ButtonTime);
                }

                return;
            }

            if (_switchList[i] == texMid)
            {
                // S_StartSound(buttonlist->soundorg, sound);
                _sides[line.SideNum[0]].MidTexture = _switchList[i ^ 1];

                if (useAgain)
                {
                    P_StartButton(line, ButtonWhere.Middle, _switchList[i], Constants.ButtonTime);
                }

                return;
            }
                
            if (_switchList[i] == texBot)
            {
                // S_StartSound(buttonlist->soundorg, sound);
                _sides[line.SideNum[0]].BottomTexture = _switchList[i ^ 1];

                if (useAgain)
                {
                    P_StartButton(line, ButtonWhere.Bottom, _switchList[i], Constants.ButtonTime);
                }

                return;
            }
        }
    }

    /// <summary>
    /// Start a button counting down until it turns off.
    /// </summary>
    private void P_StartButton(Line line, ButtonWhere w, int texture, int time)
    {
        // See if button is already pressed
        for (var i = 0; i < Constants.MaxButtons; i++)
        {
            if (_buttonList[i].Timer != 0 && _buttonList[i].Line == line)
            {
                return;
            }
        }

        for (var i = 0; i < Constants.MaxButtons; i++)
        {
            if (_buttonList[i].Timer == 0)
            {
                _buttonList[i].Line = line;
                _buttonList[i].Where = w;
                _buttonList[i].Texture = texture;
                _buttonList[i].Timer = time;
                _buttonList[i].SoundOrigin = line.FrontSector?.SoundOrigin;
                return;
            }
        }

        DoomGame.Error("P_StartButton: no button slots left!");
    }

    private void P_InitPicAnims()
    {
        //	Init animation
        for (var i = 0; i < Constants.MaxAnimations; i++)
        {
            _animations[i] = new AnimatingItem();
        }

        for (var i = 0; i < AnimationDefinition.Definitions.Length; i++)
        {
            _lastAnimation = _animations[i];
            var animDef = AnimationDefinition.Definitions[i];
            if (animDef.IsTexture)
            {
                // different episode ?
                if (DoomGame.Instance.Renderer.CheckTextureNumForName(animDef.StartName) == -1)
                {
                    continue;
                }

                _lastAnimation.PicNum = DoomGame.Instance.Renderer.TextureNumForName(animDef.EndName);
                _lastAnimation.BasePic = DoomGame.Instance.Renderer.TextureNumForName(animDef.StartName);
            }
            else
            {
                if (DoomGame.Instance.WadData.CheckNumForName(animDef.StartName) == -1)
                {
                    continue;
                }

                _lastAnimation.PicNum = DoomGame.Instance.Renderer.FlatNumForName(animDef.EndName);
                _lastAnimation.BasePic = DoomGame.Instance.Renderer.FlatNumForName(animDef.StartName);
            }

            _lastAnimation.IsTexture = animDef.IsTexture;
            _lastAnimation.NumPics = _lastAnimation.PicNum - _lastAnimation.BasePic + 1;

            if (_lastAnimation.NumPics < 2)
            {
                DoomGame.Error($"P_InitPicAnims: bad cycle from {animDef.StartName} to {animDef.EndName}");
                return;
            }

            _lastAnimation.Speed = animDef.Speed;
        }
    }

    private void P_SpawnSpecials()
    {
        var episode = 1;
        if (DoomGame.Instance.WadData.CheckNumForName("TEXTURE2") >= 0)
        {
            episode = 2;
        }

        //// See if -TIMER needs to be used;
        //LevelTimer = false;

        //i = M_CheckParm("-avg");
        //if (i && deathmatch)
        //{
        //    levelTimer = true;
        //    levelTimeCount = 20 * 60 * 35;
        //}

        //i = M_CheckParm("-timer");
        //if (i && deathmatch)
        //{
        //    int time;
        //    time = atoi(myargv[i + 1]) * 60 * 35;
        //    levelTimer = true;
        //    levelTimeCount = time;
        //}

        //	Init special SECTORs.
        foreach (var sector in Sectors.Where(x => x.Special != 0))
        {
            switch (sector.Special)
            {
                case 1:
                    // FLICKERING LIGHTS
                    //P_SpawnLightFlash(sector);
                    break;

                case 2:
                    // STROBE FAST
                   // P_SpawnStrobeFlash(sector, FASTDARK, 0);
                    break;

                case 3:
                    // STROBE SLOW
                   // P_SpawnStrobeFlash(sector, SLOWDARK, 0);
                    break;

                case 4:
                    // STROBE FAST/DEATH SLIME
                   // P_SpawnStrobeFlash(sector, FASTDARK, 0);
                    sector.Special = 4;
                    break;

                case 8:
                    // GLOWING LIGHT
                   // P_SpawnGlowingLight(sector);
                    break;
                case 9:
                    // SECRET SECTOR
                    TotalSecrets++;
                    break;

                case 10:
                    // DOOR CLOSE IN 30 SECONDS
                   // P_SpawnDoorCloseIn30(sector);
                    break;

                case 12:
                    // SYNC STROBE SLOW
                   // P_SpawnStrobeFlash(sector, SLOWDARK, 1);
                    break;

                case 13:
                    // SYNC STROBE FAST
                   // P_SpawnStrobeFlash(sector, FASTDARK, 1);
                    break;

                case 14:
                    // DOOR RAISE IN 5 MINUTES
                   // P_SpawnDoorRaiseIn5Mins(sector, i);
                    break;

                case 17:
                   // P_SpawnFireFlicker(sector);
                    break;
            }
        }


        //	Init line EFFECTs
        _numLineSpecials = 0;
        foreach (var line in _lines.Where(x => x.Special == 48))
        {
            // EFFECT FIRSTCOL SCROLL+
            _lineSpecialList[_numLineSpecials++] = line;
            break;
        }

        //	Init other misc stuff
        for (var i = 0; i < Constants.MaxCeilings; i++)
        {
            _activeCeilings[i] = null;
        }

        for (var i = 0; i < Constants.MaxPlats; i++)
        {
            _activePlats[i] = null;
        }

        for (var i = 0; i < Constants.MaxButtons; i++)
        {
            _buttonList[i] = new Button();
        }

        // UNUSED: no horizonal sliders.
        //	P_InitSlidingDoorFrames();
    }

    /// <summary>
    /// Get info needed to make ticcmd_ts for the players
    /// </summary>
    public bool HandleEvent(InputEvent currentEvent)
    {
        // allow spy mode changes even during the demo
        if (GameState == GameState.Level && 
            currentEvent.Type == EventType.KeyDown &&
            currentEvent.Data1 == (int)Keys.F12 && 
            (SingleDemo || !DeathMatch))
        {
            // spy mode 
            do
            {
                DisplayPlayer++;
                if (DisplayPlayer == Constants.MaxPlayers)
                {
                    DisplayPlayer = 0;
                }
            } while (!PlayerInGame[DisplayPlayer] && DisplayPlayer != ConsolePlayer);
            
            return true;
        }

        // any other key pops up menu if in demos
        if (GameAction == GameAction.Nothing && 
            !SingleDemo &&
            (DemoPlayback || GameState == GameState.DemoScreen)
        )
        {
            if (currentEvent.Type == EventType.KeyDown ||
                (currentEvent.Type == EventType.Mouse && currentEvent.Data1 != 0) ||
                (currentEvent.Type == EventType.Joystick && currentEvent.Data1 != 0))
            {
                DoomGame.Instance.Menu.StartControlPanel();
                return true;
            }
            return false;
        }

        if (GameState == GameState.Level)
        {
            //if (HU_Responder(ev))
            //    return true;    // chat ate the event 
            //if (ST_Responder(ev))
            //    return true;    // status window ate it 
            //if (AM_Responder(ev))
            //    return true;    // automap ate it 
        }

        if (GameState == GameState.Finale)
        {
            //if (F_Responder(ev))
            //    return true;    // finale ate the event 
        }

        switch (currentEvent.Type)
        {
            case EventType.KeyDown:
                if (currentEvent.Data1 == (int)Keys.Pause)
                {
                    SendPause = true;
                    return true;
                }

                if (currentEvent.Data1 < NumKeys)
                {
                    _gameKeyDown[currentEvent.Data1] = true;
                }
                
                return true;    // eat key down events 

            case EventType.KeyUp:
                if (currentEvent.Data1 < NumKeys)
                {
                    _gameKeyDown[currentEvent.Data1] = false;
                }

                return false;   // always let key up events filter down 

            case EventType.Mouse:
                _mouseButtons[0] = (currentEvent.Data1 & 1) != 0;
                _mouseButtons[1] = (currentEvent.Data1 & 2) != 0;
                _mouseButtons[2] = (currentEvent.Data1 & 4) != 0;
                _mouseX = currentEvent.Data2 * (DoomGame.Instance.Menu.MouseSensitivity + 5) / 10;
                _mouseY = currentEvent.Data3 * (DoomGame.Instance.Menu.MouseSensitivity + 5) / 10;
                return true;    // eat events 

            case EventType.Joystick:
                _joyButtons[0] = (currentEvent.Data1 & 1) != 0;
                _joyButtons[1] = (currentEvent.Data1 & 2) != 0;
                _joyButtons[2] = (currentEvent.Data1 & 4) != 0;
                _joyButtons[3] = (currentEvent.Data1 & 8) != 0;
                _joyXMove = currentEvent.Data2;
                _joyYMove = currentEvent.Data3;
                return true;    // eat events 
        }

        return false;
    }

    /// <summary>
    /// Builds a ticcmd from all of the available inputs
    /// or reads it from the demo buffer. 
    /// If recording a demo, write it out 
    /// </summary>
    public TicCommand BuildTicCommand()
    {
        var cmd = new TicCommand(); // empty, or external driver

        cmd.Consistency = _consistency[ConsolePlayer][DoomGame.Instance.MakeTic % Constants.BackupTics];

        var strafe = _gameKeyDown[_keyStrafe] || _mouseButtons[_mouseButtonStrafe] || _joyButtons[_joyButtonStrafe];
        var speed = (_gameKeyDown[_keySpeed] || _joyButtons[_joyButtonSpeed]) ? 1 : 0;

        var turnSpeed = 0;
        var forward = 0;
        var side = 0;

        // use two stage accelerative turning
        // on the keyboard and joystick
        if (_joyXMove < 0 || _joyXMove > 0
                          || _gameKeyDown[_keyRight]
                          || _gameKeyDown[_keyLeft])
        {
            _turnHeld += DoomGame.Instance.TicDup;
        }
        else
        {
            _turnHeld = 0;
        }

        if (_turnHeld < SlowTurnTics)
        {
            turnSpeed = 2;             // slow turn 
        }
        else
        {
            turnSpeed = speed;
        }

        // let movement keys cancel each other out
        if (strafe)
        {
            if (_gameKeyDown[_keyRight])
            {
                // fprintf(stderr, "strafe right\n");
                side += SideMove[speed];
            }

            if (_gameKeyDown[_keyLeft])
            {
                //	fprintf(stderr, "strafe left\n");
                side -= SideMove[speed];
            }

            if (_joyXMove > 0)
            {
                side += SideMove[speed];
            }

            if (_joyXMove < 0)
            {
                side -= SideMove[speed];
            }

        }
        else
        {
            if (_gameKeyDown[_keyRight])
            {
                cmd.AngleTurn -= (short)AngleTurn[turnSpeed];
            }

            if (_gameKeyDown[_keyLeft])
            {
                cmd.AngleTurn += (short)AngleTurn[turnSpeed];
            }

            if (_joyXMove > 0)
            {
                cmd.AngleTurn -= (short)AngleTurn[turnSpeed];
            }

            if (_joyXMove < 0)
            {
                cmd.AngleTurn += (short)AngleTurn[turnSpeed];
            }
        }

        if (_gameKeyDown[_keyUp])
        {
            // fprintf(stderr, "up\n");
            forward += ForwardMove[speed];
        }
        
        if (_gameKeyDown[_keyDown])
        {
            // fprintf(stderr, "down\n");
            forward -= ForwardMove[speed];
        }

        if (_joyYMove < 0)
        {
            forward += ForwardMove[speed];
        }

        if (_joyYMove > 0)
        {
            forward -= ForwardMove[speed];
        }

        if (_gameKeyDown[_keyStrafeRight])
        {
            side += SideMove[speed];
        }

        if (_gameKeyDown[_keyStrafeLeft])
        {
            side -= SideMove[speed];
        }

        // buttons
        cmd.ChatChar = (char)0; // TODO HU_dequeueChatChar();

        if (_gameKeyDown[_keyFire] || _mouseButtons[_mouseButtonFire]
                                   || _joyButtons[_joyButtonFire])
        {
            cmd.Buttons |= ButtonCode.Attack;
        }

        if (_gameKeyDown[_keyUse] || _joyButtons[_joyButtonUse])
        {
            cmd.Buttons |= ButtonCode.Use;
            // clear double clicks if hit use button 
            _doubleClicks = 0;
        }

        // chainsaw overrides 
        for (var i = 0; i < (int)WeaponType.NumberOfWeapons - 1; i++)
        {
            if (!_gameKeyDown['1' + i])
            {
                continue;
            }

            cmd.Buttons |= ButtonCode.Change;
            cmd.Buttons |= (ButtonCode)(i << (int)ButtonCode.WeaponShift);
            break;
        }

        // mouse
        if (_mouseButtons[_mouseButtonForward])
        {
            forward += ForwardMove[speed];
        }

        // forward double click
        if (_mouseButtons[_mouseButtonForward] != _doubleClickState && _doubleClickTime > 1)
        {
            _doubleClickState = _mouseButtons[_mouseButtonForward];
            if (_doubleClickState)
            {
                _doubleClicks++;
            }

            if (_doubleClicks == 2)
            {
                cmd.Buttons |= ButtonCode.Use;
                _doubleClicks = 0;
            }
            else
            {
                _doubleClickTime = 0;
            }
        }
        else
        {
            _doubleClickTime += DoomGame.Instance.TicDup;
            if (_doubleClickTime > 20)
            {
                _doubleClicks = 0;
                _doubleClickState = false;
            }
        }

        // strafe double click
        var bstrafe = _mouseButtons[_mouseButtonStrafe] || _joyButtons[_joyButtonStrafe];
        if (bstrafe != _doubleClickState2 && _doubleClickTime2 > 1)
        {
            _doubleClickState2 = bstrafe;
            if (_doubleClickState2)
            {
                _doubleClicks2++;
            }

            if (_doubleClicks2 == 2)
            {
                cmd.Buttons |= ButtonCode.Use;
                _doubleClicks2 = 0;
            }
            else
            {
                _doubleClickTime2 = 0;
            }
        }
        else
        {
            _doubleClickTime2 += DoomGame.Instance.TicDup;
            if (_doubleClickTime2 > 20)
            {
                _doubleClicks2 = 0;
                _doubleClickState2 = false;
            }
        }

        forward += _mouseY;
        if (strafe)
        {
            side += _mouseX * 2;
        }
        else
        {
            cmd.AngleTurn -= (short)(_mouseX * 0x8);
        }

        _mouseX = _mouseY = 0;

        if (forward > ForwardMove[1]) // MAXPLMOVE = forwardmove[1]
        {
            forward = ForwardMove[1];
        }
        else if (forward < -ForwardMove[1])
        {
            forward = -ForwardMove[1];
        }

        if (side > ForwardMove[1])
        {
            side = ForwardMove[1];
        }
        else if (side < -ForwardMove[1])
        {
            side = -ForwardMove[1];
        }

        cmd.ForwardMove += (sbyte)forward;
        cmd.SideMove += (sbyte)side;

        // special buttons
        if (SendPause)
        {
            SendPause = false;
            cmd.Buttons = ButtonCode.Special | ButtonCode.Pause;
        }

        if (SendSave)
        {
            SendSave = false;
            cmd.Buttons = ButtonCode.Special | ButtonCode.SaveGame | (ButtonCode)(_saveGameSlot << (int)ButtonCode.SaveShift);
        }

        return cmd;
    }

    public void ExitLevel()
    {
        // _secretExit = false;
        GameAction = GameAction.Completed;
    }

    public int GetPlayerIndex(Player player)
    {
        for (var i = 0; i < Players.Length; i++)
        {
            if (Players[i] == player)
            {
                return i;
            }
        }

        return -1;
    }
}