namespace DoomSharp.Core.Graphics;

public class DrawSegment
{
    private int[]? _spriteTopClip;
    private int[]? _spriteBottomClip;
    private int[]? _maskedTextureCol;

    public Segment? CurrentLine { get; set; }
    public int X1 { get; set; }
    public int X2 { get; set; }

    public Fixed Scale1 { get; set; }
    public Fixed Scale2 { get; set; }
    public Fixed ScaleStep { get; set; }

    // 0=none, 1=bottom, 2=top, 3=both
    public int Silhouette { get; set; }

    // do not clip sprites above this
    public Fixed BottomSilhouetteHeight { get; set; }

    // do not clip sprites below this
    public Fixed TopSilhouetteHeight { get; set; }

    // Pointers to lists for sprite clipping,
    //  all three adjusted so [x1] is first value.
    public int[]? SpriteTopClip
    {
        get => _spriteTopClip;
        set
        {
            SpriteTopClipIdx = null;
            if (value is { Length: > 0 })
            {
                SpriteTopClipIdx = 0;
            }

            _spriteTopClip = value;
        }
    }
    public int? SpriteTopClipIdx { get; set; }
    
    public int[]? SpriteBottomClip
    {
        get => _spriteBottomClip;
        set
        {
            SpriteBottomClipIdx = null;
            if (value is { Length: > 0 })
            {
                SpriteBottomClipIdx = 0;
            }

            _spriteBottomClip = value;
        }
    }
    public int? SpriteBottomClipIdx { get; set; }

    public int[]? MaskedTextureCol
    {
        get => _maskedTextureCol;
        set
        {
            MaskedTextureColIdx = null;
            if (value is { Length: > 0 })
            {
                MaskedTextureColIdx = 0;
            }

            _maskedTextureCol = value;
        }
    }
    public int? MaskedTextureColIdx { get; set; }
}