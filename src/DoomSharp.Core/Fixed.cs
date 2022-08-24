namespace DoomSharp.Core;

public record Fixed(int IntVal)
{
    public static readonly Fixed MinValue = new(int.MinValue);
    public static readonly Fixed MaxValue = new(int.MaxValue);
    public static readonly Fixed Zero = new(0);

    public static Fixed Mul(Fixed a, Fixed b)
    {
        var newIntVal = (a.IntVal * b.IntVal) >> Constants.FracBits;
        return new Fixed(newIntVal);
    }

    public static Fixed Div(Fixed a, Fixed b)
    {
        if ((Math.Abs(a.IntVal) >> 14) >= Math.Abs(b.IntVal))
        {
            return new Fixed((a.IntVal ^ b.IntVal) < 0 ? int.MaxValue : int.MinValue);
        }

        return Div2(a, b);
    }

    public static Fixed Div2(Fixed a, Fixed b)
    {
        var c = ((double)a.IntVal) / ((double)b.IntVal) * Constants.FracUnit;

        if (c is >= 2147483648.0 or < -2147483648.0)
        {
            DoomGame.Error("FixedDiv: divide by zero");
            return new Fixed(0);
        }

        return new Fixed((int)c);
    }
}