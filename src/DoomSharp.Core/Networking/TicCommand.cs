namespace DoomSharp.Core.Networking;

public struct TicCommand
{
    public sbyte ForwardMove { get; set; }
    public sbyte SideMove { get; set; }
    public byte AngleTurn { get; set; }
    public short Consistency { get; set; }
    public byte ChatChar { get; set; }
    public byte Buttons { get; set; }
}