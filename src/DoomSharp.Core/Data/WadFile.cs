using System.Text;

namespace DoomSharp.Core.Data;

public class WadFile : IDisposable
{
    public struct WadInfo
    {
        // Should be "IWAD" or "PWAD".
        public string Identification;
        public int NumLumps;
        public int InfoTableOfs;
    }

    public struct FileLump
    {
        public int FilePos;
        public int Size;
        public string Name;

        public static FileLump ReadFromWadData(BinaryReader reader)
        {
            return new FileLump
            {
                FilePos = reader.ReadInt32(),
                Size = reader.ReadInt32(),
                Name = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd('\0')
            };
        }
    }

    private readonly BinaryReader _reader;

    public WadFile(BinaryReader reader)
    {
        _reader = reader;
    }

    public WadInfo Header { get; init; }
    public ICollection<WadLump> Lumps { get; set; } = Array.Empty<WadLump>();
    public int LumpCount => Header.NumLumps;

    public void ReadLumpData(WadLump destination)
    {
        _reader.BaseStream.Seek(destination.Lump.FilePos, SeekOrigin.Begin);
        destination.Data = _reader.ReadBytes(destination.Lump.Size);
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}