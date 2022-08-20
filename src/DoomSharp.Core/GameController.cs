using System;

namespace DoomSharp.Core;

public class GameController
{
    private string _demoName = "";

    public void Ticker()
    {
        var buf = 0;
        //ticcmd_t* cmd;

        //// do player reborns if needed
        //for (i = 0; i < MAXPLAYERS; i++)
        //    if (playeringame[i] && players[i].playerstate == PST_REBORN)
        //        G_DoReborn(i);

        // do things to change the game state
        while (DoomGame.Instance.GameAction != GameAction.Nothing)
        {
            switch (DoomGame.Instance.GameAction)
            {
                case GameAction.LoadLevel:
                    // G_DoLoadLevel();
                    break;
                case GameAction.NewGame:
                    // G_DoNewGame();
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
                    DoomGame.Instance.GameAction = GameAction.Nothing;
                    break;
                case GameAction.Nothing:
                    break;
            }
        }

        // get commands, check consistancy,
        // and build new consistancy check
        buf = (DoomGame.Instance.GameTic / DoomGame.Instance.TicDup) % Constants.BackupTics;

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            //if (playeringame[i])
            //{
            //    cmd = &players[i].cmd;

            //    memcpy(cmd, &netcmds[i][buf], sizeof(ticcmd_t));

            //    if (demoplayback)
            //        G_ReadDemoTiccmd(cmd);
            //    if (demorecording)
            //        G_WriteDemoTiccmd(cmd);

            //    // check for turbo cheats
            //    if (cmd->forwardmove > TURBOTHRESHOLD
            //    && !(gametic & 31) && ((gametic >> 5) & 3) == i)
            //    {
            //        static char turbomessage[80];
            //        extern char* player_names[4];
            //        sprintf(turbomessage, "%s is turbo!", player_names[i]);
            //        players[consoleplayer].message = turbomessage;
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
            //}
        }

        // check for special buttons
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            //if (playeringame[i])
            //{
            //    if (players[i].cmd.buttons & BT_SPECIAL)
            //    {
            //        switch (players[i].cmd.buttons & BT_SPECIALMASK)
            //        {
            //            case BTS_PAUSE:
            //                paused ^= 1;
            //                if (paused)
            //                    S_PauseSound();
            //                else
            //                    S_ResumeSound();
            //                break;

            //            case BTS_SAVEGAME:
            //                if (!savedescription[0])
            //                    strcpy(savedescription, "NET GAME");
            //                savegameslot =
            //                (players[i].cmd.buttons & BTS_SAVEMASK) >> BTS_SAVESHIFT;
            //                gameaction = ga_savegame;
            //                break;
            //        }
            //    }
            //}
        }

        // do main actions
        switch (DoomGame.Instance.GameState)
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

    public void DeferedPlayDemo(string demo)
    {
        _demoName = demo;
        DoomGame.Instance.GameAction = GameAction.PlayDemo;
    }

    private void DoPlayDemo()
    {
        SkillLevel skill = SkillLevel.Easy;
        
        var episode = 1;
        var map = 1;

        DoomGame.Instance.GameAction = GameAction.Nothing;
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
        //consoleplayer = *demo_p++;

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

    private void InitNew(SkillLevel skill, int episode, int map)
    {
        if (DoomGame.Instance.Paused)
        {
            DoomGame.Instance.Paused = false;
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

        //M_ClearRandom();

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
            // players[i].playerstate = PST_REBORN;
        }

        //usergame = true;                // will be set false if a demo 
        //paused = false;
        //demoplayback = false;
        //automapactive = false;
        //viewactive = true;
        //gameepisode = episode;
        //gamemap = map;
        //gameskill = skill;

        //viewactive = true;

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

        //levelstarttic = gametic;        // for time calculation

        //if (wipegamestate == GS_LEVEL)
        //    wipegamestate = -1;             // force a wipe 

        // DoomGame.Instance.GameState = GameState.Level;

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            //if (playeringame[i] && players[i].playerstate == PST_DEAD)
            //    players[i].playerstate = PST_REBORN;
            //memset(players[i].frags, 0, sizeof(players[i].frags));
        }

        // P_SetupLevel(gameepisode, gamemap, 0, gameskill);
        //displayplayer = consoleplayer;		// view the guy you are playing    
        //starttime = I_GetTime();
        DoomGame.Instance.GameAction = GameAction.Nothing; 
        //Z_CheckHeap();

        //// clear cmd building stuff
        //memset(gamekeydown, 0, sizeof(gamekeydown));
        //joyxmove = joyymove = 0; 
        //mousex = mousey = 0; 
        //sendpause = sendsave = paused = false; 
        //memset(mousebuttons, 0, sizeof(mousebuttons));
        //memset(joybuttons, 0, sizeof(joybuttons));
    }
}