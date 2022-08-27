namespace DoomSharp.Core;

public readonly struct Fixed
{
    private readonly int _value;

    public Fixed(int value)
    {
        _value = value;
    }

    public static readonly Fixed MinValue = new(int.MinValue);
    public static readonly Fixed MaxValue = new(int.MaxValue);
    public static readonly Fixed Zero = new(0);

    public override string ToString()
    {
        var leftVal = (short)(_value >> 16);
        var rightVal = (ushort)((uint)_value & 0b1111_1111_1111_1111);
        if (rightVal == 0)
        {
            return leftVal.ToString();
        }

        return $"{leftVal}.{rightVal}";
    }

    public static Fixed Div2(Fixed a, Fixed b)
    {
        var c = ((double)a._value) / ((double)b._value) * Constants.FracUnit;

        if (c is >= 2147483648.0 or < -2147483648.0)
        {
            DoomGame.Error("FixedDiv: divide by zero");
            return new Fixed(0);
        }

        return new Fixed((int)c);
    }

    public static Fixed operator +(Fixed a, Fixed b)
    {
        return new Fixed(a._value + b._value);
    }

    public static Fixed operator -(Fixed a, Fixed b)
    {
        return new Fixed(a._value - b._value);
    }

    public static Fixed operator -(Fixed a)
    {
        return new Fixed(-a._value);
    }

    public static Fixed operator *(Fixed a, Fixed b)
    {
        var newIntVal = (a._value * b._value) >> Constants.FracBits;
        return new Fixed(newIntVal);
    }

    public static Fixed operator /(Fixed a, Fixed b)
    {
        if ((Math.Abs(a._value) >> 14) >= Math.Abs(b._value))
        {
            return new Fixed((a._value ^ b._value) < 0 ? int.MaxValue : int.MinValue);
        }

        return Div2(a, b);
    }

    public static implicit operator int(Fixed a) => a._value;
    public static implicit operator Fixed(int a) => new(a);

    public static implicit operator uint(Fixed a) => (uint)a._value;
}