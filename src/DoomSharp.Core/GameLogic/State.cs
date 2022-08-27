namespace DoomSharp.Core.GameLogic;

public class State
{
    public SpriteNum Sprite { get; set; }
    public long Frame { get; set; }
    public long Tics { get; set; }
    public Action? Action { get; set; }
    public StateNum NextState { get; set; }
    public long Misc1 { get; set; }
    public long Misc2 { get; set; }
}