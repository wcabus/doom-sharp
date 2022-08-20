namespace DoomSharp.Core.Graphics;

/// <summary>
/// Patches.
/// A patch holds one or more columns.
/// Patches are used for sprites and all masked pictures,
/// and we compose textures from the TEXTURE1/2 lists
/// of patches.
/// </summary>
public record Patch(short Width, short Height, short LeftOffset, short TopOffset, int[] ColumnOffsets, Column[] Columns)
{
    public static Patch FromBytes(byte[] patchData)
    {
        using var stream = new MemoryStream(patchData, false);
        using var reader = new BinaryReader(stream);

        var width = reader.ReadInt16();
        var height = reader.ReadInt16();
        var left = reader.ReadInt16();
        var top = reader.ReadInt16();

        var offsets = new int[width];
        for (var i = 0; i < width; i++)
        {
            offsets[i] = reader.ReadInt32();
        }

        var columns = new Column[width];
        for (var i = 0; i < width; i++)
        {
            stream.Seek(offsets[i], SeekOrigin.Begin);
            
            var rowStart = reader.ReadByte();
            if (rowStart == 255)
            {
                columns[i] = new Column(255, 0, Array.Empty<byte>());
                continue;
            }

            var columnPixels = new List<byte>();
            var columnRowStart = -1;

            while (rowStart != 255)
            {
                if (columnRowStart == -1)
                {
                    columnRowStart = rowStart;
                }

                var pixelCount = reader.ReadByte();
                _ = reader.ReadByte(); // dummy value
                var pixels = reader.ReadBytes(pixelCount);

                columnPixels.AddRange(pixels);
                
                _ = reader.ReadByte(); // dummy value

                rowStart = reader.ReadByte();
            }

            if (columnRowStart == -1)
            {
                columnRowStart = 255;
            }
            columns[i] = new Column((byte)columnRowStart, (byte)columnPixels.Count, columnPixels.ToArray());
        }

        return new Patch(width, height, left, top, offsets, columns);
    }
}