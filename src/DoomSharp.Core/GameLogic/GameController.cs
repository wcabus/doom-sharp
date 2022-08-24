using System.Text;
using DoomSharp.Core.Data;
using DoomSharp.Core.Extensions;
using DoomSharp.Core.Input;
using DoomSharp.Core.Networking;
using DoomSharp.Core.Graphics;
using System;
using System.Xml.Linq;

namespace DoomSharp.Core.GameLogic;

public class GameController
{
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

    private Sky _sky = new();

    private int _levelTime;
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
    private MapObject[][] _blockLinks = Array.Empty<MapObject[]>();

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

    public GameController()
    {
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

    public const int NumKeys = 256;
    public bool[] GameKeyDown { get; } = new bool[256];

    public void Ticker()
    {
        var buf = 0;
        TicCommand cmd;

        // do player reborns if needed
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i] && Players[i].State == PlayerState.Reborn)
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
                cmd = Players[i].Command;

                //    memcpy(cmd, &netcmds[i][buf], sizeof(ticcmd_t));
                if (DemoPlayback)
                {
                    ReadDemoTicCommand(cmd);
                }

                if (DemoRecording)
                {
                    WriteDemoTicCommand(cmd);
                }

                //    // check for turbo cheats
                //    if (cmd->forwardmove > TURBOTHRESHOLD
                //    && !(gametic & 31) && ((gametic >> 5) & 3) == i)
                //    {
                //        static char turbomessage[80];
                //        extern char* player_names[4];
                //        sprintf(turbomessage, "%s is turbo!", player_names[i]);
                //        players[ConsolePlayer].message = turbomessage;
                //    }

                //    if (netgame && !netdemo && !(gametic % ticdup))
                //    {
                //        if (gametic > BACKUPTICS
                //            && consistancy[i][buf] != cmd->consistancy)
                //        {
                //            I_Error("consistency failure (%i should be %i)",
                //                 cmd->consistancy, consistancy[i][buf]);
                //        }
                //        if (players[i].mo)
                //            consistancy[i][buf] = players[i].mo->x;
                //        else
                //            consistancy[i][buf] = rndindex;
                //    }
            }
        }

        // check for special buttons
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (PlayerInGame[i])
            {
                if ((Players[i].Command.Buttons & (int)ButtonCode.Special) != 0)
                {
                    switch (Players[i].Command.Buttons & (int)ButtonCode.SpecialMask)
                    {
                        case (int)ButtonCode.Pause:
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

                        case (int)ButtonCode.SaveGame:
                            if (string.IsNullOrWhiteSpace(_saveDescription))
                            {
                                _saveDescription = "NET GAME";
                            }

                            _saveGameSlot = (Players[i].Command.Buttons & (int)ButtonCode.SaveMask) >> (int)ButtonCode.SaveShift;
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
                //ST_Ticker();
                //AM_Ticker();
                //HU_Ticker();
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

    private void PlayerReborn(int player)
    {

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
            // Players[player].mo->player = NULL;

            // spawn at random spot if in death match 
            if (DeathMatch)
            {
                // G_DeathMatchSpawnPlayer(playernum);
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
            Players[i].State = PlayerState.Reborn;
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

        // set the sky map for the episode
        if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY3");
            if (GameMap < 12)
            {
                _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY1");
            }
            else if (GameMap < 21)
            {
                _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY2");
            }
        }
        else
        {
            switch (episode)
            {
                case 1:
                    _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY1");
                    break;
                case 2:
                    _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY2");
                    break;
                case 3:
                    _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY3");
                    break;
                case 4: // Special Edition sky
                    _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY4");
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
        cmd.AngleTurn = (byte)(_demoData[_demoDataIdx++] << 8);
        cmd.Buttons = _demoData[_demoDataIdx++];
    }

    private void WriteDemoTicCommand(TicCommand cmd)
    {
        if (GameKeyDown['q']) // press q to end demo recording 
        {
            CheckDemoStatus();
        }

        _demoData[_demoDataIdx++] = (byte)cmd.ForwardMove;
        _demoData[_demoDataIdx++] = (byte)cmd.SideMove;
        _demoData[_demoDataIdx++] = (byte)(cmd.AngleTurn + 128 >> 8);
        _demoData[_demoDataIdx++] = cmd.Buttons;

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
        _sky.FlatNum = DoomGame.Instance.Renderer.FlatNumForName(Sky.FlatName);
        
        // DOOM determines the sky texture to be used
        // depending on the current episode, and the game version.
        if (DoomGame.Instance.GameMode == GameMode.Commercial) // or Pack TNT/Plutonium
        {
            _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY3");
            if (GameMap < 12)
            {
                _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY1");
            }
            else if (GameMap < 21)
            {
                _sky.Texture = DoomGame.Instance.Renderer.TextureNumForName("SKY2");
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
            if (PlayerInGame[i] && Players[i].State == PlayerState.Dead)
            {
                Players[i].State = PlayerState.Reborn;
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
        thinker.Acv = null;
        thinker.Acp1 = null;
        thinker.Acp2 = null;
    }

    private void RunThinkers()
    {
        var thinkerNode = _thinkers.First;

        while (thinkerNode != null)
        {
            var thinker = thinkerNode.Value;
            if (thinker.Acv == null)
            {
                // Time to remove this thinker
                var next = thinkerNode.Next;
                _thinkers.Remove(thinkerNode);
                thinkerNode = next;
            }
            else
            {
                thinker.Acp1?.Invoke(thinker);
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
            && Players[ConsolePlayer].ViewZ != new Fixed(1))
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
        _levelTime++;
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

    private void P_LoadSegs(int lump)
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
            // P_SpawnMapThing(mt);
        }
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

            if (ld.Dx.IntVal == 0)
            {
                ld.SlopeType = SlopeType.Vertical;
            }
            else if (ld.Dy.IntVal != 0)
            {
                ld.SlopeType = SlopeType.Horizontal;
            }
            else
            {
                ld.SlopeType = Fixed.Div(ld.Dy, ld.Dx).IntVal > 0 ? SlopeType.Positive : SlopeType.Negative;
            }

            if (ld.V1.X.IntVal < ld.V2.X.IntVal)
            {
                ld.BoundingBox[BoundingBox.BoxLeft] = ld.V1.X;
                ld.BoundingBox[BoundingBox.BoxRight] = ld.V2.X;
            }
            else
            {
                ld.BoundingBox[BoundingBox.BoxLeft] = ld.V2.X;
                ld.BoundingBox[BoundingBox.BoxRight] = ld.V1.X;
            }

            if (ld.V1.Y.IntVal < ld.V2.Y.IntVal)
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

    /*
    // A LineDef, as used for editing, and as input
    // to the BSP builder.
    typedef struct
    {
      short		v1;
      short		v2;
      short		flags;
      short		special;
      short		tag;
      // sidenum[1] will be -1 if one sided
      short		sidenum[2];		
    } maplinedef_t;
     */

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
        _blockLinks = new MapObject[_blockMapWidth][];
        for (var i = 0; i < _blockMapWidth; i++)
        {
            _blockLinks[i] = new MapObject[_blockMapHeight];
        }
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
            sector.SoundOrigin.X = new Fixed((boundingBox[BoundingBox.BoxRight].IntVal + boundingBox[BoundingBox.BoxLeft].IntVal) / 2);
            sector.SoundOrigin.Y = new Fixed((boundingBox[BoundingBox.BoxTop].IntVal + boundingBox[BoundingBox.BoxBottom].IntVal) / 2);

            // adjust bounding box to map blocks
            var block = (boundingBox[BoundingBox.BoxTop].IntVal - _blockMapOriginY.IntVal + Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block >= _blockMapHeight ? _blockMapHeight - 1 : block;
            sector.BlockBox[BoundingBox.BoxTop] = block;

            block = (boundingBox[BoundingBox.BoxBottom].IntVal - _blockMapOriginY.IntVal - Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block < 0 ? 0 : block;
            sector.BlockBox[BoundingBox.BoxBottom] = block;

            block = (boundingBox[BoundingBox.BoxRight].IntVal - _blockMapOriginX.IntVal + Constants.MaxRadius) >> Constants.MapBlockShift;
            block = block >= _blockMapWidth ? _blockMapWidth - 1 : block;
            sector.BlockBox[BoundingBox.BoxRight] = block;

            block = (boundingBox[BoundingBox.BoxLeft].IntVal - _blockMapOriginX.IntVal - Constants.MaxRadius) >> Constants.MapBlockShift;
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
        Players[ConsolePlayer].ViewZ = new Fixed(1);

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

        _levelTime = 0;

        // note: most of this ordering is important	
        P_LoadBlockMap(lumpNum + MapLumps.BlockMap);
        P_LoadVertexes(lumpNum + MapLumps.Vertices);
        P_LoadSectors(lumpNum + MapLumps.Sectors);
        P_LoadSideDefs(lumpNum + MapLumps.SideDefs);

        P_LoadLineDefs(lumpNum + MapLumps.LineDefs);
        P_LoadSubSectors(lumpNum + MapLumps.SubSectors);
        P_LoadNodes(lumpNum + MapLumps.Nodes);
        P_LoadSegs(lumpNum + MapLumps.Segs);

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
                    // DeathMatchSpawnPlayer(i);
                }
            }
        }

        // clear special respawning que
        // iquehead = iquetail = 0;

        // set up world state
        // P_SpawnSpecials();

        // build subsector connect matrix
        //	UNUSED P_ConnectSubsectors ();

        // preload graphics
        if (PreCache)
        {
            // R_PrecacheLevel();
        }
    }

    public void P_Init()
    {
        //P_InitSwitchList();
        //P_InitPicAnims();
        //R_InitSprites(sprnames);
    }
}