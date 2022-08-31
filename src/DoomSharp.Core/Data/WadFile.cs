using System.Runtime.InteropServices;
using System.Text;

namespace DoomSharp.Core.Data;

public class WadFile : IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WadInfo
    {
        // Should be "IWAD" or "PWAD".
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Identification;

        [MarshalAs(UnmanagedType.I4)]
        public int NumLumps;

        [MarshalAs(UnmanagedType.I4)]
        public int InfoTableOfs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileLump
    {
        [MarshalAs(UnmanagedType.I4)]
        public int FilePos;

        [MarshalAs(UnmanagedType.I4)]
        public int Size;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Name;
    }

    public static WadFile? LoadFromFile(string file)
    {
        var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
        var br = new BinaryReader(fs, Encoding.ASCII, false);

        DoomGame.Console.WriteLine($" adding {file}");

        if (string.Equals(Path.GetExtension(file), ".wad", StringComparison.OrdinalIgnoreCase))
        {
            // WAD file
            return LoadWad(file, br);
        }
        else
        {
            // Single lump file
            // TODO
        }

        return null;
    }

    private static WadFile? LoadWad(string file, BinaryReader reader)
    {
        var wadFile = new WadFile(reader)
        {
            Header = reader.ReadStruct<WadInfo>()
        };

        if (string.Equals(wadFile.Header.Identification, "IWAD", StringComparison.Ordinal) == false)
        {
            if (string.Equals(wadFile.Header.Identification, "PWAD", StringComparison.Ordinal) == false)
            {
                DoomGame.Error($"Wad file {file} doesn't have IWAD or PWAD id");
                return null;
            }
        }

        var fileInfo = new List<WadLump>(wadFile.LumpCount);

        reader.BaseStream.Seek(wadFile.Header.InfoTableOfs, SeekOrigin.Begin);
        for (var i = 0; i < wadFile.LumpCount; i++)
        {
            var lump = reader.ReadStruct<FileLump>();
            fileInfo.Add(new WadLump(wadFile, lump));
        }

        wadFile.Lumps = fileInfo;

        return wadFile;
    }

    private readonly BinaryReader _reader;

    public WadFile(BinaryReader reader)
    {
        _reader = reader;
        Header = reader.ReadStruct<WadInfo>();
    }

    public WadInfo Header { get; private init; }
    public ICollection<WadLump> Lumps { get; set; } = Array.Empty<WadLump>();
    public int LumpCount => Header.NumLumps;

    public void Dispose()
    {
        _reader.Dispose();
    }

    public void ReadLumpData(WadLump destination)
    {
        _reader.BaseStream.Seek(destination.Lump.FilePos, SeekOrigin.Begin);
        destination.Data = _reader.ReadBytes(destination.Lump.Size);
    }
}