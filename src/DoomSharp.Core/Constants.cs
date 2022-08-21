namespace DoomSharp.Core;

public static class Constants
{
    public const int BaseWidth = 320;

    public const int ScreenWidth = 320;
    public const int ScreenHeight = 200;
    public const int LineHeight = 1;

    public const int ScreenMul = 1;
    public const float InvertedAspectRatio = 0.625f;
    
    /// <summary>
    /// State updates, number of ticks / second
    /// </summary>
    public const int TicRate = 35;

    // Networking and tick handling related
    public const int BackupTics = 12;

    public const int MaxPlayers = 4;
    public const int MaxNetNodes = 8; // 4 players max + drones

    public const int MaxEvents = 64;

    public const char HuFontStart = '!';
    public const char HuFontEnd = '_';
    public const int HuFontSize = HuFontEnd - HuFontStart + 1;
}