namespace DoomSharp.Core.Networking;

public struct DoomData
{
    public uint CheckSum { get; set; }
    public int RetransmitFrom { get; set; }
    public int StartTic { get; set; }
    public int Player { get; set; }
    public int NumTics { get; set; }
    public TicCommand[] Commands { get; set; }
}