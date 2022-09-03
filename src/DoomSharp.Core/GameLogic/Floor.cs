using DoomSharp.Core.Graphics;

namespace DoomSharp.Core.GameLogic;

public class Floor : Thinker
{
    public const int FloorSpeed = Constants.FracUnit;

    public FloorType Type { get; set; }
    public bool Crush { get; set; }
    public Sector? Sector { get; set; }
    public int Direction { get; set; }
    public int NewSpecial { get; set; }
    public short Texture { get; set; }
    public Fixed FloorDestHeight { get; set; }
    public Fixed Speed { get; set; }
}