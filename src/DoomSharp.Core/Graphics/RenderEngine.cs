using System.Text;
using DoomSharp.Core.Data;
using DoomSharp.Core.Extensions;

namespace DoomSharp.Core.Graphics;

public class RenderEngine
{
    private int _frameCount = 0;

    private int _firstFlat;
    private int _lastFlat;
    private int _numFlats;

    private int _firstPatch;
    private int _lastPatch;
    private int _numPatches;

    private int _firstSpriteLump;
    private int _lastSpriteLump;
    private int _numSpriteLumps;

    private int _numTextures;
    private Texture[] _textures = Array.Empty<Texture>();

    private int[] _textureWidthMask = Array.Empty<int>();
    // needed for texture pegging
    private Fixed[] _textureHeight = Array.Empty<Fixed>();
    private int[] _textureCompositeSize = Array.Empty<int>();
    private short[][] _textureColumnLump = Array.Empty<short[]>();
    private ushort[][] _textureColumnOfs = Array.Empty<ushort[]>();
    private byte[][] _textureComposite = Array.Empty<byte[]>();
    
    // for global animation
    private int[] _flatTranslation = Array.Empty<int>();
    private int[] _textureTranslation = Array.Empty<int>();

    // needed for pre rendering
    private Fixed[] _spriteWidth = Array.Empty<Fixed>();
    private Fixed[] _spriteOffset = Array.Empty<Fixed>();
    private Fixed[] _spriteTopOffset = Array.Empty<Fixed>();

    private byte[] _colorMaps = Array.Empty<byte>();

    public void InitData()
    {
        InitTextures();
        DoomGame.Console.Write($"{Environment.NewLine}InitTextures");

        InitFlats();
        DoomGame.Console.Write($"{Environment.NewLine}InitFlats");

        InitSpriteLumps();
        DoomGame.Console.Write($"{Environment.NewLine}InitSpriteLumps");

        InitColormaps();
        DoomGame.Console.Write($"{Environment.NewLine}InitColormaps");
    }

    // Initializes the texture list
    //  with the textures from the world map.
    private void InitTextures()
    {
        // Load the patch names from pnames.lmp.
        var names = DoomGame.Instance.WadData.GetLumpName("PNAMES", PurgeTag.Static)!;
        var numMapPatches = DoomConvert.ToInt32(names[..4]);
        var patchLookup = new int[numMapPatches];

        for (var i = 0; i < numMapPatches; i++)
        {
            var name = Encoding.ASCII.GetString(names, 4 + (i * 8), 8).TrimEnd('\0');
            patchLookup[i] = DoomGame.Instance.WadData.CheckNumForName(name);
        }

        // Load the map texture definitions from textures.lmp.
        // The data is contained in one or two lumps,
        //  TEXTURE1 for shareware, plus TEXTURE2 for commercial.
        var mapTex = DoomGame.Instance.WadData.GetLumpName("TEXTURE1", PurgeTag.Static)!;
        var numTextures1 = DoomConvert.ToInt32(mapTex[..4]);

        var maxOff = DoomGame.Instance.WadData.LumpLength(DoomGame.Instance.WadData.GetNumForName("TEXTURE1"));
        var directory = 4;

        byte[]? mapTex2 = null;
        var numTextures2 = 0;
        var maxOff2 = 0;
        if (DoomGame.Instance.WadData.CheckNumForName("TEXTURE2") != -1)
        {
            mapTex2 = DoomGame.Instance.WadData.GetLumpName("TEXTURE2", PurgeTag.Static)!;
            numTextures2 = DoomConvert.ToInt32(mapTex2[..4]);
            maxOff2 = DoomGame.Instance.WadData.LumpLength(DoomGame.Instance.WadData.GetNumForName("TEXTURE2"));
        }
        _numTextures = numTextures1 + numTextures2;

        _textures = new Texture[_numTextures];

        _textureColumnLump = new short[_numTextures][];
        _textureColumnOfs = new ushort[_numTextures][];
        _textureComposite = new byte[_numTextures][];
        _textureCompositeSize = new int[_numTextures];
        _textureWidthMask = new int[_numTextures];
        _textureHeight = new Fixed[_numTextures];
        
        var totalWidth = 0;

        //	Really complex printing shit...
        var temp1 = DoomGame.Instance.WadData.GetNumForName("S_START");  // P_???????
        var temp2 = DoomGame.Instance.WadData.GetNumForName("S_END") - 1;
        var temp3 = ((temp2 - temp1 + 63) / 64) + ((_numTextures + 63) / 64);
        DoomGame.Console.Write("[");
        for (var i = 0; i < temp3; i++)
        {
            DoomGame.Console.Write(" ");
        }
        DoomGame.Console.Write("         ]");
        for (var i = 0; i < temp3; i++)
        {
            DoomGame.Console.Write("\b");
        }
        DoomGame.Console.Write("\b\b\b\b\b\b\b\b\b\b");

        for (var i = 0; i < _numTextures; i++, directory += 4)
        {
            if ((i & 63) == 0)
            {
                DoomGame.Console.Write(".");
            }

            if (i == numTextures1 && mapTex2 != null)
            {
                // Start looking in the second texture file.
                mapTex = mapTex2;
                maxOff = maxOff2;
                directory = 4;
            }

            var offset = DoomConvert.ToInt32(mapTex[directory..(directory + 4)]);
            if (offset > maxOff)
            {
                DoomGame.Error("R_InitTextures: bad texture directory");
                return;
            }

            var mapTexture = MapTexture.FromBytes(mapTex[offset..(offset + MapTexture.BaseSize)]);
            mapTexture.ReadMapPatches(mapTex[(offset + MapTexture.BaseSize)..(offset + mapTexture.Size)]);

            _textures[i] = new Texture(mapTexture.Name, mapTexture.Width, mapTexture.Height, mapTexture.PatchCount);
            
            var texture = _textures[i];
            for (var j = 0; j < texture.PatchCount; j++)
            {
                var mpatch = mapTexture.Patches[j];
                texture.Patches[j] = new TexturePatch(mpatch.OriginX, mpatch.OriginY, patchLookup[mpatch.Patch]);
                if (texture.Patches[j].Patch == -1)
                {
                    DoomGame.Error($"R_InitTextures: Missing patch in texture {texture.Name}");
                    return;
                }
            }

            _textureColumnLump[i] = new short[texture.Width];
            _textureColumnOfs[i] = new ushort[texture.Width];
            
            var k = 1;
            while (k * 2 <= texture.Width)
            {
                k <<= 1;
            }

            _textureWidthMask[i] = k - 1;
            _textureHeight[i] = new Fixed(texture.Height << Constants.FracBits);
            totalWidth += texture.Width;
        }

        // Precalculate whatever possible.	
        for (var i = 0; i < _numTextures; i++)
        {
            GenerateLookup(i);
        }

        // Create translation table for global animation.
        _textureTranslation = new int[_numTextures + 1];
        for (var i = 0; i < _numTextures; i++)
        {
            _textureTranslation[i] = i;
        }
    }

    private void GenerateLookup(int texNum)
    {
        var texture = _textures[texNum];
        
        // Composited texture not created yet.
        _textureComposite[texNum] = Array.Empty<byte>();

        _textureCompositeSize[texNum] = 0;
        var colLump = _textureColumnLump[texNum];
        var colOfs = _textureColumnOfs[texNum];

        // Now count the number of columns
        //  that are covered by more than one patch.
        // Fill in the lump / offset, so columns
        //  with only a single patch are all done.
        var patchCount = new byte[texture.Width];
        var x = 0;

        for (var i = 0; i < texture.PatchCount; i++)
        {
            var patch = texture.Patches[i];
            var realPatch = Patch.FromBytes(DoomGame.Instance.WadData.GetLumpNum(patch.Patch, PurgeTag.Cache)!);
            var x1 = patch.OriginX;
            var x2 = x1 + realPatch.Width;

            x = x1 < 0 ? 0 : x1;

            if (x2 > texture.Width)
            {
                x2 = texture.Width;
            }

            for (; x < x2; x++)
            {
                patchCount[x]++;
                colLump[x] = (short)patch.Patch;
                colOfs[x] = (ushort)(realPatch.ColumnOffsets[x - x1] + 3);
            }
        }

        for (x = 0; x < texture.Width; x++)
        {
            if (patchCount[x] == 0)
            {
                DoomGame.Console.WriteLine($"R_GenerateLookup: column without a patch ({texture.Name})");
                return;
            }

            // I_Error ("R_GenerateLookup: column without a patch");

            if (patchCount[x] > 1)
            {
                // Use the cached block.
                colLump[x] = -1;
                colOfs[x] = (ushort)_textureCompositeSize[texNum];

                if (_textureCompositeSize[texNum] > 0x10000 - texture.Height)
                {
                    DoomGame.Error($"R_GenerateLookup: texture {texNum} is >64k");
                    return;
                }

                _textureCompositeSize[texNum] += texture.Height;
            }
        }
    }

    private void InitFlats()
    {
        _firstFlat = DoomGame.Instance.WadData.GetNumForName("F_START") + 1;
        _lastFlat = DoomGame.Instance.WadData.GetNumForName("F_END") - 1;
        _numFlats = _lastFlat - _firstFlat + 1;

        // Create translation table for global animation.
        _flatTranslation = new int[_numFlats + 1];

        for (var i = 0; i < _numFlats; i++)
        {
            _flatTranslation[i] = i;
        }
    }

    //
    // R_InitSpriteLumps
    // Finds the width and hoffset of all sprites in the wad,
    //  so the sprite does not need to be cached completely
    //  just for having the header info ready during rendering.
    //
    private void InitSpriteLumps()
    {
        _firstSpriteLump = DoomGame.Instance.WadData.GetNumForName("S_START") + 1;
        _lastSpriteLump = DoomGame.Instance.WadData.GetNumForName("S_END") - 1;

        _numSpriteLumps = _lastSpriteLump - _firstSpriteLump + 1;
        _spriteWidth = new Fixed[_numSpriteLumps];
        _spriteOffset = new Fixed[_numSpriteLumps];
        _spriteTopOffset = new Fixed[_numSpriteLumps];

        for (var i = 0; i < _numSpriteLumps; i++)
        {
            if ((i & 63) != 0)
            {
                DoomGame.Console.Write(".");
            }

            var patchData = DoomGame.Instance.WadData.GetLumpNum(_firstSpriteLump + i, PurgeTag.Cache);
            var patch = Patch.FromBytes(patchData!);
            _spriteWidth[i] = new Fixed(patch.Width << Constants.FracBits);
            _spriteOffset[i] = new Fixed(patch.LeftOffset << Constants.FracBits);
            _spriteTopOffset[i] = new Fixed(patch.TopOffset << Constants.FracBits);
        }
    }

    private void InitColormaps()
    {
        // Load in the light tables, 
        //  256 byte align tables.
        var lump = DoomGame.Instance.WadData.GetNumForName("COLORMAP");

        // Something weird is happening here in the original code:
        /*
        length = W_LumpLength (lump) + 255; 
        colormaps = Z_Malloc (length, PU_STATIC, 0); 
        colormaps = (byte *)( ((int)colormaps + 255)&~0xff); 
         */

        _colorMaps = DoomGame.Instance.WadData.GetLumpNum(lump, PurgeTag.Cache)!;
    }

    public int FlatNumForName(string name)
    {
        var i = DoomGame.Instance.WadData.CheckNumForName(name);
        if (i == -1)
        {
            DoomGame.Error($"R_FlatNumForName: {name} not found");
            return -1;
        }

        return i - _firstFlat;
    }

    public int TextureNumForName(string name)
    {
        var i = CheckTextureNumForName(name);
        if (i == -1)
        {
            DoomGame.Error($"R_TextureNumForName: {name} not found");
            return -1;
        }

        return i;
    }

    private int CheckTextureNumForName(string name)
    {
        // "NoTexture" marker.
        if (name[0] == '-')
        {
            return 0;
        }

        for (var i = 0; i < _numTextures; i++)
        {
            if (string.Equals(_textures[i].Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    public void Initialize()
    {
        InitData();
        DoomGame.Console.Write($"{Environment.NewLine}R_InitData");
        // R_InitPointToAngle();
        DoomGame.Console.Write($"{Environment.NewLine}R_InitPointToAngle");
        // R_InitTables();
        // viewwidth / viewheight / detailLevel are set by the defaults
        DoomGame.Console.Write($"{Environment.NewLine}R_InitTables");

        // R_SetViewSize(screenblocks, detailLevel);
        // R_InitPlanes();
        DoomGame.Console.Write($"{Environment.NewLine}R_InitPlanes");
        // R_InitLightTables();
        DoomGame.Console.Write($"{Environment.NewLine}R_InitLightTables");
        // R_InitSkyMap();
        DoomGame.Console.Write($"{Environment.NewLine}R_InitSkyMap");
        // R_InitTranslationTables();
        DoomGame.Console.Write($"{Environment.NewLine}R_InitTranslationTables");

        _frameCount = 0;
    }
}