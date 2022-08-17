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
    
    public static async Task<WadFile?> LoadFromFileAsync(string file)
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

        var fileInfo = new List<FileLump>(wadFile.LumpCount);

        reader.BaseStream.Seek(wadFile.Header.InfoTableOfs, SeekOrigin.Begin);
        for (var i = 0; i < wadFile.LumpCount; i++)
        {
            var lump = reader.ReadStruct<FileLump>();
            fileInfo.Add(lump);
        }

        wadFile.Lumps = fileInfo;

        return wadFile;
    }

    private readonly BinaryReader _reader;

    public WadFile(BinaryReader reader)
    {
        _reader = reader;
    }

    public WadInfo Header { get; private init; }
    public ICollection<FileLump> Lumps { get; private set; } = Array.Empty<FileLump>();
    public int LumpCount => Header.NumLumps;

    public void Dispose()
    {
        _reader.Dispose();
    }
}