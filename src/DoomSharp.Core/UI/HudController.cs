using DoomSharp.Core.Data;
using DoomSharp.Core.GameLogic;
using DoomSharp.Core.Graphics;
using DoomSharp.Core.Input;

namespace DoomSharp.Core.UI;

public class HudController
{
    public const char HuFontStart = '!';
    public const char HuFontEnd = '_';
    public const int HuFontSize = HuFontEnd - HuFontStart + 1;

    public const int HuMaxLineLength = 80;
    public const int HuMaxLines = 4;

    public const int Broadcast = 5;

    public const int MessageRefresh = (int)Keys.Enter;
    public const int MessageX = 0;
    public const int MessageY = 0;
    public const int MessageWidth = 64; // width in characters
    public const int MessageHeight = 1; // in lines

    public const int TitleX = 0;

    public const int MessageTimeout = Constants.TicRate * 4;

    // DOOM shareware/registered/retail (Ultimate) names.
    public static readonly List<string> MapNames;

    // DOOM 2 map names.
    public static readonly List<string> MapNames2;

    private Player? _player;
    private bool _chatOn;

    private bool _messageOn;
    public bool MessageDontFuckWithMe { get; set; }
    private bool _messageNotToBeFuckedWith;

    private int _messageCounter;

    // Message text storage (replaces hu_stext_t w_message)
    private string _messageText = "";
    private int _messageNeedsUpdate;

    // Map title text storage (replaces hu_textline_t w_title)
    private string _titleText = "";
    private int _titleY;
    private int _titleNeedsUpdate;
    private bool _lastAutomapActive;

    static HudController()
    {
        MapNames = new List<string>(new[]
        {
            Messages.HUSTR_E1M1,
            Messages.HUSTR_E1M2,
            Messages.HUSTR_E1M3,
            Messages.HUSTR_E1M4,
            Messages.HUSTR_E1M5,
            Messages.HUSTR_E1M6,
            Messages.HUSTR_E1M7,
            Messages.HUSTR_E1M8,
            Messages.HUSTR_E1M9,

            Messages.HUSTR_E2M1,
            Messages.HUSTR_E2M2,
            Messages.HUSTR_E2M3,
            Messages.HUSTR_E2M4,
            Messages.HUSTR_E2M5,
            Messages.HUSTR_E2M6,
            Messages.HUSTR_E2M7,
            Messages.HUSTR_E2M8,
            Messages.HUSTR_E2M9,

            Messages.HUSTR_E3M1,
            Messages.HUSTR_E3M2,
            Messages.HUSTR_E3M3,
            Messages.HUSTR_E3M4,
            Messages.HUSTR_E3M5,
            Messages.HUSTR_E3M6,
            Messages.HUSTR_E3M7,
            Messages.HUSTR_E3M8,
            Messages.HUSTR_E3M9,

            Messages.HUSTR_E4M1,
            Messages.HUSTR_E4M2,
            Messages.HUSTR_E4M3,
            Messages.HUSTR_E4M4,
            Messages.HUSTR_E4M5,
            Messages.HUSTR_E4M6,
            Messages.HUSTR_E4M7,
            Messages.HUSTR_E4M8,
            Messages.HUSTR_E4M9,

            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL",
            "NEWLEVEL"
        });

        MapNames2 = new List<string>(new[]
        {
            Messages.HUSTR_1,
            Messages.HUSTR_2,
            Messages.HUSTR_3,
            Messages.HUSTR_4,
            Messages.HUSTR_5,
            Messages.HUSTR_6,
            Messages.HUSTR_7,
            Messages.HUSTR_8,
            Messages.HUSTR_9,
            Messages.HUSTR_10,
            Messages.HUSTR_11,

            Messages.HUSTR_12,
            Messages.HUSTR_13,
            Messages.HUSTR_14,
            Messages.HUSTR_15,
            Messages.HUSTR_16,
            Messages.HUSTR_17,
            Messages.HUSTR_18,
            Messages.HUSTR_19,
            Messages.HUSTR_20,
            
            Messages.HUSTR_21,
            Messages.HUSTR_22,
            Messages.HUSTR_23,
            Messages.HUSTR_24,
            Messages.HUSTR_25,
            Messages.HUSTR_26,
            Messages.HUSTR_27,
            Messages.HUSTR_28,
            Messages.HUSTR_29,
            Messages.HUSTR_30,
            Messages.HUSTR_31,
            Messages.HUSTR_32
        });
    }

    public HudController()
    {
        // Load the heads-up font
        var j = (int)HuFontStart;
        for (var i = 0; i < HuFontSize; i++)
        {
            var lump = DoomGame.Instance.WadData.GetLumpName($"STCFN{j++:000}", PurgeTag.Cache)!;
            Font[i] = Patch.FromBytes(lump);
        }
    }

    public Patch[] Font { get; } = new Patch[HuFontSize];

    public bool HeadsUpActive { get; private set; }

    public void Stop()
    {
        HeadsUpActive = false;
    }

    public void Start()
    {
        var game = DoomGame.Instance.Game;

        if (HeadsUpActive)
        {
            Stop();
        }

        _player = game.Players[DoomGame.Instance.Game.ConsolePlayer];
        _messageOn = false;
        MessageDontFuckWithMe = false;
        _messageNotToBeFuckedWith = false;
        _chatOn = false;

        // Clear message text
        _messageText = "";
        _messageNeedsUpdate = 0;

        // Calculate title Y position: 167 - font height
        // In original DOOM: HU_TITLEY = (167 - SHORT(hu_font[0]->height))
        _titleY = 167 - Font[0].Height;

        // Build map title string
        var mapName = "";
        switch (DoomGame.Instance.GameMode)
        {
            case GameMode.Shareware:
            case GameMode.Registered:
            case GameMode.Retail:
                mapName = MapNames[(game.GameEpisode - 1) * 9 + game.GameMap - 1];
                break;
            case GameMode.Commercial:
            default:
                mapName = MapNames2[game.GameMap - 1];
                break;
        }

        _titleText = mapName;
        _titleNeedsUpdate = 4; // needs initial draw

        // create the inputbuffer widgets
        for (var i = 0; i < Constants.MaxPlayers; i++)
        {
            // HUlib_initIText(&w_inputbuffer[i], 0, 0, 0, 0, &always_off);
        }

        HeadsUpActive = true;
    }

    public void Drawer()
    {
        // Draw the message if on
        DrawMessage();

        // Draw map title when automap is active
        if (DoomGame.Instance.Game.AutomapActive)
        {
            DrawTextLine(_titleText, TitleX, _titleY);
        }
    }

    public void Erase()
    {
        // Erase the message text
        EraseTextLine(MessageY, _messageNeedsUpdate, ref _messageOn);
        if (_messageNeedsUpdate > 0)
        {
            _messageNeedsUpdate--;
        }

        // Erase the title text
        var automapActive = DoomGame.Instance.Game.AutomapActive;
        if (_lastAutomapActive && !automapActive)
        {
            _titleNeedsUpdate = 4;
        }
        EraseTextLine(_titleY, _titleNeedsUpdate);
        if (_titleNeedsUpdate > 0)
        {
            _titleNeedsUpdate--;
        }
        _lastAutomapActive = automapActive;
    }

    /// <summary>
    /// Erases a text line by copying from the background screen (screen 1) to the
    /// front screen (screen 0). This is the equivalent of HUlib_eraseTextLine.
    /// Only erases when NOT in automap and the screen is reduced (viewwindowx != 0).
    /// </summary>
    private void EraseTextLine(int y, int needsUpdate)
    {
        var renderer = DoomGame.Instance.Renderer;

        if (!DoomGame.Instance.Game.AutomapActive &&
            renderer.ViewWindowX != 0 && needsUpdate > 0)
        {
            var lh = Font[0].Height + 1;
            for (var row = y; row < y + lh; row++)
            {
                var yoffset = row * Constants.ScreenWidth;

                if (row < renderer.ViewWindowY || row >= renderer.ViewWindowY + renderer.ViewHeight)
                {
                    // erase entire line
                    Array.Copy(DoomGame.Instance.Video.Screens[1], yoffset,
                        DoomGame.Instance.Video.Screens[0], yoffset, Constants.ScreenWidth);
                }
                else
                {
                    // erase left border
                    Array.Copy(DoomGame.Instance.Video.Screens[1], yoffset,
                        DoomGame.Instance.Video.Screens[0], yoffset, renderer.ViewWindowX);
                    // erase right border
                    var rightOfs = yoffset + renderer.ViewWindowX + renderer.ViewWidth;
                    Array.Copy(DoomGame.Instance.Video.Screens[1], rightOfs,
                        DoomGame.Instance.Video.Screens[0], rightOfs, renderer.ViewWindowX);
                }
            }
        }
    }

    /// <summary>
    /// Erases a text line with message on/off tracking (for stext).
    /// This is the equivalent of HUlib_eraseSText.
    /// </summary>
    private void EraseTextLine(int y, int needsUpdate, ref bool on)
    {
        // If message was on last frame but is now off, force an update
        // to erase the old text
        if (_lastMessageOn && !_messageOn)
        {
            _messageNeedsUpdate = 4;
        }
        EraseTextLine(y, _messageNeedsUpdate);
        _lastMessageOn = _messageOn;
    }

    private bool _lastMessageOn = true;

    /// <summary>
    /// Draws a single line of text using the HU font.
    /// This is the equivalent of HUlib_drawTextLine.
    /// </summary>
    private void DrawTextLine(string text, int x, int y)
    {
        var cx = x;
        foreach (var ch in text)
        {
            var c = char.ToUpper(ch);
            if (c != ' ' && c >= HuFontStart && c <= HuFontEnd)
            {
                var fontIdx = c - HuFontStart;
                var w = Font[fontIdx].Width;
                if (cx + w > Constants.ScreenWidth)
                {
                    break;
                }
                DoomGame.Instance.Video.DrawPatchDirect(cx, y, 0, Font[fontIdx]);
                cx += w;
            }
            else
            {
                cx += 4;
                if (cx >= Constants.ScreenWidth)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Draws the current message if it's on.
    /// This is the equivalent of HUlib_drawSText.
    /// </summary>
    private void DrawMessage()
    {
        if (!_messageOn)
        {
            return;
        }

        DrawTextLine(_messageText, MessageX, MessageY);
    }

    public void Ticker()
    {
        var game = DoomGame.Instance.Game;

        // tick down message counter if message is up
        if (_messageCounter != 0 && --_messageCounter == 0)
        {
            _messageOn = false;
            _messageNotToBeFuckedWith = false;
        }

        if (DoomGame.Instance.Menu.ShowMessages || MessageDontFuckWithMe)
        {
            // display message if necessary
            if ((_player!.Message != null && !_messageNotToBeFuckedWith)
                || (_player.Message != null && MessageDontFuckWithMe))
            {
                _messageText = _player.Message;
                _messageNeedsUpdate = 4;
                _player.Message = null;
                _messageOn = true;
                _messageCounter = MessageTimeout;
                _messageNotToBeFuckedWith = MessageDontFuckWithMe;
                MessageDontFuckWithMe = false;
            }

        } // else message_on = false;

        // check for incoming chat characters
        //if (game.NetGame)
        //{
        //    for (var i = 0; i < Constants.MaxPlayers; i++)
        //    {
        //        if (!game.PlayerInGame[i])
        //        {
        //            continue;
        //        }

        //        if (i != game.ConsolePlayer && (c = game.Players[i].Command.ChatChar) != 0)
        //        {
        //            if (c <= Broadcast)
        //            {
        //                chat_dest[i] = c;
        //            }
        //            else
        //            {
        //                if (c >= 'a' && c <= 'z')
        //                {
        //                    c = (char)shiftxform[(unsigned char) c];
        //                }
        //                rc = HUlib_keyInIText(&w_inputbuffer[i], c);
        //                if (rc != 0 && c == (int)Keys.Enter)
        //                {
        //                    if (w_inputbuffer[i].l.len
        //                        && (chat_dest[i] == consoleplayer + 1
        //                            || chat_dest[i] == HU_BROADCAST))
        //                    {
        //                        HUlib_addMessageToSText(&w_message,
        //                            player_names[i],
        //                            w_inputbuffer[i].l.l);

        //                        message_nottobefuckedwith = true;
        //                        message_on = true;
        //                        message_counter = HU_MSGTIMEOUT;
        //                        if (gamemode == commercial)
        //                            S_StartSound(0, sfx_radio);
        //                        else
        //                            S_StartSound(0, sfx_tink);
        //                    }
        //                    HUlib_resetIText(&w_inputbuffer[i]);
        //                }
        //            }
        //            game.Players[i].Command.ChatChar = (char)0;
        //        }
        //    }
        //}
    }

    public bool HandleEvent(InputEvent currentEvent)
    {
        // Pressing Enter refreshes the last message
        if (currentEvent.Type == EventType.KeyDown && currentEvent.Data1 == MessageRefresh)
        {
            _messageOn = true;
            _messageCounter = MessageTimeout;
            return true;
        }

        return false;
    }
}
