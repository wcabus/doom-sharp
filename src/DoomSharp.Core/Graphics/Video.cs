namespace DoomSharp.Core.Graphics;

public class Video
{
    private readonly byte[][] _screens = new byte[5][];

    public void Initialize()
    {
        for (var i = 0; i < 4; i++)
        {
            _screens[i] = new byte[Constants.ScreenWidth * Constants.ScreenHeight];
        }
    }
}