using DoomSharp.Core.Data;

namespace DoomSharp.Core.Abstractions
{
    public interface IWadStreamProvider
    {
        Task<WadFile?> LoadFromFile(string file);
    }
}
