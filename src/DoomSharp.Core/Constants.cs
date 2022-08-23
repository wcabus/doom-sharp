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

    public const int DemoMarker = 0x80;

    // Networking and tick handling related
    public const int BackupTics = 12;

    public const int DoomComId = 0x12345678;
    public const int MaxPlayers = 4;
    public const int MaxNetNodes = 8; // 4 players max + drones

    public const int MaxEvents = 64;

    public const char HuFontStart = '!';
    public const char HuFontEnd = '_';
    public const int HuFontSize = HuFontEnd - HuFontStart + 1;

    public static class NetCommands
    {
        public const uint Exit = 0x80000000;
        public const int Retransmit = 0x40000000;
        public const int Setup = 0x20000000;
        public const int Kill = 0x10000000; // kill game
        public const int CheckSum = 0x0fffffff;
    }
}