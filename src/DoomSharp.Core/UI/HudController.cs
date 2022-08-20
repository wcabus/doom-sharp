using DoomSharp.Core.Data;
using DoomSharp.Core.Graphics;

namespace DoomSharp.Core.UI
{
    public class HudController
    {
        public HudController()
        {
            var j = (int)Constants.HuFontStart;
            for (var i = 0; i < Constants.HuFontSize; i++)
            {
                var lump = DoomGame.Instance.WadData.GetLumpName($"STCFN{j++:000}", PurgeTag.Cache);
                Font[i] = Patch.FromBytes(lump);
            }
        }

        public Patch[] Font { get; } = new Patch[Constants.HuFontSize];
    }
}