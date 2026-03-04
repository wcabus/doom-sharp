using DoomSharp.Core.Graphics;

namespace DoomSharp.Core;

public static class DoomMath
{
    public const int FineAngleCount = 8192;
    public const int FineMask = FineAngleCount - 1;
    public const int AngleToFineShift = 19;

    public static void InitTables()
    {
        // Use the exact precomputed tables from the original DOOM source (tables.c).
        // Computing these at runtime with floating point math introduces precision
        // differences that cause demo playback desync.
        Array.Copy(DoomTables.FineTangent, FineTangent, FineAngleCount / 2);
        Array.Copy(DoomTables.FineSine, FineSine, 5 * FineAngleCount / 4);

        // finecosine is finesine offset by FINEANGLES/4, matching original DOOM:
        // int *finecosine = &finesine[FINEANGLES/4];
        const int fineCosOffset = FineAngleCount / 4;
        Array.Copy(DoomTables.FineSine, fineCosOffset, FineCosine, 0, FineAngleCount);
    }

    public static void InitPointToAngle()
    {
        // Use the exact precomputed tantoangle table from original DOOM source (tables.c).
        for (var i = 0; i <= RenderEngine.SlopeRange; i++)
        {
            TanToAngle[i] = new Angle(DoomTables.TanToAngle[i]);
        }
    }

    public static Fixed Tan(Angle angle)
    {
        var idx = angle.Value >> AngleToFineShift;
        if (idx >= (FineAngleCount / 2))
        {
            idx -= FineAngleCount / 2;
        }

        return new Fixed(FineTangent[idx]);
    }

    public static Fixed Tan(int fineAngle)
    {
        if (fineAngle >= (FineAngleCount / 2))
        {
            fineAngle -= FineAngleCount / 2;
        }
        return new Fixed(FineTangent[fineAngle]);
    }

    public static Fixed Sin(Angle angle)
    {
        return new Fixed(FineSine[angle.Value >> AngleToFineShift]);
    }

    public static Fixed Sin(int fineAngle)
    {
        return new Fixed(FineSine[fineAngle]);
    }

    public static Fixed Cos(Angle angle)
    {
        return new Fixed(FineCosine[(angle.Value >> AngleToFineShift)]);
    }

    public static Fixed Cos(int fineAngle)
    {
        return new Fixed(FineCosine[fineAngle]);
    }

    private static readonly int[] FineTangent = new int[FineAngleCount / 2];
    private static readonly int[] FineSine = new int[5 * FineAngleCount / 4];
    private static readonly int[] FineCosine = new int[FineAngleCount];
    public static readonly Angle[] TanToAngle = new Angle[RenderEngine.SlopeRange + 1];
}
