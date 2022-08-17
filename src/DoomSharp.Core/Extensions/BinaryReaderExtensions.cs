using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.IO;

internal static class BinaryReaderExtensions
{
    public static T ReadStruct<T>(this BinaryReader reader) where T : struct
    {
        var buffer = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        try
        {
            return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }
    }
}