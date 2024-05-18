using DoomSharp.Core.Graphics;

namespace DoomSharp.Core;

public static class DoomMath
{
    public const int FineAngleCount = 8192;
    public const int FineMask = FineAngleCount - 1;
    public const int AngleToFineShift = 19;

    private const float PI = 3.141592654f;

    public static void InitTables()
    {
        // viewangle tangent table
        for (var i = 0; i < (FineAngleCount / 2); i++)
        {
            var angle = (i - FineAngleCount / 4.0 + 0.5) * PI * 2 / FineAngleCount;
            var fv = Constants.FracUnit * Math.Tan(angle);
            FineTangent[i] = (int)fv;
        }

        // finesine table
        const int fineCosOffset = FineAngleCount / 4;
        for (var i = 0; i < (5 * FineAngleCount / 4); i++)
        {
            var angle = (i + 0.5) * PI * 2 / FineAngleCount;
            var t = Constants.FracUnit * Math.Sin(angle);
            FineSine[i] = (int)t;

            var c = i - fineCosOffset;
            if (c < 0)
            {
                c += 5 * FineAngleCount / 4;
            }
            FineCosine[c] = (int)t;
        }
    }

    public static void InitPointToAngle()
    {
        for (var i = 0; i <= RenderEngine.SlopeRange; i++)
        {
            var f = (float)Math.Atan((float)i / RenderEngine.SlopeRange) / (PI * 2); // PI was wrong here: 3.141592657f
            var t = (long)(0xffffffff * f);
            TanToAngle[i] = new Angle((uint)t);
        }
    }

    public static Fixed Tan(Angle angle)
    {
        var idx = angle.Value >> AngleToFineShift;
        if (idx > (FineAngleCount / 2))
        {
            idx -= FineAngleCount / 2;
        }

        return new Fixed(FineTangent[idx]);
    }

    public static Fixed Tan(int fineAngle)
    {
        if (fineAngle > (FineAngleCount / 2))
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
    private static readonly int[] FineCosine = new int[5 * FineAngleCount / 4];
    public static readonly Angle[] TanToAngle = new Angle[RenderEngine.SlopeRange + 1];
}