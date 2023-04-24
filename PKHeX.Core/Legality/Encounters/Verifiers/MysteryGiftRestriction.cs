using System;

namespace PKHeX.Core;

/// <summary>
/// Restriction Flags for receiving 3DS/NDS era events.
/// </summary>
[Flags]
public enum MysteryGiftRestriction
{
    None = 0,
    LangJapanese = 1 << LanguageID.Japanese,
    LangEnglish = 1 << LanguageID.English,
    LangFrench = 1 << LanguageID.French,
    LangItalian = 1 << LanguageID.Italian,
    LangGerman = 1 << LanguageID.German,
    LangSpanish = 1 << LanguageID.Spanish,
    LangKorean = 1 << LanguageID.Korean,

    RegionBase = LangKorean,
    LangRestrict = LangJapanese | LangEnglish | LangFrench | LangItalian | LangGerman | LangSpanish | LangKorean,

    RegJP = RegionBase << Region3DSIndex.Japan,
    RegNA = RegionBase << Region3DSIndex.NorthAmerica,
    RegEU = RegionBase << Region3DSIndex.Europe,
    RegZH = RegionBase << Region3DSIndex.China,
    RegKO = RegionBase << Region3DSIndex.Korea,
    RegTW = RegionBase << Region3DSIndex.Taiwan,

    RegionRestrict = RegJP | RegNA | RegEU | RegZH | RegKO | RegTW,

    OTReplacedOnTrade = RegTW << 1,
}

/// <summary>
/// Extension methods for <see cref="MysteryGiftRestriction"/>.
/// </summary>
public static class MysteryGiftRestrictionExtensions
{
    /// <summary>
    /// Checks the flags to pick out a language that can receive the gift.
    /// </summary>
    /// <param name="value">Flag value</param>
    /// <returns>Language ID; -1 if none</returns>
    public static int GetSuggestedLanguage(this MysteryGiftRestriction value)
    {
        for (int i = (int)LanguageID.Japanese; i <= (int)LanguageID.Korean; i++)
        {
            if (value.HasFlag((MysteryGiftRestriction)(1 << i)))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Finds the lowest index of a region that can receive the gift.
    /// </summary>
    /// <param name="value">Flag value</param>
    /// <returns>Region ID; -1 if none</returns>
    public static int GetSuggestedRegion(this MysteryGiftRestriction value)
    {
        for (int i = (int)Region3DSIndex.Japan; i <= (int)Region3DSIndex.Taiwan; i++)
        {
            if (value.HasFlag((MysteryGiftRestriction)((int)MysteryGiftRestriction.RegionBase << i)))
                return i;
        }
        return -1;
    }
}
