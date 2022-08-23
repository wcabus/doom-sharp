using DoomSharp.Core.Networking;

namespace DoomSharp.Core.GameLogic;

public class Player
{
    public PlayerState State { get; set; } = PlayerState.NotSet;
    public TicCommand Command { get; set; }

    public float ViewZ { get; set; }
    public float ViewHeight { get; set; }
    public float DeltaViewHeight { get; set; }
    public float Bob { get; set; }

    public int Health { get; set; }
    public int ArmorPoints { get; set; }
    public int ArmorType { get; set; }

    public int[] Powers { get; } = new int[(int)PowerUpType.NumberOfPowerUps];
    public bool[] Cards { get; } = new bool[(int)KeyCardType.NumberOfKeyCards];
    public bool Backpack { get; set; }
}