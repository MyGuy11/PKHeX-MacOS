using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 C-Gear Background Image
/// </summary>
public sealed class CGearBackground
{
    public const string Extension = "cgb";
    public const string Filter = $"C-Gear Background|*.{Extension}";
    public const int Width = 256; // px
    public const int Height = 192; // px
    public const int SIZE_CGB = 0x2600;
    private const int ColorCount = 0x10;
    private const int TileSize = 8;
    private const int TileCount = (Width / TileSize) * (Height / TileSize); // 0x300

    /* CGearBackground Documentation
    * CGearBackgrounds (.cgb) are tiled images.
    * Tiles are 8x8, and serve as a tileset for building the image.
    * The first 0x2000 bytes are the tile building region.
    * A tile to have two pixels defined in one byte of space.
    * A tile takes up 64 pixels, 32 bytes, 0x20 chunks.
    * The last tile is actually the colors used in the image (16bit).
    * Only 16 colors can be used for the entire image.
    * 255 tiles may be chosen from, as (0x2000-(0x20))/0x20 = 0xFF
    * The last 0x600 bytes are the tiles used.
    * 256/8 = 32, 192/8 = 24
    * 32 * 24 = 0x300
    * The tiles are chosen based on the 16bit index of the tile.
    * 0x300 * 2 = 0x600!
    *
    * CGearBackgrounds tilemap (when stored on BW) employs some obfuscation.
    * BW obfuscates by adding 0xA0A0.
    * The obfuscated number is then tweaked by adding 15*(i/17)
    * To reverse, use a similar reverse calculation
    * PSK files are basically raw game rips (obfuscated)
    * CGB files are un-obfuscated / B2W2.
    * Due to BW and B2W2 using different obfuscation adds, PSK files are incompatible between the versions.
    */

    public CGearBackground(byte[] data)
    {
        if (data.Length != SIZE_CGB)
            throw new ArgumentOutOfRangeException(nameof(data));

        // decode for easy handling
        if (!IsCGB(data))
        {
            _psk = data;
            data = PSKtoCGB(data);
        }
        else
        {
            _cgb = data;
        }

        var Region1 = data.AsSpan(0, 0x1FE0);
        var ColorData = data.Slice(0x1FE0, 0x20);
        var Region2 = data.Slice(0x2000, 0x600);

        ColorPalette = new int[ColorCount];
        for (int i = 0; i < ColorPalette.Length; i++)
            ColorPalette[i] = GetRGB555_16(ReadUInt16LittleEndian(ColorData.AsSpan(i * 2)));

        Tiles = new Tile[0xFF];
        for (int i = 0; i < Tiles.Length; i++)
        {
            byte[] tiledata = Region1.Slice(i * Tile.SIZE_TILE, Tile.SIZE_TILE).ToArray();
            Tiles[i] = new Tile(tiledata);
            Tiles[i].SetTile(ColorPalette);
        }

        Map = new TileMap(Region2);
    }

    public readonly int[] ColorPalette;
    public readonly Tile[] Tiles;
    public readonly TileMap Map;

    // Track the original data
    private readonly byte[]? _cgb;
    private readonly byte[]? _psk;

    /// <summary>
    /// Writes the <see cref="CGearBackground"/> data to a binary form.
    /// </summary>
    /// <param name="B2W2">True if the destination game is <see cref="GameVersion.B2W2"/>, otherwise false for <see cref="GameVersion.BW"/>.</param>
    /// <returns>Serialized skin data for writing to the save file</returns>
    public byte[] GetSkin(bool B2W2) => B2W2 ? GetCGB() : GetPSK();

    private byte[] GetCGB() => _cgb ?? Write();
    private byte[] GetPSK() => _psk ?? CGBtoPSK(Write());

    private byte[] Write()
    {
        byte[] data = new byte[SIZE_CGB];
        for (int i = 0; i < Tiles.Length; i++)
            Array.Copy(Tiles[i].Write(), 0, data, i*Tile.SIZE_TILE, Tile.SIZE_TILE);

        for (int i = 0; i < ColorPalette.Length; i++)
        {
            var value = GetRGB555(ColorPalette[i]);
            var span = data.AsSpan(0x1FE0 + (i * 2));
            WriteUInt16LittleEndian(span, value);
        }

        Array.Copy(Map.Write(), 0, data, 0x2000, 0x600);

        return data;
    }

    private static bool IsCGB(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE_CGB)
            return false;

        // check odd bytes for anything not rotation flag
        for (int i = 0x2000; i < 0x2600; i += 2)
        {
            if ((data[i + 1] & ~0b1100) != 0)
                return false;
        }
        return true;
    }

    private static byte[] CGBtoPSK(ReadOnlySpan<byte> cgb)
    {
        byte[] psk = cgb.ToArray();
        for (int i = 0x2000; i < 0x2600; i += 2)
        {
            var span = psk.AsSpan(i);
            var tileVal = ReadUInt16LittleEndian(span);
            int val = GetPSKValue(tileVal);
            WriteUInt16LittleEndian(span, (ushort)val);
        }
        return psk;
    }

    private static int GetPSKValue(ushort val)
    {
        int rot = val & 0xFF00;
        int tile = val & 0x00FF;
        if (tile == 0xFF) // invalid tile?
            tile = 0;

        return tile + (15 * (tile / 17)) + 0xA0A0 + rot;
    }

    private static byte[] PSKtoCGB(ReadOnlySpan<byte> psk)
    {
        byte[] cgb = psk.ToArray();
        var tileRegion = cgb.AsSpan(0x2000, 0x600);
        ConvertTilesPSKtoCGB(tileRegion);
        return cgb;
    }

    private static void ConvertTilesPSKtoCGB(Span<byte> tileRegion)
    {
        for (int i = 0; i < tileRegion.Length; i += 2)
        {
            var exist = tileRegion.Slice(i, 2);
            var value = ReadUInt16LittleEndian(exist);
            var index = ValToIndex(value);

            byte tile = (byte)index;
            byte rot = (byte)(index >> 8);
            if (tile == 0xFF)
                tile = 0;
            exist[0] = tile;
            exist[1] = rot;
        }
    }

    private static int ValToIndex(ushort val)
    {
        var trunc = (val & 0x3FF);
        if (trunc is < 0xA0 or > 0x280)
            return (val & 0x5C00) | 0xFF;
        return ((val % 0x20) + (17 * ((trunc - 0xA0) / 0x20))) | (val & 0x5C00);
    }

    private static byte Convert8to5(int colorval)
    {
        byte i = 0;
        while (colorval > Convert5To8[i]) i++;
        return i;
    }

    private static int GetRGB555_32(int val)
    {
        var R = (val >> 00) & 0xFF;
        var G = (val >> 08) & 0xFF;
        var B = (val >> 16) & 0xFF;
        return (0xFF << 24) | (B << 16) | (G << 8) | R;
    }

    private static int GetRGB555_16(ushort val)
    {
        int R = (val >> 0) & 0x1F;
        int G = (val >> 5) & 0x1F;
        int B = (val >> 10) & 0x1F;

        R = Convert5To8[R];
        G = Convert5To8[G];
        B = Convert5To8[B];

        return (0xFF << 24) | (R << 16) | (G << 8) | B;
    }

    private static ushort GetRGB555(int v)
    {
        var R = (byte)(v >> 16);
        var G = (byte)(v >> 8);
        var B = (byte)(v >> 0);

        int val = 0;
        val |= Convert8to5(R) << 0;
        val |= Convert8to5(G) << 5;
        val |= Convert8to5(B) << 10;
        return (ushort)val;
    }

    private static ReadOnlySpan<byte> Convert5To8 => new byte[] // 0x20 entries
    {
        0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
        0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
        0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
        0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF,
    };

    /// <summary>
    /// Creates a new C-Gear Background object from an input image data byte array, with 32 bits per pixel.
    /// </summary>
    /// <param name="data">Image data</param>
    /// <returns>new C-Gear Background object</returns>
    public static CGearBackground GetBackground(ReadOnlySpan<byte> data)
    {
        const int bpp = 4;
        if (Width * Height * bpp != data.Length)
            throw new ArgumentException("Invalid image data size.");

        var pixels = MemoryMarshal.Cast<byte, int>(data);
        var colors = GetColorData(pixels);

        var Palette = colors.Distinct().ToArray();
        if (Palette.Length > ColorCount)
            throw new ArgumentException($"Too many unique colors. Expected <= 16, got {Palette.Length}");

        var tiles = GetTiles(colors, Palette);
        GetTileList(tiles, out List<Tile> tilelist, out TileMap tm);
        if (tilelist.Count >= 0xFF)
            throw new ArgumentException($"Too many unique tiles. Expected < 256, received {tilelist.Count}.");

        // Finished!
        return new CGearBackground(Palette, tilelist.ToArray(), tm);
    }

    private static int[] GetColorData(ReadOnlySpan<int> pixels)
    {
        int[] colors = new int[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
            colors[i] = GetRGB555_32(pixels[i]);
        return colors;
    }

    private static Tile[] GetTiles(ReadOnlySpan<int> colors, ReadOnlySpan<int> palette)
    {
        var tiles = new Tile[TileCount];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = GetTile(colors, palette, i);
        return tiles;
    }

    private static Tile GetTile(ReadOnlySpan<int> colors, ReadOnlySpan<int> palette, int tileIndex)
    {
        int x = (tileIndex * 8) % Width;
        int y = 8 * ((tileIndex * 8) / Width);

        var t = new Tile();
        for (uint ix = 0; ix < 8; ix++)
        {
            for (uint iy = 0; iy < 8; iy++)
            {
                int index = ((int) (y + iy) * Width) + (int) (x + ix);
                int c = colors[index];

                t.ColorChoices[(ix % 8) + (iy * 8)] = palette.IndexOf(c);
            }
        }

        t.SetTile(palette);
        return t;
    }

    private static void GetTileList(ReadOnlySpan<Tile> tiles, out List<Tile> tilelist, out TileMap tm)
    {
        tilelist = new List<Tile> { tiles[0] };
        tm = new TileMap(new byte[2 * Width * Height / 64]);

        // start at 1 as the 0th tile is always non-duplicate
        for (int i = 1; i < tm.TileChoices.Length; i++)
            FindPossibleRotatedTile(tiles[i], tilelist, tm, i);
    }

    private static void FindPossibleRotatedTile(Tile t, IList<Tile> tilelist, TileMap tm, int tileIndex)
    {
        // Test all tiles currently in the list
        for (int j = 0; j < tilelist.Count; j++)
        {
            int rotVal = t.GetRotationValue(tilelist[j].ColorChoices);
            if (rotVal <= -1)
                continue;
            tm.TileChoices[tileIndex] = j;
            tm.Rotations[tileIndex] = rotVal;
            return;
        }

        // No tile found, add to list
        tilelist.Add(t);
        tm.TileChoices[tileIndex] = tilelist.Count - 1;
        tm.Rotations[tileIndex] = 0;
    }

    private CGearBackground(int[] Palette, Tile[] tilelist, TileMap tm)
    {
        Map = tm;
        ColorPalette = Palette;
        Tiles = tilelist;
    }

    public byte[] GetImageData()
    {
        byte[] data = new byte[4 * Width * Height];
        for (int i = 0; i < Map.TileChoices.Length; i++)
        {
            int x = (i * 8) % Width;
            int y = 8 * ((i * 8) / Width);
            var choice = Map.TileChoices[i] % (Tiles.Length + 1);
            var tile = Tiles[choice];
            var tileData = tile.Rotate(Map.Rotations[i]);
            for (int iy = 0; iy < 8; iy++)
            {
                int src = iy * (4 * 8);
                int dest = (((y+iy) * Width) + x) * 4;
                Array.Copy(tileData, src, data, dest, 4*8);
            }
        }
        return data;
    }
}

public sealed class Tile
{
    internal const int SIZE_TILE = 0x20;
    private const int TileWidth = 8;
    private const int TileHeight = 8;
    internal readonly int[] ColorChoices;
    private byte[] PixelData;
    private byte[]? PixelDataX;
    private byte[]? PixelDataY;

    internal Tile() : this(new byte[SIZE_TILE]) { }

    internal Tile(byte[] data)
    {
        if (data.Length != SIZE_TILE)
            throw new ArgumentException(nameof(data));

        ColorChoices = new int[TileWidth * TileHeight];
        for (int i = 0; i < data.Length; i++)
        {
            var ofs = i * 2;
            ColorChoices[ofs + 0] = data[i] & 0xF;
            ColorChoices[ofs + 1] = data[i] >> 4;
        }
        PixelData = Array.Empty<byte>();
    }

    internal void SetTile(ReadOnlySpan<int> palette) => PixelData = GetTileData(palette);

    private byte[] GetTileData(ReadOnlySpan<int> Palette)
    {
        const int pixels = TileWidth * TileHeight;
        byte[] data = new byte[pixels * 4];
        for (int i = 0; i < pixels; i++)
        {
            var choice = ColorChoices[i];
            var value = Palette[choice];
            var span = data.AsSpan(4 * i, 4);
            WriteInt32LittleEndian(span, value);
        }
        return data;
    }

    internal byte[] Write()
    {
        byte[] data = new byte[SIZE_TILE];
        for (int i = 0; i < data.Length; i++)
        {
            var span = ColorChoices.AsSpan(i * 2, 2);
            data[i] = (byte)((span[0] & 0xF) | ((span[1] & 0xF) << 4));
        }
        return data;
    }

    public byte[] Rotate(int rotFlip)
    {
        if (rotFlip == 0)
            return PixelData;
        if ((rotFlip & 4) > 0)
            return PixelDataX ??= FlipX(PixelData, TileWidth);
        if ((rotFlip & 8) > 0)
            return PixelDataY ??= FlipY(PixelData, TileHeight);
        return PixelData;
    }

    private static byte[] FlipX(ReadOnlySpan<byte> data, int width, int bpp = 4)
    {
        byte[] result = new byte[data.Length];
        int pixels = data.Length / bpp;
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            x = width - x - 1; // flip x
            int dest = ((y * width) + x) * bpp;

            var o = 4 * i;
            result[dest + 0] = data[o + 0];
            result[dest + 1] = data[o + 1];
            result[dest + 2] = data[o + 2];
            result[dest + 3] = data[o + 3];
        }
        return result;
    }

    private static byte[] FlipY(ReadOnlySpan<byte> data, int height, int bpp = 4)
    {
        byte[] result = new byte[data.Length];
        int pixels = data.Length / bpp;
        int width = pixels / height;
        for (int i = 0; i < pixels; i++)
        {
            int x = i % width;
            int y = i / width;

            y = height - y - 1; // flip x
            int dest = ((y * width) + x) * bpp;

            var o = 4 * i;
            result[dest + 0] = data[o + 0];
            result[dest + 1] = data[o + 1];
            result[dest + 2] = data[o + 2];
            result[dest + 3] = data[o + 3];
        }
        return result;
    }

    internal int GetRotationValue(ReadOnlySpan<int> tileColors)
    {
        // Check all rotation types
        if (tileColors.SequenceEqual(ColorChoices))
            return 0;

        if (IsMirrorX(tileColors))
            return 4;
        if (IsMirrorY(tileColors))
            return 8;
        if (IsMirrorXY(tileColors))
            return 12;

        return -1;
    }

    private bool IsMirrorX(ReadOnlySpan<int> tileColors)
    {
        for (int i = 0; i < 64; i++)
        {
            if (ColorChoices[(7 - (i & 7)) + (8 * (i / 8))] != tileColors[i])
                return false;
        }

        return true;
    }

    private bool IsMirrorY(ReadOnlySpan<int> tileColors)
    {
        for (int i = 0; i < 64; i++)
        {
            if (ColorChoices[64 - (8 * (1 + (i / 8))) + (i & 7)] != tileColors[i])
                return false;
        }

        return true;
    }

    private bool IsMirrorXY(ReadOnlySpan<int> tileColors)
    {
        for (int i = 0; i < 64; i++)
        {
            if (ColorChoices[63 - i] != tileColors[i])
                return false;
        }

        return true;
    }
}

public sealed class TileMap
{
    public readonly int[] TileChoices;
    public readonly int[] Rotations;

    internal TileMap(byte[] data)
    {
        TileChoices = new int[data.Length / 2];
        Rotations = new int[data.Length / 2];
        for (int i = 0; i < data.Length; i += 2)
        {
            TileChoices[i / 2] = data[i];
            Rotations[i / 2] = data[i + 1];
        }
    }

    internal byte[] Write()
    {
        byte[] data = new byte[TileChoices.Length * 2];
        for (int i = 0; i < data.Length; i += 2)
        {
            data[i] = (byte)TileChoices[i / 2];
            data[i + 1] = (byte)Rotations[i / 2];
        }
        return data;
    }
}
