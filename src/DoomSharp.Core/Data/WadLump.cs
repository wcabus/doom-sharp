using static DoomSharp.Core.Data.WadFile;

namespace DoomSharp.Core.Data;

public record WadLump(WadFile File, FileLump Lump)
{
    public ReadOnlyMemory<byte>? Data { get; set; }
    public PurgeTag Tag { get; set; } = PurgeTag.Cache; // Default (purgeable)
}