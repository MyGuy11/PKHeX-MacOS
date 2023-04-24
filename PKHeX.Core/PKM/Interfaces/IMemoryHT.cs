﻿namespace PKHeX.Core;

/// <summary>
/// Exposes memory details for the Handling Trainer (not OT).
/// </summary>
public interface IMemoryHT
{
    byte HT_Memory { get; set; }
    ushort HT_TextVar { get; set; }
    byte HT_Feeling { get; set; }
    byte HT_Intensity { get; set; }
}

public static partial class Extensions
{
    /// <summary>
    /// Sets a Link Trade memory to the <see cref="ht"/>.
    /// </summary>
    public static void SetTradeMemoryHT6(this IMemoryHT ht, bool bank)
    {
        ht.HT_Memory = 4; // Link trade to [VAR: General Location]
        ht.HT_TextVar = bank ? (byte)0 : (byte)9; // Somewhere (Bank) : Pokécenter (Trade)
        ht.HT_Intensity = 1;
        ht.HT_Feeling = MemoryContext6.GetRandomFeeling6(4, bank ? 10 : 20); // 0-9 Bank, 0-19 Trade
    }

    /// <summary>
    /// Sets a Link Trade memory to the <see cref="ht"/>.
    /// </summary>
    public static void SetTradeMemoryHT8(this IMemoryHT ht)
    {
        ht.HT_Memory = 4; // Link trade to [VAR: General Location]
        ht.HT_TextVar = 9; // Pokécenter (Trade)
        ht.HT_Intensity = 1;
        ht.HT_Feeling = MemoryContext8.GetRandomFeeling8(4, 20); // 0-19 Trade
    }

    /// <summary>
    /// Sets all values to zero.
    /// </summary>
    public static void ClearMemoriesHT(this IMemoryHT ht)
    {
        ht.HT_TextVar = ht.HT_Memory = ht.HT_Feeling = ht.HT_Intensity = 0;
    }
}
