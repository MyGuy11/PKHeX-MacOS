using System;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for Generation 5 games.
/// </summary>
public sealed class Zukan5 : Zukan<SAV5>
{
    protected override int OFS_SEEN => OFS_CAUGHT + BitSeenSize;
    protected override int OFS_CAUGHT => 0x8;
    protected override int BitSeenSize => 0x54;
    protected override int DexLangFlagByteCount => 7;
    protected override int DexLangIDCount => 7;

    public Zukan5(SAV5B2W2 sav, int dex, int langflag) : base(sav, dex, langflag)
    {
        DexFormIndexFetcher = DexFormUtil.GetDexFormIndexB2W2;
    }

    public Zukan5(SAV5BW sav, int dex, int langflag) : base(sav, dex, langflag)
    {
        DexFormIndexFetcher = DexFormUtil.GetDexFormIndexBW;
    }

    public readonly Func<ushort, byte, int> DexFormIndexFetcher;

    protected override int GetDexLangFlag(int lang)
    {
        lang--;
        if (lang > 5)
            lang--; // 0-6 language values
        if ((uint)lang > 5)
            return -1;
        return lang;
    }

    protected override bool GetSaneFormsToIterate(ushort species, out int formStart, out int formEnd, int formIn)
    {
        formStart = 0;
        formEnd = 0;
        return false;
    }

    protected override void SetSpindaDexData(PKM pk, bool alreadySeen)
    {
    }

    protected override void SetAllDexFlagsLanguage(int bit, int lang, bool value = true)
    {
        lang = GetDexLangFlag(lang);
        if (lang < 0)
            return;

        // Set the Language
        int lbit = (bit * DexLangIDCount) + lang;
        if (bit < 493) // shifted by 1, Gen5 species do not have international language bits
            SetFlag(PokeDexLanguageFlags, lbit, value);
    }

    protected override void SetAllDexSeenFlags(int baseBit, byte form, int gender, bool isShiny, bool value = true)
    {
        var shiny = isShiny ? 1 : 0;
        SetDexFlags(baseBit, baseBit, gender, shiny);
        SetFormFlags((ushort)(baseBit + 1), form, shiny, value);
    }

    public override void SetDex(PKM pk)
    {
        if (pk.Species is 0 or > Legal.MaxSpeciesID_5)
            return;
        if (pk.IsEgg) // do not add
            return;

        int bit = pk.Species - 1;
        SetCaughtFlag(bit);

        // Set the [Species/Gender/Shiny] Seen Flag
        SetAllDexSeenFlags(bit, pk.Form, pk.Gender, pk.IsShiny);
        SetAllDexFlagsLanguage(bit, pk.Language);
        SetFormFlags(pk);
    }

    private void SetCaughtFlag(int bit) => SetFlag(OFS_CAUGHT, bit);

    protected override void SetDisplayedFlag(int baseBit, int formBit, bool value, int shift)
    {
        if (!value)
        {
            SetDisplayed(baseBit, shift, false);
            return;
        }

        bool displayed = GetIsSpeciesAnyDisplayed(baseBit);
        if (displayed)
            return; // no need to set another bit

        SetDisplayed(baseBit, shift, true);
    }

    private bool GetIsSpeciesAnyDisplayed(int baseBit)
    {
        // Check Displayed Status for base form
        for (int i = 0; i < 4; i++)
        {
            if (GetDisplayed(baseBit, i))
                return true;
        }
        return false;
    }

    private int FormLen => SAV is SAV5B2W2 ? 0xB : 0x9;
    private int FormDex => 0x8 + (BitSeenSize * 9);

    private void SetFormFlags(PKM pk)
    {
        var species = pk.Species;
        var form = pk.Form;
        var shiny = pk.IsShiny ? 1 : 0;
        SetFormFlags(species, form, shiny);
    }

    private void SetFormFlags(ushort species, byte form, int shiny, bool value = true)
    {
        var fc = SAV.Personal[species].FormCount;
        int f = DexFormIndexFetcher(species, fc);
        if (f < 0)
            return;

        var bit = f + form;

        // Set Form Seen Flag
        SetFormFlag(bit, shiny, value);

        // Set Displayed Flag if necessary, check all flags
        if (!value || !GetIsFormDisplayed(f, fc))
            SetFormFlag(bit, 2 + shiny, value);
    }

    public bool GetFormFlag(int formIndex, int flagRegion) => GetFlag(FormDex + (FormLen * flagRegion), formIndex);
    public void SetFormFlag(int formIndex, int flagRegion, bool value = true) => SetFlag(FormDex + (FormLen * flagRegion), formIndex, value);

    private bool GetIsFormDisplayed(int f, byte formCount)
    {
        for (byte i = 0; i < formCount; i++)
        {
            var index = f + i;
            if (GetFormFlag(index, 2)) // Nonshiny
                return true; // already set
            if (GetFormFlag(index, 3)) // Shiny
                return true; // already set
        }
        return false;
    }

    public bool[] GetLanguageBitflags(ushort species)
    {
        var result = new bool[DexLangIDCount];
        int bit = species - 1;
        for (int i = 0; i < DexLangIDCount; i++)
        {
            int lbit = (bit * DexLangIDCount) + i;
            result[i] = GetFlag(PokeDexLanguageFlags, lbit);
        }
        return result;
    }

    public void SetLanguageBitflags(ushort species, ReadOnlySpan<bool> value)
    {
        int bit = species - 1;
        for (int i = 0; i < DexLangIDCount; i++)
        {
            int lbit = (bit * DexLangIDCount) + i;
            SetFlag(PokeDexLanguageFlags, lbit, value[i]);
        }
    }

    public void ToggleLanguageFlagsAll(bool value)
    {
        var arr = GetBlankLanguageBits(value);
        for (ushort i = 1; i <= SAV.MaxSpeciesID; i++)
            SetLanguageBitflags(i, arr);
    }

    public void ToggleLanguageFlagsSingle(ushort species, bool value)
    {
        var arr = GetBlankLanguageBits(value);
        SetLanguageBitflags(species, arr);
    }

    private bool[] GetBlankLanguageBits(bool value)
    {
        var result = new bool[DexLangIDCount];
        for (int i = 0; i < DexLangIDCount; i++)
            result[i] = value;
        return result;
    }
}
