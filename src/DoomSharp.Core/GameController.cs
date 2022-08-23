using DoomSharp.Core.Data;
using DoomSharp.Core.GameLogic;
using DoomSharp.Core.Input;
using DoomSharp.Core.Networking;
using System;

namespace DoomSharp.Core;

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

    public GameController()
    {
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            InitPlayer(i);
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
    public int StartTime { get; set; }

    public bool ViewActive { get; set; }

    public bool DeathMatch { get; set; }
    public bool NetGame { get; set; }
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
        buf = (GameTic / DoomGame.Instance.TicDup) % Constants.BackupTics;

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
                //P_Ticker();
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
        SkillLevel skill = SkillLevel.Easy;
        
        var episode = 1;
        var map = 1;

        GameAction = GameAction.Nothing;
        //demobuffer = demo_p = DoomGame.Instance.WadData.GetLumpName(_demoName, Constants.PuStatic);
        //if (*demo_p++ != VERSION)
        //{
        //    fprintf(stderr, "Demo is from a different game version!\n");
        //    DoomGame.Instance.GameAction = GameAction.Nothing;
        //    return;
        //}

        //skill = *demo_p++;
        //episode = *demo_p++;
        //map = *demo_p++;
        //deathmatch = *demo_p++;
        //respawnparm = *demo_p++;
        //fastparm = *demo_p++;
        //nomonsters = *demo_p++;
        //ConsolePlayer = *demo_p++;

        //for (i = 0; i < MAXPLAYERS; i++)
        //    playeringame[i] = *demo_p++;
        //if (playeringame[1])
        //{
        //    netgame = true;
        //    netdemo = true;
        //}

        //// don't spend a lot of time in loadlevel 
        //precache = false;
        InitNew(skill, episode, map);
        //precache = true;

        //usergame = false;
        //demoplayback = true;
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

        if ((map > 9) && (DoomGame.Instance.GameMode != GameMode.Commercial))
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
        //if (DoomGame.Instance.GameMode == GameMode.Commercial)
        //{
        //    skytexture = R_TextureNumForName("SKY3");
        //    if (gamemap < 12)
        //        skytexture = R_TextureNumForName("SKY1");
        //    else
        //    if (gamemap < 21)
        //        skytexture = R_TextureNumForName("SKY2");
        //}
        //else
        //{
        //    switch (episode)
        //    {
        //        case 1:
        //            skytexture = R_TextureNumForName("SKY1");
        //            break;
        //        case 2:
        //            skytexture = R_TextureNumForName("SKY2");
        //            break;
        //        case 3:
        //            skytexture = R_TextureNumForName("SKY3");
        //            break;
        //        case 4: // Special Edition sky
        //            skytexture = R_TextureNumForName("SKY4");
        //            break;
        //    }
        //}

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
        _demoData[_demoDataIdx++] = (byte)((cmd.AngleTurn + 128) >> 8);
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
        //// Set the sky map.
        //// First thing, we have a dummy sky texture name,
        ////  a flat. The data is in the WAD only because
        ////  we look for an actual index, instead of simply
        ////  setting one.
        //skyflatnum = R_FlatNumForName(SKYFLATNAME);

        //// DOOM determines the sky texture to be used
        //// depending on the current episode, and the game version.
        //if ((gamemode == commercial)
        //    || (gamemode == pack_tnt)
        //    || (gamemode == pack_plut))
        //{
        //    skytexture = R_TextureNumForName("SKY3");
        //    if (gamemap < 12)
        //        skytexture = R_TextureNumForName("SKY1");
        //    else
        //    if (gamemap < 21)
        //        skytexture = R_TextureNumForName("SKY2");
        //}

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
            //memset(players[i].frags, 0, sizeof(players[i].frags));
        }

        // P_SetupLevel(gameepisode, gamemap, 0, gameskill);
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

    private bool CheckDemoStatus()
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
}