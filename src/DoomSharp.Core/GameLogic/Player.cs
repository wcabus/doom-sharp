using DoomSharp.Core.Networking;

namespace DoomSharp.Core.GameLogic;

public class Player
{
    public MapObject? MapObject { get; set; }
    public PlayerState State { get; set; } = PlayerState.NotSet;
    public TicCommand Command { get; set; }

    // Determine POV,
    //  including viewpoint bobbing during movement.
    // Focal origin above r.z
    public Fixed ViewZ { get; set; }
    // Base height above floor for viewz.
    public Fixed ViewHeight { get; set; }
    // Bob/squat speed.
    public Fixed DeltaViewHeight { get; set; }
    // bounded/scaled total momentum.
    public Fixed Bob { get; set; }

    // This is only used between levels,
    // mo->health is used during levels.
    public int Health { get; set; }
    public int ArmorPoints { get; set; }
    // Armor type is 0-2.
    public int ArmorType { get; set; }

    // Power ups. invinc and invis are tic counters.
    public int[] Powers { get; } = new int[(int)PowerUpType.NumberOfPowerUps];
    public bool[] Cards { get; } = new bool[(int)KeyCardType.NumberOfKeyCards];
    public bool Backpack { get; set; }

    // Frags, kills of other players.
    public int[] Frags { get; } = new int[Constants.MaxPlayers];
    public WeaponType ReadyWeapon { get; set; }

    // Is wp_nochange if not changing.
    public WeaponType PendingWeapon { get; set; }

    public bool[] WeaponOwned { get; } = new bool[(int)WeaponType.NumberOfWeapons];
    public int[] Ammo { get; } = new int[(int)AmmoType.NumAmmo];
    public int[] MaxAmmo { get; } = new int[(int)AmmoType.NumAmmo];

    // True if button down last tic.
    public int AttackDown { get; set; }
    public int UseDown { get; set; }

    // Bit flags, for cheats and debug.
    // See cheat_t, above.
    public int Cheats { get; set; }

    // Refired shots are less accurate.
    public int Refire { get; set; }

    // For intermission stats.
    public int KillCount { get; set; }
    public int ItemCount { get; set; }
    public int SecretCount { get; set; }

    // Hint messages.
    public string Message { get; set; }

    // For screen flashing (red or bright).
    public int DamageCount { get; set; }
    public int BonusCount { get; set; }

    // Who did damage (NULL for floors/ceilings).
    // mobj_t* attacker;

    // So gun flashes light up areas.
    public int ExtraLight { get; set; }

    // Current PLAYPAL, ???
    //  can be set to REDCOLORMAP for pain, etc.
    public int FixedColorMap { get; set; }

    // Player skin colorshift,
    //  0-3 for which color to draw player.
    public int ColorMap { get; set; }

    // Overlay view sprites (gun, etc).
    // pspdef_t psprites[NUMPSPRITES];

    // True if secret level has been done.
    public bool DidSecret { get; set; }

    public void Think()
    {
        
    }
}