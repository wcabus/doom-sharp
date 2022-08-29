using DoomSharp.Core.Input;

namespace DoomSharp.Core.Networking;

public class TicCommand
{
    public sbyte ForwardMove { get; set; }
    public sbyte SideMove { get; set; }
    public byte AngleTurn { get; set; }
    public short Consistency { get; set; }
    public char ChatChar { get; set; }
    public ButtonCode Buttons { get; set; }
}