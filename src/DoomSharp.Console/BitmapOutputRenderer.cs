using System.Drawing;
using DoomSharp.Core;
using DoomSharp.Core.Graphics;

namespace DoomSharp.Console;

public class BitmapGraphics : IGraphics
{
    private static int OutputIndex = 1;

    public void Initialize()
    {
        
    }

    public void ScreenReady(byte[] output)
    {
        using var bmp = new Bitmap(Constants.ScreenWidth, Constants.ScreenHeight);
        var x = 0;
        var y = 0;
        foreach (var pixel in output)
        {
            bmp.SetPixel(x++, y, GetColorFromPalette(pixel));
            if (x >= Constants.ScreenWidth)
            {
                x = 0;
                y++;
            }
        }

        bmp.Save($"C:\\temp\\DOOM\\Render{OutputIndex:000000}.bmp");
    }

    private Color GetColorFromPalette(byte pixel)
    {
        switch (pixel)
        {
            case 0:
            case 1:
            case 2:
            case 5:
            case 6:
            case 7:
            case 8:
                return Color.Black;
            case 3:
                return Color.Gray;
            case 4:
            case 168:
            case 208:
            case 224:
                return Color.White;
            case >= 9 and <= 15:
                return Color.SaddleBrown;
            case >= 16 and <= 47:
                return Color.Red;
            case >= 48 and <= 74:
                return Color.SaddleBrown;
            case >= 75 and <= 79:
                return Color.DarkKhaki;
            case >= 80 and <= 95:
                return Color.Silver;
            case >= 96 and <= 111:
                return Color.Gray;
            case >= 112 and <= 127:
                return Color.DarkGreen;
            case >= 128 and <= 151:
                return Color.Brown;
            case >= 152 and <= 159:
                return Color.Khaki;
            case >= 160 and <= 162:
                return Color.Yellow;
            case >= 163 and <= 167:
                return Color.SandyBrown;
            case >= 169 and <= 191:
                return Color.Red;
            case >= 192 and <= 207:
                return Color.Blue;
            case >= 209 and <= 223:
                return Color.Orange;
            case >= 225 and <= 231:
                return Color.Yellow;
            case >= 232 and <= 235:
                return Color.SaddleBrown;
            case >= 236 and <= 239:
                return Color.DimGray;
            case >= 240 and <= 247:
                return Color.Navy;
            case 248:
                return Color.Orange;
            case 249:
                return Color.Yellow;
            case 250:
                return Color.Pink;
            case 251:
                return Color.Magenta;
            case 252:
                return Color.DarkMagenta;
            case 253:
                return Color.Purple;
            case 254:
                return Color.Indigo;
            case 255:
                return Color.DarkSalmon;
        }
    }
}