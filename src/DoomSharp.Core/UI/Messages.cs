namespace DoomSharp.Core.UI;

public static class Messages
{
    public const int NumberOfQuitMessages = 22;

    public static bool IsEnglish => DoomGame.Instance.Language == GameLanguage.English;

    //
    // D_Main.C
    //
    public static string D_DEVSTR => "Development mode ON.\n";
    public static string D_CDROM => "CD-ROM Version: default.cfg from c:\\doomdata\n";

    //
    // M_Menu.C
    //
    public static string PressAnyKey =>
        IsEnglish
            ? "press a key."
            : "APPUYEZ SUR UNE TOUCHE.";

    public static string PressYesOrNo =>
        IsEnglish
            ? "press y or n."
            : "APPUYEZ SUR Y OU N.";

    public static string QuickSaveSpot =>
        IsEnglish
            ? $"you haven't picked a quicksave slot yet!\n\n" + PressAnyKey
            : $"VOUS N'AVEZ PAS CHOISI UN EMPLACEMENT!\n\n" + PressAnyKey;

    public static string QuickSavePrompt =>
        IsEnglish
            ? $"quicksave over your game named\n\n'{{0}}'?\n" + PressYesOrNo
            : $"SAUVEGARDE RAPIDE DANS LE FICHIER \n\n'{{0}}'?\n" + PressYesOrNo;

    public static string QuickLoadPrompt =>
        IsEnglish
            ? $"do you want to quickload the game named\n\n'{{0}}'?\n" + PressYesOrNo
            : $"VOULEZ-VOUS CHARGER LA SAUVEGARDE\n\n'{{0}}'?\n" + PressYesOrNo;

    public static string NewGame =>
        IsEnglish
            ? $"you can't start a new game\nwhile in a network game\n\n" + PressAnyKey
            : $"VOUS NE POUVEZ PAS LANCER\nUN NOUVEAU JEU SUR RESEAU.\n\n" + PressAnyKey;

    public static string Nightmare =>
        IsEnglish
            ? $"are you sure? this skill level\nisn't even remotely fair.\n\n" + PressYesOrNo
            : $"VOUS CONFIRMEZ? CE NIVEAU EST\nVRAIMENT IMPITOYABLE!\n" + PressYesOrNo;

    public static string Shareware =>
        IsEnglish
            ? $"this is the shareware version of doom.\n\nyou need to order the entire trilogy.\n\n" + PressAnyKey
            : $"CECI EST UNE VERSION SHAREWARE DE DOOM.\n\nVOUS DEVRIEZ COMMANDER LA TRILOGIE COMPLETE.\n\n" + PressAnyKey;

    public static string GenericQuitMessage =>
        IsEnglish
            ? "are you sure you want to\nquit this great game?"
            : "VOUS VOULEZ VRAIMENT\nQUITTER CE SUPER JEU?";

    public static string LoadNet =>
        IsEnglish
            ? "you can't do load while in a net game!\n\n" + PressAnyKey
            : "";

    public static string QLoadNet =>
        IsEnglish
            ? "you can't quickload during a netgame!\n\n" + PressAnyKey
            : "";

    public static string SaveDead =>
        IsEnglish
            ? "you can't save if you aren't playing!\n\n" + PressAnyKey
            : "";

    public static string NetEnd =>
        IsEnglish
            ? "you can't end a netgame!\n\n" + PressAnyKey
            : "";

    public static string EndGame =>
        IsEnglish
            ? "are you sure you want to end the game?\n\n" + PressYesOrNo
            : "";

    public static string DosY =>
        IsEnglish
            ? "(press y to quit)"
            : "(APPUYEZ SUR Y POUR REVENIR AU OS.)";

    public static string DetailHigh =>
        IsEnglish
            ? "High detail"
            : "GRAPHISMES MAXIMUM";

    public static string DetailLow =>
        IsEnglish
            ? "Low detail"
            : "GRAPHISMES MINIMUM";

    public static string MsgOff => "Messages OFF";
    public static string MsgOn => "Messages ON";

    public static string GammaLvl0 => "Gamma correction OFF";
    public static string GammaLvl1 => "Gamma correction level 1";
    public static string GammaLvl2 => "Gamma correction level 2";
    public static string GammaLvl3 => "Gamma correction level 3";
    public static string GammaLvl4 => "Gamma correction level 4";

    public static readonly string[] GammaMessages =
    {
        GammaLvl0, GammaLvl1, GammaLvl2, GammaLvl3, GammaLvl4
    };

    public static string EmptyString => "empty slot";

    //
    // P_inter.C
    //
    public static string GotArmor => IsEnglish ? "Picked up the armor." : "";
    public static string GotMega => IsEnglish ? "Picked up the MegaArmor!" : "";
    public static string GotHealthBonus => IsEnglish ? "Picked up a health bonus." : "";
    public static string GotArmorBonus => IsEnglish ? "Picked up an armor bonus." : "";
    public static string GotStim => IsEnglish ? "Picked up a stimpack." : "";
    public static string GotMedikitAndItWasNeeded => IsEnglish ? "Picked up a medikit that you REALLY need!" : "";
    public static string GotMedikit => IsEnglish ? "Picked up a medikit." : "";
    public static string GotSuper => IsEnglish ? "Supercharge!" : "";

    public static string GotBlueCard => IsEnglish ? "Picked up a blue keycard." : "";
    public static string GotYellowCard => IsEnglish ? "Picked up a yellow keycard." : "";
    public static string GotRedCard => IsEnglish ? "Picked up a red keycard." : "";
    public static string GotBlueSkull => IsEnglish ? "Picked up a blue skull key." : "";
    public static string GotYellowSkull => IsEnglish ? "Picked up a yellow skull key." : "";
    public static string GotRedSkull => IsEnglish ? "Picked up a red skull key." : "";

    public static string GotInvulnerability => IsEnglish ? "Invulnerability!" : "";
    public static string GotBerserk => IsEnglish ? "Berserk!" : "";
    public static string GotInvisibility => IsEnglish ? "Partial Invisibility" : "";
    public static string GotSuit => IsEnglish ? "Radiation Shielding Suit" : "";
    public static string GotMap => IsEnglish ? "Computer Area Map" : "";
    public static string GotVisor => IsEnglish ? "Light Amplification Visor" : "";
    public static string GotMegaSphere => IsEnglish ? "MegaSphere!" : "";

    public static string GotClip => IsEnglish ? "Picked up a clip." : "";
    public static string GotClipBox => IsEnglish ? "Picked up a box of bullets." : "";
    public static string GotRocket => IsEnglish ? "Picked up a rocket." : "";
    public static string GotRocketBox => IsEnglish ? "Picked up a box of rockets." : "";
    public static string GotCell => IsEnglish ? "Picked up an energy cell." : "";
    public static string GotCellBox => IsEnglish ? "Picked up an energy cell pack." : "";
    public static string GotShells => IsEnglish ? "Picked up 4 shotgun shells." : "";
    public static string GotShellBox => IsEnglish ? "Picked up a box of shotgun shells." : "";
    public static string GotBackpack => IsEnglish ? "Picked up a backpack full of ammo!" : "";

    public static string GotBfg9000 => IsEnglish ? "You got the BFG9000!  Oh, yes." : "";
    public static string GotChaingun => IsEnglish ? "You got the chaingun!" : "";
    public static string GotChainsaw => IsEnglish ? "A chainsaw!  Find some meat!" : "";
    public static string GotRocketLauncher => IsEnglish ? "You got the rocket launcher!" : "";
    public static string GotPlasmaRifle => IsEnglish ? "You got the plasma gun!" : "";
    public static string GotShotgun => IsEnglish ? "You got the shotgun!" : "";
    public static string GotShotgun2 => IsEnglish ? "You got the super shotgun!" : "";

    //
    // P_Doors.C
    //
    public static string PD_BLUEO => IsEnglish ? "You need a blue key to activate this object" : "";
    public static string PD_REDO => IsEnglish ? "You need a red key to activate this object" : "";
    public static string PD_YELLOWO => IsEnglish ? "You need a yellow key to activate this object" : "";

    public static string PD_BLUEK => IsEnglish ? "You need a blue key to open this door" : "";
    public static string PD_REDK => IsEnglish ? "You need a red key to open this door" : "";
    public static string PD_YELLOWK => IsEnglish ? "You need a yellow key to open this door" : "";

    //
    // G_game.C
    //
    public static string GGSAVED => "game saved.";

    //
    // HU_stuff.C
    //
    public static string HUSTR_MSGU => "[Message unsent]";

    public static string HUSTR_E1M1 => IsEnglish ? "E1M1: Hangar" : "E1M1: HANGAR";
    public static string HUSTR_E1M2 => IsEnglish ? "E1M2: Nuclear Plant" : "E1M2: USINE NUCLEAIRE ";
    public static string HUSTR_E1M3 => IsEnglish ? "E1M3: Toxin Refinery" : "E1M3: RAFFINERIE DE TOXINES ";
    public static string HUSTR_E1M4 => IsEnglish ? "E1M4: Command Control" : "E1M4: CENTRE DE CONTROLE ";
    public static string HUSTR_E1M5 => IsEnglish ? "E1M5: Phobos Lab" : "E1M5: LABORATOIRE PHOBOS ";
    public static string HUSTR_E1M6 => IsEnglish ? "E1M6: Central Processing" : "E1M6: TRAITEMENT CENTRAL ";
    public static string HUSTR_E1M7 => IsEnglish ? "E1M7: Computer Station" : "E1M7: CENTRE INFORMATIQUE ";
    public static string HUSTR_E1M8 => IsEnglish ? "E1M8: Phobos Anomaly" : "E1M8: ANOMALIE PHOBOS ";
    public static string HUSTR_E1M9 => IsEnglish ? "E1M9: Military Base" : "E1M9: BASE MILITAIRE ";

    public static string HUSTR_E2M1 => IsEnglish ? "E2M1: Deimos Anomaly" : "E2M1: ANOMALIE DEIMOS ";
    public static string HUSTR_E2M2 => IsEnglish ? "E2M2: Containment Area" : "E2M2: ZONE DE CONFINEMENT ";
    public static string HUSTR_E2M3 => IsEnglish ? "E2M3: Refinery" : "E2M3: RAFFINERIE";
    public static string HUSTR_E2M4 => IsEnglish ? "E2M4: Deimos Lab" : "E2M4: LABORATOIRE DEIMOS ";
    public static string HUSTR_E2M5 => IsEnglish ? "E2M5: Command Center" : "E2M5: CENTRE DE CONTROLE ";
    public static string HUSTR_E2M6 => IsEnglish ? "E2M6: Halls of the Damned" : "E2M6: HALLS DES DAMNES ";
    public static string HUSTR_E2M7 => IsEnglish ? "E2M7: Spawning Vats" : "E2M7: CUVES DE REPRODUCTION ";
    public static string HUSTR_E2M8 => IsEnglish ? "E2M8: Tower of Babel" : "E2M8: TOUR DE BABEL ";
    public static string HUSTR_E2M9 => IsEnglish ? "E2M9: Fortress of Mystery" : "E2M9: FORTERESSE DU MYSTERE ";

    public static string HUSTR_E3M1 => IsEnglish ? "E3M1: Hell Keep" : "E3M1: DONJON DE L'ENFER ";
    public static string HUSTR_E3M2 => IsEnglish ? "E3M2: Slough of Despair" : "E3M2: BOURBIER DU DESESPOIR ";
    public static string HUSTR_E3M3 => IsEnglish ? "E3M3: Pandemonium" : "E3M3: PANDEMONIUM";
    public static string HUSTR_E3M4 => IsEnglish ? "E3M4: House of Pain" : "E3M4: MAISON DE LA DOULEUR ";
    public static string HUSTR_E3M5 => IsEnglish ? "E3M5: Unholy Cathedral" : "E3M5: CATHEDRALE PROFANE ";
    public static string HUSTR_E3M6 => IsEnglish ? "E3M6: Mt. Erebus" : "E3M6: MONT EREBUS";
    public static string HUSTR_E3M7 => IsEnglish ? "E3M7: Limbo" : "E3M7: LIMBES";
    public static string HUSTR_E3M8 => IsEnglish ? "E3M8: Dis" : "E3M8: DIS";
    public static string HUSTR_E3M9 => IsEnglish ? "E3M9: Warrens" : "E3M9: CLAPIERS";

    public static string HUSTR_E4M1 => "E4M1: Hell Beneath";
    public static string HUSTR_E4M2 => "E4M2: Perfect Hatred";
    public static string HUSTR_E4M3 => "E4M3: Sever The Wicked";
    public static string HUSTR_E4M4 => "E4M4: Unruly Evil";
    public static string HUSTR_E4M5 => "E4M5: They Will Repent";
    public static string HUSTR_E4M6 => "E4M6: Against Thee Wickedly";
    public static string HUSTR_E4M7 => "E4M7: And Hell Followed";
    public static string HUSTR_E4M8 => "E4M8: Unto The Cruel";
    public static string HUSTR_E4M9 => "E4M9: Fear";

    public static string HUSTR_1 => IsEnglish ? "level 1: entryway" : "NIVEAU 1: ENTREE ";
    public static string HUSTR_2 => IsEnglish ? "level 2: underhalls" : "NIVEAU 2: HALLS SOUTERRAINS ";
    public static string HUSTR_3 => IsEnglish ? "level 3: the gantlet" : "NIVEAU 3: LE FEU NOURRI ";
    public static string HUSTR_4 => IsEnglish ? "level 4: the focus" : "NIVEAU 4: LE FOYER ";
    public static string HUSTR_5 => IsEnglish ? "level 5: the waste tunnels" : "NIVEAU 5: LES EGOUTS ";
    public static string HUSTR_6 => IsEnglish ? "level 6: the crusher" : "NIVEAU 6: LE BROYEUR ";
    public static string HUSTR_7 => IsEnglish ? "level 7: dead simple" : "NIVEAU 7: L'HERBE DE LA MORT";
    public static string HUSTR_8 => IsEnglish ? "level 8: tricks and traps" : "NIVEAU 8: RUSES ET PIEGES ";
    public static string HUSTR_9 => IsEnglish ? "level 9: the pit" : "NIVEAU 9: LE PUITS ";
    public static string HUSTR_10 => IsEnglish ? "level 10: refueling base" : "NIVEAU 10: BASE DE RAVITAILLEMENT ";
    public static string HUSTR_11 => IsEnglish ? "level 11: 'o' of destruction!" : "NIVEAU 11: LE CERCLE DE LA MORT!";

    public static string HUSTR_12 => IsEnglish ? "level 12: the factory" : "NIVEAU 12: L'USINE ";
    public static string HUSTR_13 => IsEnglish ? "level 13: downtown" : "NIVEAU 13: LE CENTRE VILLE";
    public static string HUSTR_14 => IsEnglish ? "level 14: the inmost dens" : "NIVEAU 14: LES ANTRES PROFONDES ";
    public static string HUSTR_15 => IsEnglish ? "level 15: industrial zone" : "NIVEAU 15: LA ZONE INDUSTRIELLE ";
    public static string HUSTR_16 => IsEnglish ? "level 16: suburbs" : "NIVEAU 16: LA BANLIEUE";
    public static string HUSTR_17 => IsEnglish ? "level 17: tenements" : "NIVEAU 17: LES IMMEUBLES";
    public static string HUSTR_18 => IsEnglish ? "level 18: the courtyard" : "NIVEAU 18: LA COUR ";
    public static string HUSTR_19 => IsEnglish ? "level 19: the citadel" : "NIVEAU 19: LA CITADELLE ";
    public static string HUSTR_20 => IsEnglish ? "level 20: gotcha!" : "NIVEAU 20: JE T'AI EU!";

    public static string HUSTR_21 => IsEnglish ? "level 21: nirvana" : "NIVEAU 21: LE NIRVANA";
    public static string HUSTR_22 => IsEnglish ? "level 22: the catacombs" : "NIVEAU 22: LES CATACOMBES ";
    public static string HUSTR_23 => IsEnglish ? "level 23: barrels o' fun" : "NIVEAU 23: LA GRANDE FETE ";
    public static string HUSTR_24 => IsEnglish ? "level 24: the chasm" : "NIVEAU 24: LE GOUFFRE ";
    public static string HUSTR_25 => IsEnglish ? "level 25: bloodfalls" : "NIVEAU 25: LES CHUTES DE SANG";
    public static string HUSTR_26 => IsEnglish ? "level 26: the abandoned mines" : "NIVEAU 26: LES MINES ABANDONNEES ";
    public static string HUSTR_27 => IsEnglish ? "level 27: monster condo" : "NIVEAU 27: CHEZ LES MONSTRES ";
    public static string HUSTR_28 => IsEnglish ? "level 28: the spirit world" : "NIVEAU 28: LE MONDE DE L'ESPRIT ";
    public static string HUSTR_29 => IsEnglish ? "level 29: the living end" : "NIVEAU 29: LA LIMITE ";
    public static string HUSTR_30 => IsEnglish ? "level 30: icon of sin" : "NIVEAU 30: L'ICONE DU PECHE ";

    public static string HUSTR_31 => IsEnglish ? "level 31: wolfenstein" : "NIVEAU 31: WOLFENSTEIN";
    public static string HUSTR_32 => IsEnglish ? "level 32: grosse" : "NIVEAU 32: LE MASSACRE";

    // Plutonia WAD map names
    public static string PHUSTR_1 => "level 1: congo";
    public static string PHUSTR_2 => "level 2: well of souls";
    public static string PHUSTR_3 => "level 3: aztec";
    public static string PHUSTR_4 => "level 4: caged";
    public static string PHUSTR_5 => "level 5: ghost town";
    public static string PHUSTR_6 => "level 6: baron's lair";
    public static string PHUSTR_7 => "level 7: caughtyard";
    public static string PHUSTR_8 => "level 8: realm";
    public static string PHUSTR_9 => "level 9: abattoire";
    public static string PHUSTR_10 => "level 10: onslaught";
    public static string PHUSTR_11 => "level 11: hunted";

    public static string PHUSTR_12 => "level 12: speed";
    public static string PHUSTR_13 => "level 13: the crypt";
    public static string PHUSTR_14 => "level 14: genesis";
    public static string PHUSTR_15 => "level 15: the twilight";
    public static string PHUSTR_16 => "level 16: the omen";
    public static string PHUSTR_17 => "level 17: compound";
    public static string PHUSTR_18 => "level 18: neurosphere";
    public static string PHUSTR_19 => "level 19: nme";
    public static string PHUSTR_20 => "level 20: the death domain";

    public static string PHUSTR_21 => "level 21: slayer";
    public static string PHUSTR_22 => "level 22: impossible mission";
    public static string PHUSTR_23 => "level 23: tombstone";
    public static string PHUSTR_24 => "level 24: the final frontier";
    public static string PHUSTR_25 => "level 25: the temple of darkness";
    public static string PHUSTR_26 => "level 26: bunker";
    public static string PHUSTR_27 => "level 27: anti-christ";
    public static string PHUSTR_28 => "level 28: the sewers";
    public static string PHUSTR_29 => "level 29: odyssey of noises";
    public static string PHUSTR_30 => "level 30: the gateway of hell";

    public static string PHUSTR_31 => "level 31: cyberden";
    public static string PHUSTR_32 => "level 32: go 2 it";

    // TNT WAD map names
    public static string THUSTR_1 => "level 1: system control";
    public static string THUSTR_2 => "level 2: human bbq";
    public static string THUSTR_3 => "level 3: power control";
    public static string THUSTR_4 => "level 4: wormhole";
    public static string THUSTR_5 => "level 5: hanger";
    public static string THUSTR_6 => "level 6: open season";
    public static string THUSTR_7 => "level 7: prison";
    public static string THUSTR_8 => "level 8: metal";
    public static string THUSTR_9 => "level 9: stronghold";
    public static string THUSTR_10 => "level 10: redemption";
    public static string THUSTR_11 => "level 11: storage facility";

    public static string THUSTR_12 => "level 12: crater";
    public static string THUSTR_13 => "level 13: nukage processing";
    public static string THUSTR_14 => "level 14: steel works";
    public static string THUSTR_15 => "level 15: dead zone";
    public static string THUSTR_16 => "level 16: deepest reaches";
    public static string THUSTR_17 => "level 17: processing area";
    public static string THUSTR_18 => "level 18: mill";
    public static string THUSTR_19 => "level 19: shipping/respawning";
    public static string THUSTR_20 => "level 20: central processing";

    public static string THUSTR_21 => "level 21: administration center";
    public static string THUSTR_22 => "level 22: habitat";
    public static string THUSTR_23 => "level 23: lunar mining project";
    public static string THUSTR_24 => "level 24: quarry";
    public static string THUSTR_25 => "level 25: baron's den";
    public static string THUSTR_26 => "level 26: ballistyx";
    public static string THUSTR_27 => "level 27: mount pain";
    public static string THUSTR_28 => "level 28: heck";
    public static string THUSTR_29 => "level 29: river styx";
    public static string THUSTR_30 => "level 30: last call";

    public static string THUSTR_31 => "level 31: pharaoh";
    public static string THUSTR_32 => "level 32: caribbean";

    // Chat macros
    public static string HUSTR_CHATMACRO0 => "No";
    public static string HUSTR_CHATMACRO1 => "I'm ready to kick butt!";
    public static string HUSTR_CHATMACRO2 => "I'm OK.";
    public static string HUSTR_CHATMACRO3 => "I'm not looking too good!";
    public static string HUSTR_CHATMACRO4 => "Help!";
    public static string HUSTR_CHATMACRO5 => "You suck!";
    public static string HUSTR_CHATMACRO6 => "Next time, scumbag...";
    public static string HUSTR_CHATMACRO7 => "Come here!";
    public static string HUSTR_CHATMACRO8 => "I'll take care of it.";
    public static string HUSTR_CHATMACRO9 => "Yes";

    public static readonly string[] ChatMacros =
    {
        HUSTR_CHATMACRO0,
        HUSTR_CHATMACRO1,
        HUSTR_CHATMACRO2,
        HUSTR_CHATMACRO3,
        HUSTR_CHATMACRO4,
        HUSTR_CHATMACRO5,
        HUSTR_CHATMACRO6,
        HUSTR_CHATMACRO7,
        HUSTR_CHATMACRO8,
        HUSTR_CHATMACRO9
    };

    // Talk to self messages
    public static string HUSTR_TALKTOSELF1 => "You mumble to yourself";
    public static string HUSTR_TALKTOSELF2 => "Who's there?";
    public static string HUSTR_TALKTOSELF3 => "You scare yourself";
    public static string HUSTR_TALKTOSELF4 => "You start to rave";
    public static string HUSTR_TALKTOSELF5 => "You've lost it...";

    public static string HUSTR_MESSAGESENT => "[Message Sent]";

    // Player names for chat
    public static string HUSTR_PLRGREEN => "Green: ";
    public static string HUSTR_PLRINDIGO => "Indigo: ";
    public static string HUSTR_PLRBROWN => "Brown: ";
    public static string HUSTR_PLRRED => "Red: ";

    public static readonly string[] PlayerNames =
    {
        HUSTR_PLRGREEN,
        HUSTR_PLRINDIGO,
        HUSTR_PLRBROWN,
        HUSTR_PLRRED
    };

    // Player chat key bindings
    public const char HUSTR_KEYGREEN = 'g';
    public const char HUSTR_KEYINDIGO = 'i';
    public const char HUSTR_KEYBROWN = 'b';
    public const char HUSTR_KEYRED = 'r';

    //
    // AM_map.C
    //
    public static string AMSTR_FOLLOWON => "Follow Mode ON";
    public static string AMSTR_FOLLOWOFF => "Follow Mode OFF";
    public static string AMSTR_GRIDON => "Grid ON";
    public static string AMSTR_GRIDOFF => "Grid OFF";
    public static string AMSTR_MARKEDSPOT => "Marked Spot";
    public static string AMSTR_MARKSCLEARED => "All Marks Cleared";

    //
    // ST_stuff.C
    //
    public static string STSTR_MUS => "Music Change";
    public static string STSTR_NOMUS => "IMPOSSIBLE SELECTION";
    public static string STSTR_DQDON => "Degreelessness Mode On";
    public static string STSTR_DQDOFF => "Degreelessness Mode Off";
    public static string STSTR_KFAADDED => "Very Happy Ammo Added";
    public static string STSTR_FAADDED => "Ammo (no keys) Added";
    public static string STSTR_NCON => "No Clipping Mode ON";
    public static string STSTR_NCOFF => "No Clipping Mode OFF";
    public static string STSTR_BEHOLD => "inVuln, Str, Inviso, Rad, Allmap, or Lite-amp";
    public static string STSTR_BEHOLDX => "Power-up Toggled";
    public static string STSTR_CHOPPERS => "... doesn't suck - GM";
    public static string STSTR_CLEV => "Changing Level...";

    //
    // Quit messages
    //
    public static IEnumerable<string> QuitMessages
    {
        get
        {
            // DOOM1
            yield return GenericQuitMessage;
            yield return "please don't leave, there's more\ndemons to toast!";
            yield return "let's beat it -- this is turning\ninto a bloodbath!";
            yield return "i wouldn't leave if i were you.\ndos is much worse.";
            yield return "you're trying to say you like dos\nbetter than me, right?";
            yield return "don't leave yet -- there's a\ndemon around that corner!";
            yield return "ya know, next time you come in here\ni'm gonna toast ya.";
            yield return "go ahead and leave. see if i care.";

            // QuitDOOM II messages
            yield return "you want to quit?\nthen, thou hast lost an eighth!";
            yield return "don't go now, there's a \ndimensional shambler waiting\nat the dos prompt!";
            yield return "get outta here and go back\nto your boring programs.";
            yield return "if i were your boss, i'd \n deathmatch ya in a minute!";
            yield return "look, bud. you leave now\nand you forfeit your body count!";
            yield return "just leave. when you come\nback, i'll be waiting with a bat.";
            yield return "you're lucky i don't smack\nyou for thinking about leaving.";

            // FinalDOOM?
            yield return "fuck you, pussy!\nget the fuck out!";
            yield return "you quit and i'll jizz\nin your cystholes!";
            yield return "if you leave, i'll make\nthe lord drink my jizz.";
            yield return "hey, ron! can we say\n'fuck' in the game?";
            yield return "i'd leave: this is just\nmore monsters and levels.\nwhat a load.";
            yield return "suck it down, asshole!\nyou're a fucking wimp!";
            yield return "don't quit now! we're \nstill spending your money!";

            // Internal debug. Different style, too.
            yield return "THIS IS NO MESSAGE!\nPage intentionally left blank.";
        }
    }

    //
    // F_Finale.C - Episode ending texts
    //
    public static string E1TEXT =>
        "Once you beat the big badasses and\n"
        + "clean out the moon base you're supposed\n"
        + "to win, aren't you? Aren't you? Where's\n"
        + "your fat reward and ticket home? What\n"
        + "the hell is this? It's not supposed to\n"
        + "end this way!\n"
        + "\n"
        + "It stinks like rotten meat, but looks\n"
        + "like the lost Deimos base.  Looks like\n"
        + "you're stuck on The Shores of Hell.\n"
        + "The only way out is through.\n"
        + "\n"
        + "To continue the DOOM experience, play\n"
        + "The Shores of Hell and its amazing\n"
        + "sequel, Inferno!\n";

    public static string E2TEXT =>
        "You've done it! The hideous cyber-\n"
        + "demon lord that ruled the lost Deimos\n"
        + "moon base has been slain and you\n"
        + "are triumphant! But ... where are\n"
        + "you? You clamber to the edge of the\n"
        + "moon and look down to see the awful\n"
        + "truth.\n"
        + "\n"
        + "Deimos floats above Hell itself!\n"
        + "You've never heard of anyone escaping\n"
        + "from Hell, but you'll make the bastards\n"
        + "sorry they ever heard of you! Quickly,\n"
        + "you rappel down to  the surface of\n"
        + "Hell.\n"
        + "\n"
        + "Now, it's on to the final chapter of\n"
        + "DOOM! -- Inferno.";

    public static string E3TEXT =>
        "The loathsome spiderdemon that\n"
        + "masterminded the invasion of the moon\n"
        + "bases and caused so much death has had\n"
        + "its ass kicked for all time.\n"
        + "\n"
        + "A hidden doorway opens and you enter.\n"
        + "You've proven too tough for Hell to\n"
        + "contain, and now Hell at last plays\n"
        + "fair -- for you emerge from the door\n"
        + "to see the green fields of Earth!\n"
        + "Home at last.\n"
        + "\n"
        + "You wonder what's been happening on\n"
        + "Earth while you were battling evil\n"
        + "unleashed. It's good that no Hell-\n"
        + "spawn could have come through that\n"
        + "door with you ...";

    public static string E4TEXT =>
        "the spider mastermind must have sent forth\n"
        + "its legions of hellspawn before your\n"
        + "final confrontation with that terrible\n"
        + "beast from hell.  but you stepped forward\n"
        + "and brought forth eternal damnation and\n"
        + "suffering upon the horde as a true hero\n"
        + "would in the face of something so evil.\n"
        + "\n"
        + "besides, someone was gonna pay for what\n"
        + "happened to daisy, your pet rabbit.\n"
        + "\n"
        + "but now, you see spread before you more\n"
        + "potential pain and gibbitude as a nation\n"
        + "of demons run amok among our cities.\n"
        + "\n"
        + "next stop, hell on earth!";

    // DOOM II intermission texts
    public static string C1TEXT =>
        "YOU HAVE ENTERED DEEPLY INTO THE INFESTED\n"
        + "STARPORT. BUT SOMETHING IS WRONG. THE\n"
        + "MONSTERS HAVE BROUGHT THEIR OWN REALITY\n"
        + "WITH THEM, AND THE STARPORT'S TECHNOLOGY\n"
        + "IS BEING SUBVERTED BY THEIR PRESENCE.\n"
        + "\n"
        + "AHEAD, YOU SEE AN OUTPOST OF HELL, A\n"
        + "FORTIFIED ZONE. IF YOU CAN GET PAST IT,\n"
        + "YOU CAN PENETRATE INTO THE HAUNTED HEART\n"
        + "OF THE STARBASE AND FIND THE CONTROLLING\n"
        + "SWITCH WHICH HOLDS EARTH'S POPULATION\n"
        + "HOSTAGE.";

    public static string C2TEXT =>
        "YOU HAVE WON! YOUR VICTORY HAS ENABLED\n"
        + "HUMANKIND TO EVACUATE EARTH AND ESCAPE\n"
        + "THE NIGHTMARE.  NOW YOU ARE THE ONLY\n"
        + "HUMAN LEFT ON THE FACE OF THE PLANET.\n"
        + "CANNIBAL MUTATIONS, CARNIVOROUS ALIENS,\n"
        + "AND EVIL SPIRITS ARE YOUR ONLY NEIGHBORS.\n"
        + "YOU SIT BACK AND WAIT FOR DEATH, CONTENT\n"
        + "THAT YOU HAVE SAVED YOUR SPECIES.\n"
        + "\n"
        + "BUT THEN, EARTH CONTROL BEAMS DOWN A\n"
        + "MESSAGE FROM SPACE: \"SENSORS HAVE LOCATED\n"
        + "THE SOURCE OF THE ALIEN INVASION. IF YOU\n"
        + "GO THERE, YOU MAY BE ABLE TO BLOCK THEIR\n"
        + "ENTRY.  THE ALIEN BASE IS IN THE HEART OF\n"
        + "YOUR OWN HOME CITY, NOT FAR FROM THE\n"
        + "STARPORT.\" SLOWLY AND PAINFULLY YOU GET\n"
        + "UP AND RETURN TO THE FRAY.";

    public static string C3TEXT =>
        "YOU ARE AT THE CORRUPT HEART OF THE CITY,\n"
        + "SURROUNDED BY THE CORPSES OF YOUR ENEMIES.\n"
        + "YOU SEE NO WAY TO DESTROY THE CREATURES'\n"
        + "ENTRYWAY ON THIS SIDE, SO YOU CLENCH YOUR\n"
        + "TEETH AND PLUNGE THROUGH IT.\n"
        + "\n"
        + "THERE MUST BE A WAY TO CLOSE IT ON THE\n"
        + "OTHER SIDE. WHAT DO YOU CARE IF YOU'VE\n"
        + "GOT TO GO THROUGH HELL TO GET TO IT?";

    public static string C4TEXT =>
        "THE HORRENDOUS VISAGE OF THE BIGGEST\n"
        + "DEMON YOU'VE EVER SEEN CRUMBLES BEFORE\n"
        + "YOU, AFTER YOU PUMP YOUR ROCKETS INTO\n"
        + "HIS EXPOSED BRAIN. THE MONSTER SHRIVELS\n"
        + "UP AND DIES, ITS THRASHING LIMBS\n"
        + "DEVASTATING UNTOLD MILES OF HELL'S\n"
        + "SURFACE.\n"
        + "\n"
        + "YOU'VE DONE IT. THE INVASION IS OVER.\n"
        + "EARTH IS SAVED. HELL IS A WRECK. YOU\n"
        + "WONDER WHERE BAD FOLKS WILL GO WHEN THEY\n"
        + "DIE, NOW. WIPING THE SWEAT FROM YOUR\n"
        + "FOREHEAD YOU BEGIN THE LONG TREK BACK\n"
        + "HOME. REBUILDING EARTH OUGHT TO BE A\n"
        + "LOT MORE FUN THAN RUINING IT WAS.\n";

    public static string C5TEXT =>
        "CONGRATULATIONS, YOU'VE FOUND THE SECRET\n"
        + "LEVEL! LOOKS LIKE IT'S BEEN BUILT BY\n"
        + "HUMANS, RATHER THAN DEMONS. YOU WONDER\n"
        + "WHO THE INMATES OF THIS CORNER OF HELL\n"
        + "WILL BE.";

    public static string C6TEXT =>
        "CONGRATULATIONS, YOU'VE FOUND THE\n"
        + "SUPER SECRET LEVEL!  YOU'D BETTER\n"
        + "BLAZE THROUGH THIS ONE!\n";

    // Plutonia texts
    public static string P1TEXT =>
        "You gloat over the steaming carcass of the\n"
        + "Guardian.  With its death, you've wrested\n"
        + "the Accelerator from the stinking claws\n"
        + "of Hell.  You relax and glance around the\n"
        + "room.  Damn!  There was supposed to be at\n"
        + "least one working prototype, but you can't\n"
        + "see it. The demons must have taken it.\n"
        + "\n"
        + "You must find the prototype, or all your\n"
        + "struggles will have been wasted. Keep\n"
        + "moving, keep fighting, keep killing.\n"
        + "Oh yes, keep living, too.";

    public static string P2TEXT =>
        "Even the deadly Arch-Vile labyrinth could\n"
        + "not stop you, and you've gotten to the\n"
        + "prototype Accelerator which is soon\n"
        + "efficiently and permanently deactivated.\n"
        + "\n"
        + "You're good at that kind of thing.";

    public static string P3TEXT =>
        "You've bashed and battered your way into\n"
        + "the heart of the devil-hive.  Time for a\n"
        + "Search-and-Destroy mission, aimed at the\n"
        + "Gatekeeper, whose foul offspring is\n"
        + "cascading to Earth.  Yeah, he's bad. But\n"
        + "you know who's worse!\n"
        + "\n"
        + "Grinning evilly, you check your gear, and\n"
        + "get ready to give the bastard a little Hell\n"
        + "of your own making!";

    public static string P4TEXT =>
        "The Gatekeeper's evil face is splattered\n"
        + "all over the place.  As its tattered corpse\n"
        + "collapses, an inverted Gate forms and\n"
        + "sucks down the shards of the last\n"
        + "prototype Accelerator, not to mention the\n"
        + "few remaining demons.  You're done. Hell\n"
        + "has gone back to pounding bad dead folks \n"
        + "instead of good live ones.  Remember to\n"
        + "tell your grandkids to put a rocket\n"
        + "launcher in your coffin. If you go to Hell\n"
        + "when you die, you'll need it for some\n"
        + "final cleaning-up ...";

    public static string P5TEXT =>
        "You've found the second-hardest level we\n"
        + "got. Hope you have a saved game a level or\n"
        + "two previous.  If not, be prepared to die\n"
        + "aplenty. For master marines only.";

    public static string P6TEXT =>
        "Betcha wondered just what WAS the hardest\n"
        + "level we had ready for ya?  Now you know.\n"
        + "No one gets out alive.";

    // TNT texts
    public static string T1TEXT =>
        "You've fought your way out of the infested\n"
        + "experimental labs.   It seems that UAC has\n"
        + "once again gulped it down.  With their\n"
        + "high turnover, it must be hard for poor\n"
        + "old UAC to buy corporate health insurance\n"
        + "nowadays..\n"
        + "\n"
        + "Ahead lies the military complex, now\n"
        + "swarming with diseased horrors hot to get\n"
        + "their teeth into you. With luck, the\n"
        + "complex still has some warlike ordnance\n"
        + "laying around.";

    public static string T2TEXT =>
        "You hear the grinding of heavy machinery\n"
        + "ahead.  You sure hope they're not stamping\n"
        + "out new hellspawn, but you're ready to\n"
        + "ream out a whole herd if you have to.\n"
        + "They might be planning a blood feast, but\n"
        + "you feel about as mean as two thousand\n"
        + "maniacs packed into one mad killer.\n"
        + "\n"
        + "You don't plan to go down easy.";

    public static string T3TEXT =>
        "The vista opening ahead looks real damn\n"
        + "familiar. Smells familiar, too -- like\n"
        + "fried excrement. You didn't like this\n"
        + "place before, and you sure as hell ain't\n"
        + "planning to like it now. The more you\n"
        + "brood on it, the madder you get.\n"
        + "Hefting your gun, an evil grin trickles\n"
        + "onto your face. Time to take some names.";

    public static string T4TEXT =>
        "Suddenly, all is silent, from one horizon\n"
        + "to the other. The agonizing echo of Hell\n"
        + "fades away, the nightmare sky turns to\n"
        + "blue, the heaps of monster corpses start \n"
        + "to evaporate along with the evil stench \n"
        + "that filled the air. Jeeze, maybe you've\n"
        + "done it. Have you really won?\n"
        + "\n"
        + "Something rumbles in the distance.\n"
        + "A blue light begins to glow inside the\n"
        + "ruined skull of the demon-spitter.";

    public static string T5TEXT =>
        "What now? Looks totally different. Kind\n"
        + "of like King Tut's condo. Well,\n"
        + "whatever's here can't be any worse\n"
        + "than usual. Can it?  Or maybe it's best\n"
        + "to let sleeping gods lie..";

    public static string T6TEXT =>
        "Time for a vacation. You've burst the\n"
        + "bowels of hell and by golly you're ready\n"
        + "for a break. You mutter to yourself,\n"
        + "Maybe someone else can kick Hell's ass\n"
        + "next time around. Ahead lies a quiet town,\n"
        + "with peaceful flowing water, quaint\n"
        + "buildings, and presumably no Hellspawn.\n"
        + "\n"
        + "As you step off the transport, you hear\n"
        + "the stomp of a cyberdemon's iron shoe.";

    //
    // Character cast strings F_FINALE.C
    //
    public static string CC_ZOMBIE => "ZOMBIEMAN";
    public static string CC_SHOTGUN => "SHOTGUN GUY";
    public static string CC_HEAVY => "HEAVY WEAPON DUDE";
    public static string CC_IMP => "IMP";
    public static string CC_DEMON => "DEMON";
    public static string CC_LOST => "LOST SOUL";
    public static string CC_CACO => "CACODEMON";
    public static string CC_HELL => "HELL KNIGHT";
    public static string CC_BARON => "BARON OF HELL";
    public static string CC_ARACH => "ARACHNOTRON";
    public static string CC_PAIN => "PAIN ELEMENTAL";
    public static string CC_REVEN => "REVENANT";
    public static string CC_MANCU => "MANCUBUS";
    public static string CC_ARCH => "ARCH-VILE";
    public static string CC_SPIDER => "THE SPIDER MASTERMIND";
    public static string CC_CYBER => "THE CYBERDEMON";
    public static string CC_HERO => "OUR HERO";
}
