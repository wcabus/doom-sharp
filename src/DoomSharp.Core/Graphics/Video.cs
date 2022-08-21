namespace DoomSharp.Core.Graphics;

public class Video
{
    private IGraphics _graphics;
    
    private readonly byte[][] _screens = new byte[5][];
    private readonly float[] _dirtyBox = new float[4];

    public Video(IGraphics graphics)
    {
        _graphics = graphics;
    }

    public void Initialize()
    {
        for (var i = 0; i < 4; i++)
        {
            _screens[i] = new byte[Constants.ScreenWidth * Constants.ScreenHeight];
        }

        ClearBox(_dirtyBox);
    }

    public void SetOutputRenderer(IGraphics renderer)
    {
        _graphics = renderer;
    }

    public void SignalOutputReady(int screen = 0)
    {
        _graphics.ScreenReady(_screens[screen]);
    }

    //
    // V_DrawPatch
    // Masks a column based masked pic to the screen. 
    //
    public void DrawPatch(int x, int y, int screen, Patch patch)
    {
        y -= patch.TopOffset;
        x -= patch.LeftOffset;

        // Range check
        if (x < 0 ||
            (x + patch.Width) > Constants.ScreenWidth ||
            y < 0 ||
            (y + patch.Height) > Constants.ScreenHeight ||
            screen > 4)
        {
            DoomGame.Console.WriteLine($"Patch at {x}, {y} exceeds LFB");
            DoomGame.Console.WriteLine("V_DrawPatch: bad patch (ignored)");
            return;
        }
        // End range check

        if (screen == 0)
        {
            MarkRectangle(x, y, patch.Width, patch.Height);
        }

        var col = 0;
        var destTop = y * Constants.ScreenWidth + x;
        var width = patch.Width;

        for (; col < width; x++, col++, destTop++)
        {
            var column = patch.Columns[col];

            if (column is null || column.TopDelta == 255)
            {
                continue;
            }

            // step through the posts in a column 
            while (column != null && column.TopDelta != 255)
            {
                var source = 0;
                var dest = destTop + column.TopDelta * Constants.ScreenWidth;
                var count = column.Length;

                while (count-- > 0)
                {
                    _screens[screen][dest] = column.Pixels[source++];
                    dest += Constants.ScreenWidth;
                }

                column = column.Next;
            }
        }
    }

    //
    // V_DrawPatch
    // Masks a column based masked pic to the screen. 
    //
    public void DrawPatch(int x, int y, int screen, byte[] patch)
    {
        DrawPatch(x, y, screen, Patch.FromBytes(patch));
    }

    //
    // V_DrawPatchDirect
    // Draws directly to the screen on the pc. 
    //
    public void DrawPatchDirect(int x, int y, int screen, Patch patch)
    {
        DrawPatch(x, y, screen, patch);
    }

    //
    // V_DrawPatchDirect
    // Draws directly to the screen on the pc. 
    //
    public void DrawPatchDirect(int x, int y, int screen, byte[] patch)
    {
        DrawPatch(x, y, screen, Patch.FromBytes(patch));
    }

    private void MarkRectangle(float x, float y, float width, float height)
    {
        AddToBox(_dirtyBox, x, y);
        AddToBox(_dirtyBox, x + width - 1, y + height - 1);
    }

    // Bounding box functions
    private void ClearBox(IList<float> box)
    {
        box[2] = box[3] = float.MinValue;
        box[0] = box[1] = float.MaxValue;
    }

    private void AddToBox(IList<float> box, float x, float y)
    {
        if (x < box[0])
        {
            box[0] = x;
        }
        else if (x > box[3])
        {
            box[3] = x;
        }

        if (y < box[1])
        {
            box[1] = y;
        }
        else if (y > box[2])
        {
            box[2] = y;
        }
    }
}