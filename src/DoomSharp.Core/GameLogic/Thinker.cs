namespace DoomSharp.Core.GameLogic;

public class Thinker
{
    public Action? Acv { get; set; }
    public Action<Thinker>? Acp1 { get; set; }
    public Action<object, object>? Acp2 { get; set; }
}