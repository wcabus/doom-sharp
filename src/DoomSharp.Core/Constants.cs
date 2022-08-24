namespace DoomSharp.Core;

public static class Constants
{
    public const int BaseWidth = 320;

    public const int ScreenWidth = 320;
    public const int ScreenHeight = 200;
    
    public const int ScreenMul = 1;
    public const float InvertedAspectRatio = 0.625f;

    public const int MaxDrawSegs = 256;

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
    public const int ResendCount = 10;
    public const int PlayerDrone = 0x80;

    public const int MaxEvents = 64;

    // Fixed point, 32 bit as 16.16
    public const int FracBits = 16;
    public const int FracUnit = (1 << FracBits);

    public const int FloatSpeed = FracUnit * 4;
    public const int MaxHealth = 100;
    public const int ViewHeight = FracUnit * 41;

    // LineDef attributes
    
    /// <summary>
    /// Solid, is an obstacle.
    /// </summary>
    public const int LineBlocking = 1;

    /// <summary>
    /// Block monsters only.
    /// </summary>
    public const int LineBlockMonsters = 2;

    /// <summary>
    /// Backside will not be present at all if not two sided.
    /// </summary>
    public const int LineTwoSided = 4;

    // mapblocks are used to check movement
    // against lines and things
    public const int MapBlockunits = 128;
    public const int MapBlockSize = (MapBlockunits * FracUnit);
    public const int MapBlockShift = (FracBits + 7);
    public const int MapBlockMask = (MapBlockSize - 1);
    public const int MapBlockToFrac = (MapBlockShift - FracBits);

    // player radius for movement checking
    public const int PlayerRadius = 16 * FracUnit;

    // MAXRADIUS is for precalculated sector block boxes
    // the spider demon is larger,
    // but we do not have any moving sectors nearby
    public const int MaxRadius = 32 * FracUnit;

    public const int Gravity = FracUnit;
    public const int MaxMove = (30 * FracUnit);

    public const int UseRange = (64 * FracUnit);
    public const int MeleeRange = (64 * FracUnit);
    public const int MissileRange = (32 * 64 * FracUnit);

    // follow a player exclusively for 3 seconds
    public const int BaseThreshold = 100;

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