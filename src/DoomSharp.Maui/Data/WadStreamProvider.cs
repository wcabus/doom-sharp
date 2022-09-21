using System.Text;
using DoomSharp.Core;
using DoomSharp.Core.Abstractions;
using DoomSharp.Core.Data;

namespace DoomSharp.Maui.Data
{
    internal sealed class WadStreamProvider : IWadStreamProvider
    {
        public async Task<WadFile> LoadFromFile(string file)
        {
            Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(file);
            var binaryReader = new BinaryReader(fileStream, Encoding.ASCII, false);

            DoomGame.Console.WriteLine($" adding {file}");

            if (string.Equals(Path.GetExtension(file), ".WAD", StringComparison.OrdinalIgnoreCase))
            {
                // WAD file
                return LoadWad(file, binaryReader);
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
            var wadFile = new WadFile(reader);

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
                var lump = reader.ReadStruct<WadFile.FileLump>();
                fileInfo.Add(new WadLump(wadFile, lump));
            }

            wadFile.Lumps = fileInfo;

            return wadFile;
        }
    }
}
