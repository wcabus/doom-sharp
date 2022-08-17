namespace DoomSharp.Core.Data;

public class WadFileCollection : List<WadFile>, IDisposable
{
    /// <summary>
    /// Pass a null terminated list of files to use.
    /// All files are optional, but at least one file
    ///  must be found.
    /// Files with a .wad extension are idlink files
    ///  with multiple lumps.
    /// Other files are single lumps with the base filename
    ///  for the lump name.
    /// Lump names can appear multiple times.
    /// The name searcher looks backwards, so a later file
    ///  does override all earlier ones.
    /// </summary>
    /// <param name="files"></param>
    public static async Task<WadFileCollection> InitializeMultipleFilesAsync(IEnumerable<string> files)
    {
        var collection = new WadFileCollection();

        foreach (var file in files)
        {
            var wadFile = await WadFile.LoadFromFileAsync(file);
            if (wadFile != null)
            {
                collection.Add(wadFile);
            }
        }

        if (collection.LumpCount < 1)
        {
            throw new Exception("W_InitFiles: no files found");
        }

        return collection;
    }

    public int LumpCount => Count == 0 ? 0 : this.Sum(x => x.LumpCount);

    public void Dispose()
    {
        foreach (var item in this)
        {
            item.Dispose();
        }
    }
}