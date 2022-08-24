namespace DoomSharp.Core.Graphics;

public static class BoundingBox
{
    public const int BoxTop = 0;
    public const int BoxBottom = 1;
    public const int BoxLeft = 2;
    public const int BoxRight = 3;

    public static void ClearBox(Fixed[] box)
    {
        box[BoxTop] = box[BoxRight] = Fixed.MinValue;
        box[BoxBottom] = box[BoxLeft] = Fixed.MaxValue;
    }

    public static void AddToBox(Fixed[] box, Fixed x, Fixed y)
    {
        if (x.IntVal < box[BoxLeft].IntVal)
        {
            box[BoxLeft] = x;
        }
        else if (x.IntVal > box[BoxRight].IntVal)
        {
            box[BoxRight] = x;
        }

        if (y.IntVal < box[BoxBottom].IntVal)
        {
            box[BoxBottom] = y;
        }
        else if (y.IntVal > box[BoxTop].IntVal)
        {
            box[BoxTop] = y;
        }
    }
}