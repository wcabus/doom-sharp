using DoomSharp.Core.Data;
using DoomSharp.Core.GameLogic;
using DoomSharp.Core.Graphics;
using System.Numerics;
using DoomSharp.Core.Input;

namespace DoomSharp.Core.UI;

public class IntermissionController
{
    // https://github.com/id-Software/DOOM/blob/77735c3ff0772609e9c8d29e3ce2ab42ff54d20b/linuxdoom-1.10/wi_stuff.h
    // https://github.com/id-Software/DOOM/blob/77735c3ff0772609e9c8d29e3ce2ab42ff54d20b/linuxdoom-1.10/wi_stuff.c

    // used to accelerate or skip a stage
    private bool _accelerateStage;

    // wbs->pnum
    private int _me;

    // specifies current state
    private IntermissionState _state;

    // contains information passed into intermission
    private WorldMapInfo _wbs = new();
    private WorldMapPlayer[] _players = Array.Empty<WorldMapPlayer>(); // wbs->plyr[]

    // used for general timing
    private int _cnt;

    // used for timing of background animation
    private int _bcnt;

    // signals to refresh everything for one frame
    private bool _firstRefresh;

    private readonly int[] _cntKills = new int[Constants.MaxPlayers];
    private readonly int[] _cntItems = new int[Constants.MaxPlayers];
    private readonly int[] _cntSecret = new int[Constants.MaxPlayers];
    private int _cntTime;
    private int _cntPar;
    private int _cntPause;

    // # of commercial levels
    private int _numCommercialMaps;

    // GRAPHICS
    private Patch? _background;
    private readonly List<Patch> _levelNames = new();
    private readonly Patch?[] _yah = new Patch?[2];
    private Patch? _splat;
    private Patch? _wiminus;
    private Patch?[] _num = new Patch[10];
    private Patch? _percent;
    private Patch? _finished;
    private Patch? _entering;
    private Patch? _kills;
    private Patch? _secret;
    private Patch? _spSecret;
    private Patch? _items;
    private Patch? _frags;
    private Patch? _colon;
    private Patch? _time;
    private Patch? _sucks;
    private Patch? _par;
    private Patch? _killers;
    private Patch? _victims;
    private Patch? _total;
    private Patch? _star;
    private Patch? _bstar;
    private readonly Patch?[] _p = new Patch[Constants.MaxPlayers];
    private readonly Patch?[] _bp = new Patch[Constants.MaxPlayers];
    

    // Data needed to add patches to full screen intermission pics.
    // Patches are statistics messages, and animations.
    // Loads of by-pixel layout and placement, offsets etc.

    // Different vetween registered DOOM (1994) and
    //  Ultimate DOOM - Final edition (retail, 1995?).
    // This is supposedly ignored for commercial
    //  release (aka DOOM II), which had 34 maps
    //  in one episode. So there.
    private const int NumEpisodes = 4;
    private const int NumMaps = 9; // 9 maps per episode in DOOM 1

    // GLOBAL LOCATIONS
    private const int WI_TITLEY = 2;
    private const int WI_SPACINGY = 33;

    // SINGPLE-PLAYER STUFF
    private const int SP_STATSX = 50;
    private const int SP_STATSY = 50;

    private const int SP_TIMEX = 16;
    private const int SP_TIMEY = (Constants.ScreenHeight - 32);


    // NET GAME STUFF
    private const int NG_STATSY = 50;
    // private const int NG_STATSX = (32 + (_star.Width) / 2 + 32 * !dofrags);

    private const int NG_SPACINGX = 64;


    // DEATHMATCH STUFF
    private const int DM_MATRIXX = 42;
    private const int DM_MATRIXY = 68;

    private const int DM_SPACINGX = 40;

    private const int DM_TOTALSX = 269;

    private const int DM_KILLERSX = 10;
    private const int DM_KILLERSY = 100;
    private const int DM_VICTIMSX = 5;
    private const int DM_VICTIMSY = 50;

    // foreground
    private const int FB = 0;

    // States for single-player
    private const int SP_KILLS = 0;
    private const int SP_ITEMS = 2;
    private const int SP_SECRET = 4;
    private const int SP_FRAGS = 6;
    private const int SP_TIME = 8;
    // private const int SP_PAR = ST_TIME; ???

    private const int SP_PAUSE = 1;

    // in seconds
    private const int ShowNextLocationDelay = 4;

    public void Start(WorldMapInfo wmInfo)
    {
        InitVariables(wmInfo);
        LoadData();

        if (DoomGame.Instance.Game.DeathMatch)
        {
            InitDeathmatchStats();
        }
        else if (DoomGame.Instance.Game.NetGame)
        {
            InitNetgameStats();
        }
        else
        {
            InitStats();
        }
    }

    /// <summary>
    /// Updates stuff each tick
    /// </summary>
    public void Ticker()
    {
        // counter for general background animation
        _bcnt++;

        if (_bcnt == 1)
        {
            // intermission music
            if (DoomGame.Instance.GameMode == GameMode.Commercial)
            {
                //S_ChangeMusic(mus_dm2int, true);
            }
            else
            {
                //S_ChangeMusic(mus_inter, true);
            }
        }

        CheckForAccelerate();

        switch (_state)
        {
            case IntermissionState.StatCounting:
                if (DoomGame.Instance.Game.DeathMatch)
                {
                    UpdateDeathmatchStats();
                }
                else if (DoomGame.Instance.Game.NetGame)
                {
                    UpdateNetgameStats();
                }
                else
                {
                    UpdateStats();
                }

                break;

            case IntermissionState.ShowNextLocation:
                UpdateShowNextLocation();
                break;

            case IntermissionState.NoState:
                UpdateNoState();
                break;
        }
    }

    public void Drawer()
    {
        switch (_state)
        {
            case IntermissionState.StatCounting:
                if (DoomGame.Instance.Game.DeathMatch)
                {
                    DrawDeathmatchStats();
                }
                else if (DoomGame.Instance.Game.NetGame)
                {
                    DrawNetgameStats();
                }
                else
                {
                    DrawStats();
                }

                break;

            case IntermissionState.ShowNextLocation:
                DrawShowNextLocation();
                break;

            case IntermissionState.NoState:
                DrawNoState();
                break;
        }
    }

    private void InitVariables(WorldMapInfo wmInfo)
    {
        _wbs = wmInfo;
        _accelerateStage = false;
        _cnt = _bcnt = 0;
        _firstRefresh = true;
        _me = _wbs.PlayerNum;
        _players = _wbs.Players;

        if (_wbs.MaxKills == 0)
        {
            _wbs.MaxKills = 1;
        }

        if (_wbs.MaxItems == 0)
        {
            _wbs.MaxItems = 1;
        }

        if (_wbs.MaxSecret == 0)
        {
            _wbs.MaxSecret = 1;
        }

        if (DoomGame.Instance.GameMode != GameMode.Retail)
        {
            if (_wbs.Episode > 2)
            {
                _wbs.Episode -= 3;
            }
        }
    }

    private void LoadData()
    {
        var name = DoomGame.Instance.GameMode == GameMode.Commercial ? "INTERPIC" : $"WIMAP{_wbs.Episode}";

        if (DoomGame.Instance.GameMode == GameMode.Retail && _wbs.Episode == 3)
        {
            name = "INTERPIC";
        }

        // background
        _background = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName(name, PurgeTag.Cache)!);
        DoomGame.Instance.Video.DrawPatch(0, 0, 1, _background);

        // UNUSED unsigned char *pic = screens[1];
        // if (gamemode == commercial)
        // {
        // darken the background image
        // while (pic != screens[1] + SCREENHEIGHT*SCREENWIDTH)
        // {
        //   *pic = colormaps[256*25 + *pic];
        //   pic++;
        // }
        //}

        if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            _numCommercialMaps = 32;
            _levelNames.Clear();

            for (var i = 0; i < _numCommercialMaps; i++)
            {
                name = $"CWILV{i:00}";
                _levelNames.Add(Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName(name, PurgeTag.Static)!));
            }
        }
        else
        {
            _levelNames.Clear();

            for (var i = 0; i < NumMaps; i++)
            {
                name = $"WILV{_wbs.Episode}{i}";
                _levelNames.Add(Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName(name, PurgeTag.Static)!));
            }

            // you are here
            _yah[0] = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIURH0", PurgeTag.Static)!);

            // you are here (alt.)
            _yah[1] = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIURH1", PurgeTag.Static)!);

            // splat
            _splat = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WISPLAT", PurgeTag.Static)!);

            //if (_wbs.Episode < 3)
            //{
            //    for (var j = 0; j < NumAnims[_wbs.Episode]; j++)
            //    {
            //        var a = &anims[_wbs.Episode][j];
            //        for (var i = 0; i < a->nanims; i++)
            //        {
            //            // MONDO HACK!
            //            if (_wbs.Episode != 1 || j != 8)
            //            {
            //                // animations
            //                sprintf(name, "WIA%d%.2d%.2d", _wbs.Episode, j, i);
            //                a->p[i] = W_CacheLumpName(name, PU_STATIC);
            //            }
            //            else
            //            {
            //                // HACK ALERT!
            //                a->p[i] = anims[1][4].p[i];
            //            }
            //        }
            //    }
            //}
        }

        // More hacks on minus sign.
        _wiminus = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIMINUS", PurgeTag.Static)!);

        for (var i = 0; i < 10; i++)
        {
            // numbers 0-9
            _num[i] = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName($"WINUM{i}", PurgeTag.Static)!);
        }

        // percent sign
        _percent = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIPCNT", PurgeTag.Static)!);

        // "finished"
        _finished = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIF", PurgeTag.Static)!);

        // "entering"
        _entering = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIENTER", PurgeTag.Static)!);

        // "kills"
        _kills = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIOSTK", PurgeTag.Static)!);

        // "scrt"
        _secret = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIOSTS", PurgeTag.Static)!);

        // "secret"
        _spSecret = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WISCRT2", PurgeTag.Static)!);

        // Yuck. 
        //if (french)
        //{
        //    // "items"
        //    if (netgame && !deathmatch)
        //        items = W_CacheLumpName("WIOBJ", PU_STATIC);
        //    else
        //        items = W_CacheLumpName("WIOSTI", PU_STATIC);
        //}
        //else
        _items = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIOSTI", PurgeTag.Static)!);

        // "frgs"
        _frags = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIFRGS", PurgeTag.Static)!);

        // ":"
        _colon = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WICOLON", PurgeTag.Static)!);

        // "time"
        _time = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WITIME", PurgeTag.Static)!);

        // "sucks"
        _sucks = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WISUCKS", PurgeTag.Static)!);

        // "par"
        _par = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIPAR", PurgeTag.Static)!);

        // "killers" (vertical)
        _killers = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIKILRS", PurgeTag.Static)!);

        // "victims" (horiz)
        _victims = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIVCTMS", PurgeTag.Static)!);

        // "total"
        _total = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("WIMSTT", PurgeTag.Static)!);

        // your face
        _star = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("STFST01", PurgeTag.Static)!);

        // dead face
        _bstar = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName("STFDEAD0", PurgeTag.Static)!);

        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            // "1,2,3,4"
            _p[i] = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName($"STPB{i}", PurgeTag.Static)!);

            // "1,2,3,4"
            _bp[i] = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpName($"WIBP{i+1}", PurgeTag.Static)!);
        }
    }

    private int _spState;

    private void InitStats()
    {
        _state = IntermissionState.StatCounting;
        _accelerateStage = false;
        _spState = 1;
        _cntKills[0] = _cntItems[0] = _cntSecret[0] = -1;
        _cntTime = _cntPar = -1;
        _cntPause = Constants.TicRate;

        InitAnimatedBack();
    }

    private void InitDeathmatchStats()
    {
        // not supported. yet
    }

    private void InitNetgameStats()
    {
        // not supported. yet
    }

    private void InitAnimatedBack()
    {
        if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            return;
        }

        if (_wbs.Episode > 2)
        {
            return;
        }

        //for (i = 0; i < NUMANIMS[wbs->epsd]; i++)
        //{
        //    a = &anims[wbs->epsd][i];

        //    // init variables
        //    a->ctr = -1;

        //    // specify the next time to draw it
        //    if (a->type == ANIM_ALWAYS)
        //        a->nexttic = bcnt + 1 + (M_Random() % a->period);
        //    else if (a->type == ANIM_RANDOM)
        //        a->nexttic = bcnt + 1 + a->data2 + (M_Random() % a->data1);
        //    else if (a->type == ANIM_LEVEL)
        //        a->nexttic = bcnt + 1;
        //}
    }

    private void UpdateAnimatedBack()
    {
        if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            return;
        }

        if (_wbs.Episode > 2)
        {
            return;
        }
    }

    private void CheckForAccelerate()
    {
        // check for button presses to skip delays
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            if (DoomGame.Instance.Game.PlayerInGame[i])
            {
                var player = DoomGame.Instance.Game.Players[i];

                if ((player.Command.Buttons & ButtonCode.Attack) != 0)
                {
                    if (!player.AttackDown)
                    {
                        _accelerateStage = true;
                    }
                    player.AttackDown = true;
                }
                else
                {
                    player.AttackDown = false;
                }

                if ((player.Command.Buttons & ButtonCode.Use) != 0)
                {
                    if (!player.UseDown)
                    {
                        _accelerateStage = true;
                    }
                    player.UseDown = true;
                }
                else
                {
                    player.UseDown = false;
                }
            }
        }
    }

    private void UpdateDeathmatchStats()
    {

    }

    private void UpdateNetgameStats()
    {

    }

    private void UpdateStats()
    {
        UpdateAnimatedBack();

        if (_accelerateStage && _spState != 10)
        {
            _accelerateStage = false;
            _cntKills[0] = (_players[_me].Kills * 100) / _wbs.MaxKills;
            _cntItems[0] = (_players[_me].Items * 100) / _wbs.MaxItems;
            _cntSecret[0] = (_players[_me].Secret * 100) / _wbs.MaxSecret;
            _cntTime = _players[_me].Time / Constants.TicRate;
            _cntPar = _wbs.ParTime / Constants.TicRate;
            // S_StartSound(0, sfx_barexp);
            _spState = 10;
        }

        if (_spState == 2)
        {
            _cntKills[0] += 2;

            if ((_bcnt & 3) == 0)
            {
                // S_StartSound(0, sfx_pistol);
            }

            if (_cntKills[0] >= (_players[_me].Kills * 100) / _wbs.MaxKills)
            {
                _cntKills[0] = (_players[_me].Kills * 100) / _wbs.MaxKills;
                // S_StartSound(0, sfx_barexp);
                _spState++;
            }
        }
        else if (_spState == 4)
        {
            _cntItems[0] += 2;

            if ((_bcnt & 3) == 0)
            {
                // S_StartSound(0, sfx_pistol);
            }

            if (_cntItems[0] >= (_players[_me].Items * 100) / _wbs.MaxItems)
            {
                _cntItems[0] = (_players[_me].Items * 100) / _wbs.MaxItems;
                // S_StartSound(0, sfx_barexp);
                _spState++;
            }
        }
        else if (_spState == 6)
        {
            _cntSecret[0] += 2;

            if ((_bcnt & 3) == 0)
            {
                // S_StartSound(0, sfx_pistol);
            }

            if (_cntSecret[0] >= (_players[_me].Secret * 100) / _wbs.MaxSecret)
            {
                _cntSecret[0] = (_players[_me].Secret * 100) / _wbs.MaxSecret;
                // S_StartSound(0, sfx_barexp);
                _spState++;
            }
        }
        else if (_spState == 8)
        {
            if ((_bcnt & 3) == 0)
            {
                // S_StartSound(0, sfx_pistol);
            }

            _cntTime += 3;

            if (_cntTime >= _players[_me].Time / Constants.TicRate)
            {
                _cntTime = _players[_me].Time / Constants.TicRate;
            }

            _cntPar += 3;

            if (_cntPar >= _wbs.ParTime / Constants.TicRate)
            {
                _cntPar = _wbs.ParTime / Constants.TicRate;

                if (_cntTime >= _players[_me].Time / Constants.TicRate)
                {
                    // S_StartSound(0, sfx_barexp);
                    _spState++;
                }
            }
        }
        else if (_spState == 10)
        {
            if (_accelerateStage)
            {
                // S_StartSound(0, sfx_sgcock);

                if (DoomGame.Instance.GameMode == GameMode.Commercial)
                {
                    InitNoState();
                }
                else
                {
                    InitShowNextLocation();
                }
            }
        }
        else if ((_spState & 1) != 0)
        {
            if (--_cntPause == 0)
            {
                _spState++;
                _cntPause = Constants.TicRate;
            }
        }
    }

    private void InitNoState()
    {
        _state = IntermissionState.NoState;
        _accelerateStage = false;
        _cnt = 10;
    }

    private void UpdateNoState()
    {
        UpdateAnimatedBack();

        if (--_cnt == 0)
        {
            EndIntermission();
            DoomGame.Instance.Game.WorldDone();
        }
    }

    private void EndIntermission()
    {
        // unload data
    }

    private bool _pointerOn = false;

    private void InitShowNextLocation()
    {
        _state = IntermissionState.ShowNextLocation;
        _accelerateStage = false;
        _cnt = ShowNextLocationDelay * Constants.TicRate;

        InitAnimatedBack();
    }

    private void UpdateShowNextLocation()
    {
        UpdateAnimatedBack();

        if (--_cnt == 0 || _accelerateStage)
        {
            InitNoState();
        }
        else
        {
            _pointerOn = (_cnt & 31) < 20;
        }
    }

    private void DrawDeathmatchStats()
    {

    }

    private void DrawNetgameStats()
    {

    }

    private void DrawStats()
    {
        // line height
        var lh = (3 * (_num[0]!.Height)) / 2;

        SlamBackground();

        // draw animated background
        DrawAnimatedBackground();

        DrawLevelFinished();

        DoomGame.Instance.Video.DrawPatch(SP_STATSX, SP_STATSY, FB, _kills!);
        DrawPercent(Constants.ScreenWidth - SP_STATSX, SP_STATSY, _cntKills[0]);

        DoomGame.Instance.Video.DrawPatch(SP_STATSX, SP_STATSY + lh, FB, _items!);
        DrawPercent(Constants.ScreenWidth - SP_STATSX, SP_STATSY + lh, _cntItems[0]);

        DoomGame.Instance.Video.DrawPatch(SP_STATSX, SP_STATSY + 2 * lh, FB, _spSecret!);
        DrawPercent(Constants.ScreenWidth - SP_STATSX, SP_STATSY + 2 * lh, _cntSecret[0]);

        DoomGame.Instance.Video.DrawPatch(SP_TIMEX, SP_TIMEY, FB, _time!);
        DrawTime(Constants.ScreenWidth / 2 - SP_TIMEX, SP_TIMEY, _cntTime);

        if (_wbs.Episode < 3)
        {
            DoomGame.Instance.Video.DrawPatch(Constants.ScreenWidth / 2 + SP_TIMEX, SP_TIMEY, FB, _par!);
            DrawTime(Constants.ScreenWidth - SP_TIMEX, SP_TIMEY, _cntPar);
        }
    }

    private void SlamBackground()
    {
        Array.Copy(DoomGame.Instance.Video.Screens[1], 0, DoomGame.Instance.Video.Screens[0], 0, Constants.ScreenWidth * Constants.ScreenHeight);
        DoomGame.Instance.Video.MarkRectangle(0, 0, Constants.ScreenWidth, Constants.ScreenHeight);
    }

    private void DrawAnimatedBackground()
    {
        if (DoomGame.Instance.GameMode == GameMode.Commercial)
        {
            return;
        }

        if (_wbs.Episode > 2)
        {
            return;
        }

        //for (i = 0; i < NUMANIMS[wbs->epsd]; i++)
        //{
        //    a = &anims[wbs->epsd][i];

        //    if (a->ctr >= 0)
        //        V_DrawPatch(a->loc.x, a->loc.y, FB, a->p[a->ctr]);
        //}
    }

    /// <summary>
    /// Draws "LEVELNAME finished!"
    /// </summary>
    private void DrawLevelFinished()
    {
        var y = WI_TITLEY;

        // draw level name
        DoomGame.Instance.Video.DrawPatch((Constants.ScreenWidth - _levelNames[_wbs.Last].Width) / 2, y, FB, _levelNames[_wbs.Last]);

        // draw Finished!
        y += 5 * _levelNames[_wbs.Last].Height / 4;

        DoomGame.Instance.Video.DrawPatch((Constants.ScreenWidth - _finished!.Width) / 2, y, FB, _finished);
    }

    private void DrawPercent(int x, int y, int p)
    {
        if (p < 0)
        {
            return;
        }

        DoomGame.Instance.Video.DrawPatch(x, y, FB, _percent!);
        DrawNum(x, y, p, -1);
    }

    /// <summary>
    /// Draws a number.
    /// If digits > 0, then use that many digits minimum,
    ///  otherwise only use as many as necessary.
    /// Returns new x position.
    /// </summary>
    private int DrawNum(int x, int y, int n, int digits)
    {
        var fontWidth = _num[0]!.Width;

        if (digits < 0)
        {
            if (n == 0)
            {
                // make variable-length zeros 1 digit long
                digits = 1;
            }
            else
            {
                // figure out # of digits in #
                digits = 0;
                var temp = n;

                while (temp != 0)
                {
                    temp /= 10;
                    digits++;
                }
            }
        }

        var neg = n < 0;
        if (neg)
        {
            n = -n;
        }

        // if non-number, do not draw it
        if (n == 1994)
        {
            return 0;
        }

        // draw the new number
        while (digits-- != 0)
        {
            x -= fontWidth;
            DoomGame.Instance.Video.DrawPatch(x, y, FB, _num[n % 10]!);
            n /= 10;
        }

        // draw a minus sign if necessary
        if (neg)
        {
            DoomGame.Instance.Video.DrawPatch(x -= 8, y, FB, _wiminus!);
        }

        return x;
    }

    /// <summary>
    /// Display level completion time and par,
    ///  or "sucks" message if overflow.
    /// </summary>
    private void DrawTime(int x, int y, int t)
    {
        if (t < 0)
        {
            return;
        }

        if (t <= 61 * 59)
        {
            var div = 1;

            do
            {
                var n = (t / div) % 60;
                x = DrawNum(x, y, n, 2) - _colon!.Width;
                div *= 60;

                // draw
                if (div == 60 || (t / div) != 0)
                {
                    DoomGame.Instance.Video.DrawPatch(x, y, FB, _colon);
                }

            } while (t / div != 0);
        }
        else
        {
            // "sucks"
            DoomGame.Instance.Video.DrawPatch(x - _sucks!.Width, y, FB, _sucks);
        }
    }

    private void DrawShowNextLocation()
    {

    }

    private void DrawNoState()
    {

    }
}