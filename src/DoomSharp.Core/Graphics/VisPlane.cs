namespace DoomSharp.Core.Graphics;

public class VisPlane
{
    public VisPlane()
    {
        Height = default;
        PicNum = 0;
        LightLevel = 0;
        MinX = 0;
        MaxX = 0;
        Pad1 = 0;
        Pad2 = 0;
        Pad3 = 0;
        Pad4 = 0;
    }

    public Fixed Height { get; set; }
    public int PicNum { get; set; }
    public int LightLevel { get; set; }
    public int MinX { get; set; }
    public int MaxX { get; set; }

    // leave pads for [minx-1]/[maxx+1]

    private byte Pad1 { get; set; }
    // Here lies the rub for all
    //  dynamic resize/change of resolution.
    private byte[] Top { get; } = new byte[Constants.ScreenWidth];
    private byte Pad2 { get; set; }
    private byte Pad3 { get; set; }
    // See above.
    private byte[] Bottom { get; } = new byte[Constants.ScreenWidth];
    private byte Pad4 { get; set; }

    public byte ReadTop(int i)
    {
        return i switch
        {
            < 0 => Pad1,
            >= 320 => Pad2,
            _ => Top[i]
        };
    }

    public void WriteTop(int i, byte value)
    {
        switch (i)
        {
            case < 0:
                Pad1 = value;
                break;
            case >= 320:
                Pad2 = value;
                break;
            default:
                Top[i] = value;
                break;
        }
    }

    public byte ReadBottom(int i)
    {
        return i switch
        {
            < 0 => Pad3,
            >= 320 => Pad4,
            _ => Bottom[i]
        };
    }

    public void WriteBottom(int i, byte value)
    {
        switch (i)
        {
            case < 0:
                Pad3 = value;
                break;
            case >= 320:
                Pad4 = value;
                break;
            default:
                Bottom[i] = value;
                break;
        }
    }

    public void FillTop(byte value)
    {
        Top[0] = value;
        //for (var i = 0; i < Top.Length; i++)
        //{
        //    Top[i] = value;
        //}
    }
}