using System;
using System.Collections.Generic;
using static PKHeX.Core.EncounterServerDateCheck;

namespace PKHeX.Core;

/// <summary>
/// Logic to check if a date obtained is within the date of availability.
/// </summary>
public static class EncounterServerDate
{
    private static bool IsValidDate(DateOnly obtained, DateOnly start) => obtained >= start && obtained <= DateOnly.FromDateTime(DateTime.UtcNow);

    private static bool IsValidDate(DateOnly obtained, DateOnly start, DateOnly end) => obtained >= start && obtained <= end;

    private static bool IsValidDate(DateOnly obtained, (DateOnly Start, DateOnly? End) value)
    {
        var (start, end) = value;
        if (end is not { } x)
            return IsValidDate(obtained, start);
        return IsValidDate(obtained, start, x);
    }

    private static EncounterServerDateCheck Result(bool result) => result ? Valid : Invalid;

    public static EncounterServerDateCheck IsValidDate(this IEncounterServerDate enc, DateOnly obtained) => enc switch
    {
        WC8 wc8 => Result(IsValidDateWC8(wc8.CardID, obtained)),
        WA8 wa8 => Result(IsValidDateWA8(wa8.CardID, obtained)),
        WB8 wb8 => Result(IsValidDateWB8(wb8.CardID, obtained)),
        WC9 wc9 => Result(IsValidDateWC9(wc9, obtained)),
        _ => throw new ArgumentOutOfRangeException(nameof(enc)),
    };

    public static bool IsValidDateWC8(int cardID, DateOnly obtained) => WC8Gifts.TryGetValue(cardID, out var time) && IsValidDate(obtained, time);
    public static bool IsValidDateWA8(int cardID, DateOnly obtained) => WA8Gifts.TryGetValue(cardID, out var time) && IsValidDate(obtained, time);
    public static bool IsValidDateWB8(int cardID, DateOnly obtained) => WB8Gifts.TryGetValue(cardID, out var time) && IsValidDate(obtained, time);

    public static bool IsValidDateWC9(WC9 card  , DateOnly obtained) => (WC9Gifts.TryGetValue(card.CardID, out var time)
                                                                      || WC9GiftsChk.TryGetValue(card.Checksum, out time)) && IsValidDate(obtained, time);

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, DateOnly> WC8Gifts = new()
    {
        {9000, new(2020, 02, 12)}, // Bulbasaur
        {9001, new(2020, 02, 12)}, // Charmander
        {9002, new(2020, 02, 12)}, // Squirtle
        {9003, new(2020, 02, 12)}, // Pikachu
        {9004, new(2020, 02, 15)}, // Original Color Magearna
        {9005, new(2020, 02, 12)}, // Eevee
        {9006, new(2020, 02, 12)}, // Rotom
        {9007, new(2020, 02, 12)}, // Pichu
        {9008, new(2020, 06, 02)}, // Hidden Ability Grookey
        {9009, new(2020, 06, 02)}, // Hidden Ability Scorbunny
        {9010, new(2020, 06, 02)}, // Hidden Ability Sobble
        {9011, new(2020, 06, 30)}, // Shiny Zeraora
        {9012, new(2020, 11, 10)}, // Gigantamax Melmetal
        {9013, new(2021, 06, 17)}, // Gigantamax Bulbasaur
        {9014, new(2021, 06, 17)}, // Gigantamax Squirtle
    };

    private static readonly DateOnly? Never = null;

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WA8Gifts = new()
    {
        {0138, (new(2022, 01, 27), new(2023, 02, 01))}, // Poké Center Happiny
        {0301, (new(2022, 02, 04), new(2023, 03, 01))}, // プロポチャ Piplup
        {0801, (new(2022, 02, 25), new(2022, 06, 01))}, // Teresa Roca Hisuian Growlithe
        {1201, (new(2022, 05, 31), new(2022, 08, 01))}, // 전이마을 Regigigas
        {1202, (new(2022, 05, 31), new(2022, 08, 01))}, // 빛나's Piplup
        {1203, (new(2022, 08, 18), new(2022, 11, 01))}, // Arceus Chronicles Hisuian Growlithe
        {0151, (new(2022, 09, 03), new(2022, 10, 01))}, // Otsukimi Festival 2022 Clefairy

        {9018, (new(2022, 05, 18), Never)}, // Hidden Ability Rowlet
        {9019, (new(2022, 05, 18), Never)}, // Hidden Ability Cyndaquil
        {9020, (new(2022, 05, 18), Never)}, // Hidden Ability Oshawott
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WB8Gifts = new()
    {
        {9015, (new(2022, 05, 18), Never)}, // Hidden Ability Turtwig
        {9016, (new(2022, 05, 18), Never)}, // Hidden Ability Chimchar
        {9017, (new(2022, 05, 18), Never)}, // Hidden Ability Piplup
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WC9GiftsChk = new()
    {
        {0xE5EB, (new(2022, 11, 17), new(2023, 02, 03))}, // Fly Pikachu - rev 1 (male 128 height/weight)
        {0x908B, (new(2023, 02, 02), new(2023, 03, 01))}, // Fly Pikachu - rev 2 (both 0 height/weight)
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WC9Gifts = new()
    {
        {0001, (new(2022, 11, 17), Never)}, // PokéCenter Flabébé
        {0006, (new(2022, 12, 16), new(2023, 02, 01))}, // Jump Festa Gyarados
        {0501, (new(2023, 02, 16), new(2023, 02, 21))}, // Jiseok's Garganacl
        {1513, (new(2023, 02, 27), new(2024, 03, 01))}, // Hisuian Zoroark DLC Purchase Gift
        {0502, (new(2023, 03, 31), new(2023, 07, 01))}, // TCG Flying Lechonk
    };
}
