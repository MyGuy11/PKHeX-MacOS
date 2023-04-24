using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for MYSTRY Mew origins.
/// </summary>
public static class MystryMew
{
    private static ReadOnlySpan<ushort> Seeds => new ushort[]
    {
        0x0652, 0x0932, 0x0C13, 0x0D43, 0x0EEE,
        0x1263, 0x13C9, 0x1614, 0x1C09, 0x1EA5,
        0x20BF, 0x2389, 0x2939, 0x302D, 0x306E,
        0x34F3, 0x45F3, 0x46CE, 0x4A0D, 0x4B63,

        0x4C79, 0x508E, 0x50AB, 0x5240, 0x5327,
        0x56BA, 0x56CC, 0x5841, 0x5A60, 0x5BC1,
        0x5E2B, 0x5EF3, 0x6065, 0x643F, 0x6457,
        0x67A3, 0x6944, 0x6E06, 0x6E62, 0x7667,

        0x77EF, 0x78D2, 0x8655, 0x8A92, 0x8B48,
        0x93D0, 0x941D, 0x95A0, 0x967D, 0x9690,
        0x9C37, 0x9C40, 0x9D9C, 0x9DE4, 0x9E86,
        0xA153, 0xA443, 0xA8AC, 0xAC08, 0xAFFB,

        0xB1F2, 0xB831, 0xBE96, 0xC2D4, 0xC385,
        0xC6CE, 0xC92C, 0xC953, 0xC962, 0xCC43,
        0xCD47, 0xCD96, 0xD1E4, 0xDFED, 0xE62C,
        0xE6CC, 0xE90A, 0xE95D, 0xE991, 0xEBB2,

        0xEE7F, 0xEE9F, 0xEFC8, 0xF0E4, 0xFE4E,
        0xFE9D,
    };

  //private const int FramesPerMew = 5;
    private const int MewPerRestrictedSeed = 5;

    /// <summary>
    /// Gets a random valid seed based on the input <see cref="random"/> value.
    /// </summary>
    public static uint GetSeed(uint random, PIDType type = PIDType.BACD_U)
    {
        var seeds = Seeds;
        uint restricted = random % (uint)seeds.Length;
        var seed = (uint)seeds[(int)restricted];
        if (type == PIDType.BACD_R)
            return seed;

        uint position = (random % (MewPerRestrictedSeed - 1)) + 1;
        for (int i = 0; i < position; i++)
            seed = LCRNG.Next5(seed);

        return seed;
    }

    /// <summary>
    /// Checks if the seed is a known seed.
    /// </summary>
    /// <param name="seed">Origin seed (for the PID/IV)</param>
    public static int GetSeedIndex(uint seed)
    {
        var seeds = Seeds;
        for (int i = 0; i < 5; i++)
        {
            if (seed <= ushort.MaxValue)
                return seeds.BinarySearch((ushort)seed);
            seed = LCRNG.Prev5(seed);
        }

        return -1;
    }
}
