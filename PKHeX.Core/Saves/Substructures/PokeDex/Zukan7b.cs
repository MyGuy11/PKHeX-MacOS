using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.GG"/> games, slightly modified from <see cref="Zukan7"/>.
/// </summary>>
public sealed class Zukan7b : Zukan7
{
    public Zukan7b(SAV7b sav, int dex, int langflag) : base(sav, dex, langflag)
    {
    }

    public override void SetDex(PKM pk)
    {
        if (!TryGetSizeEntryIndex(pk.Species, pk.Form, out _))
            return;
        SetSizeData((PB7)pk);
        base.SetDex(pk);
    }

    protected override void SetDex(ushort species, int bit, byte form, int gender, bool shiny, int lang)
    {
        if (IsBuddy(species, form))
            form = 0;
        base.SetDex(species, bit, form, gender, shiny, lang);
    }

    private static bool IsBuddy(ushort species, byte form) => (species == (int)Species.Pikachu && form == 8) || (species == (int)Species.Eevee && form == 1);

    public const byte DefaultEntryValue = 0x7F;

    public bool GetSizeData(DexSizeType group, ushort species, byte form, out byte height, out byte weight)
    {
        height = weight = DefaultEntryValue;
        if (TryGetSizeEntryIndex(species, form, out var index))
            return GetSizeData(group, index, out height, out weight);
        return false;
    }

    public bool GetSizeData(DexSizeType group, int index, out byte height, out byte weight)
    {
        var ofs = GetDexSizeOffset(group, index);
        var entry = SAV.Data.AsSpan(ofs);
        height = entry[1];
        weight = entry[2];
        return !IsEntryUnset(height, weight);
    }

    private static bool IsEntryUnset(byte height, byte weight) => height == DefaultEntryValue && weight == DefaultEntryValue;

    private void SetSizeData(PB7 pk)
    {
        var species = pk.Species;
        var form = pk.Form;
        if (!TryGetSizeEntryIndex(species, form, out int index))
            return;

        if (Math.Round(pk.HeightAbsolute) < pk.PersonalInfo.Height) // possible minimum height
        {
            int ofs = GetDexSizeOffset(DexSizeType.MinHeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var minHeight = entry[1];
            var calcHeight = PB7.GetHeightAbsolute(pk.PersonalInfo, minHeight);
            if (Math.Round(pk.HeightAbsolute) < Math.Round(calcHeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MinHeight);
        }
        else if (Math.Round(pk.HeightAbsolute) > pk.PersonalInfo.Height) // possible maximum height
        {
            int ofs = GetDexSizeOffset(DexSizeType.MaxHeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var maxHeight = entry[1];
            var calcHeight = PB7.GetHeightAbsolute(pk.PersonalInfo, maxHeight);
            if (Math.Round(pk.HeightAbsolute) > Math.Round(calcHeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MaxHeight);
        }

        if (Math.Round(pk.WeightAbsolute) < pk.PersonalInfo.Weight) // possible minimum weight
        {
            int ofs = GetDexSizeOffset(DexSizeType.MinWeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var minHeight = entry[1];
            var minWeight = entry[2];
            var calcWeight = PB7.GetWeightAbsolute(pk.PersonalInfo, minHeight, minWeight);
            if (Math.Round(pk.WeightAbsolute) < Math.Round(calcWeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MinWeight);
        }
        else if (Math.Round(pk.WeightAbsolute) > pk.PersonalInfo.Weight) // possible maximum weight
        {
            int ofs = GetDexSizeOffset(DexSizeType.MaxWeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var maxHeight = entry[1];
            var maxWeight = entry[2];
            var calcWeight = PB7.GetWeightAbsolute(pk.PersonalInfo, maxHeight, maxWeight);
            if (Math.Round(pk.WeightAbsolute) > Math.Round(calcWeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MaxWeight);
        }
    }

    private static int GetDexSizeOffset(DexSizeType group, int index) => 0x3978 + (index * 6) + ((int)group * 0x45C); // blockofs + 0xF78 + ([186*6]*n) + x*6

    private void SetSizeData(PB7 pk, DexSizeType group)
    {
        var tree = EvolutionTree.Evolves7b;
        ushort species = pk.Species;
        var form = pk.Form;

        byte height = pk.HeightScalar;
        byte weight = pk.WeightScalar;

        // update for all species in potential lineage
        var allspec = tree.GetEvolutionsAndPreEvolutions(species, form);
        foreach (var (s, f) in allspec)
            SetSizeData(group, s, f, height, weight);
    }

    public void SetSizeData(DexSizeType group, ushort species, byte form, byte height, byte weight)
    {
        if (TryGetSizeEntryIndex(species, form, out var index))
            SetSizeData(group, index, height, weight);
    }

    public void SetSizeData(DexSizeType group, int index, byte height, byte weight)
    {
        var ofs = GetDexSizeOffset(group, index);
        var span = SAV.Data.AsSpan(ofs);
        span[0] = 0;
        span[1] = height;
        span[2] = weight;
        span[3] = 0;
    }

    private const int EntryMeltan = 151; // Melmetal 152
    private const int EntryForms = 153;

    public static bool TryGetSizeEntryIndex(ushort species, byte form, out int index)
    {
        index = -1;
        if (form == 0)
        {
            if (species <= 151) // Mew
            {
                index = species - 1;
                return true;
            }
            if (species is 808 or 809) // Meltan & Melmetal
            {
                index = EntryMeltan + (species - 808);
                return true;
            }
            return false;
        }

        // Forms
        for (int i = 0; i < SizeDexInfoTable.Length; i += 2)
        {
            if (SizeDexInfoTable[i] != species)
                continue;
            if (SizeDexInfoTable[i + 1] != form)
                continue;
            index = EntryForms + (i >> 1);
            return true;
        }
        return false;
    }

    private static ReadOnlySpan<byte> SizeDexInfoTable => new byte[]
    {
        // species, form
        003, 1,
        006, 1,
        006, 2,
        009, 1,
        015, 1,
        018, 1,
        019, 1,
        020, 1,
        026, 1,
        027, 1,
        028, 1,
        037, 1,
        038, 1,
        050, 1,
        051, 1,
        052, 1,
        053, 1,
        065, 1,
        074, 1,
        075, 1,
        076, 1,
        080, 1,
        088, 1,
        089, 1,
        094, 1,
        103, 1,
        105, 1,
        115, 1,
        127, 1,
        130, 1,
        142, 1,
        150, 1,
        150, 2,
    };

    protected override bool GetSaneFormsToIterate(ushort species, out int formStart, out int formEnd, int formIn)
    {
        switch (species)
        {
            // Totems with Alolan Forms
            case 020 or 105: // Raticate or Marowak
                formStart = 0;
                formEnd = 1;
                return true;

            default:
                int count = DexFormUtil.GetDexFormCountGG(species);
                formStart = formEnd = 0;
                return count < formIn;
        }
    }
}

public enum DexSizeType
{
    MinHeight,
    MaxHeight,
    MinWeight,
    MaxWeight,
}
