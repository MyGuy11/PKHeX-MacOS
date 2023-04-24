using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;
using static PKHeX.Core.Encounters5DR;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Encounters
/// </summary>
public static class Encounters5B2W2
{
    internal static readonly EncounterArea5[] SlotsB2 = EncounterArea5.GetAreas(Get("b2", "52"), B2);
    internal static readonly EncounterArea5[] SlotsW2 = EncounterArea5.GetAreas(Get("w2", "52"), W2);

    static Encounters5B2W2() => MarkEncounterTradeStrings(TradeGift_B2W2_Regular, TradeB2W2);

    #region DreamWorld Encounter
    public static readonly EncounterStatic5[] DreamWorld_B2W2 = DreamWorldEntry.GetArray(B2W2, stackalloc DreamWorldEntry[]
    {
        // Pleasant Forest
        new(535, 10, 496, 414, 352), // Tympole
        new(546, 10, 073, 227, 388), // Cottonee
        new(548, 10, 079, 204, 230), // Petilil
        new(588, 10, 203, 224, 450), // Karrablast
        new(616, 10, 051, 226, 227), // Shelmet
        new(545, 30, 342, 390, 276), // Scolipede

        // Windswept Sky
        new(519, 10, 016, 095, 234), // Pidove
        new(561, 10, 095, 500, 257), // Sigilyph
        new(580, 10, 432, 362, 382), // Ducklett
        new(587, 10, 098, 403, 204), // Emolga

        // Sparkling Sea
        new(550, 10, 029, 097, 428, Form: 0), // Basculin-Red
        new(550, 10, 029, 097, 428, Form: 1), // Basculin-Blue
        new(594, 10, 392, 243, 220), // Alomomola
        new(618, 10, 189, 174, 281), // Stunfisk
        new(564, 10, 205, 175, 334), // Tirtouga

        // Spooky Manor
        new(605, 10, 377, 112, 417), // Elgyem
        new(624, 10, 210, 427, 389), // Pawniard
        new(596, 36, 486, 050, 228), // Galvantula
        new(578, 32, 105, 286, 271), // Duosion
        new(622, 10, 205, 007, 009), // Golett

        // Rugged Mountain
        new(631, 10, 510, 257, 202), // Heatmor
        new(632, 10, 210, 203, 422), // Durant
        new(556, 10, 042, 073, 191), // Maractus
        new(558, 34, 157, 068, 400), // Crustle
        new(553, 40, 242, 068, 212), // Krookodile

        // Icy Cave
        new(529, 10, 229, 319, 431), // Drilbur
        new(621, 10, 044, 424, 389), // Druddigon
        new(525, 25, 479, 174, 484), // Boldore
        new(583, 35, 429, 420, 286), // Vanillish
        new(600, 38, 451, 356, 393), // Klang
        new(610, 10, 082, 068, 400), // Axew

        // Dream Park
        new(531, 10, 270, 227, 281), // Audino
        new(538, 10, 020, 008, 276), // Throh
        new(539, 10, 249, 009, 530), // Sawk
        new(559, 10, 067, 252, 409), // Scraggy
        new(533, 25, 067, 183, 409), // Gurdurr

        // PGL
        new(575, 32, 243, Gender: 0), // Gothorita
        new(025, 10, 029, Gender: 0), // Pikachu
        new(511, 10, 437, Gender: 0), // Pansage
        new(513, 10, 257, Gender: 0), // Pansear
        new(515, 10, 056, Gender: 0), // Panpour
        new(387, 10, 254, Gender: 0), // Turtwig
        new(390, 10, 252, Gender: 0), // Chimchar
        new(393, 10, 297, Gender: 0), // Piplup
        new(575, 32, 286, Gender: 0), // Gothorita
    });
    #endregion
    #region Static Encounter/Gift Tables
    private static readonly EncounterStatic5[] Encounter_B2W2_Regular =
    {
        // Starters @ Aspertia City
        new(B2W2) { Gift = true, Species = 495, Level = 05, Location = 117 }, // Snivy
        new(B2W2) { Gift = true, Species = 498, Level = 05, Location = 117 }, // Tepig
        new(B2W2) { Gift = true, Species = 501, Level = 05, Location = 117 }, // Oshawott

        // Fossils @ Nacrene City
        new(B2W2) { Gift = true, Species = 138, Level = 25, Location = 007 }, // Omanyte
        new(B2W2) { Gift = true, Species = 140, Level = 25, Location = 007 }, // Kabuto
        new(B2W2) { Gift = true, Species = 142, Level = 25, Location = 007 }, // Aerodactyl
        new(B2W2) { Gift = true, Species = 345, Level = 25, Location = 007 }, // Lileep
        new(B2W2) { Gift = true, Species = 347, Level = 25, Location = 007 }, // Anorith
        new(B2W2) { Gift = true, Species = 408, Level = 25, Location = 007 }, // Cranidos
        new(B2W2) { Gift = true, Species = 410, Level = 25, Location = 007 }, // Shieldon
        new(B2W2) { Gift = true, Species = 564, Level = 25, Location = 007 }, // Tirtouga
        new(B2W2) { Gift = true, Species = 566, Level = 25, Location = 007 }, // Archen

        // Gift
        new(B2W2) { Gift = true, Species = 133, Level = 10, Location = 008, Ability = OnlyHidden }, // HA Eevee @ Castelia City
        new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = OnlyHidden, Form = 0 }, // HA Deerling @ Route 6
        new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = OnlyHidden, Form = 1 }, // HA Deerling @ Route 6
        new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = OnlyHidden, Form = 2 }, // HA Deerling @ Route 6
        new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = OnlyHidden, Form = 3 }, // HA Deerling @ Route 6
        new(B2  ) { Gift = true, Species = 443, Level = 01, Location = 122, Shiny = Shiny.Always, Gender = 0 }, // Shiny Gible @ Floccesy Town
        new(  W2) { Gift = true, Species = 147, Level = 01, Location = 122, Shiny = Shiny.Always, Gender = 0 }, // Shiny Dratini @ Floccesy Town
        new(B2W2) { Gift = true, Species = 129, Level = 05, Location = 068 } , // Magikarp @ Marvelous Bridge
        new(B2W2) { Gift = true, Species = 440, Level = 01, EggLocation = 60003 }, // Happiny Egg from PKMN Breeder

        // Stationary
        new(B2W2) { Species = 590, Level = 29, Location = 019 }, // Foongus @ Route 6
        new(B2W2) { Species = 591, Level = 43, Location = 024 }, // Amoonguss @ Route 11
        new(B2W2) { Species = 591, Level = 47, Location = 127 }, // Amoonguss @ Route 22
        new(B2W2) { Species = 591, Level = 56, Location = 128 }, // Amoonguss @ Route 23
        new(B2  ) { Species = 593, Level = 40, Location = 071, Ability = OnlyHidden, Gender = 0 }, // HA Jellicent @ Undella Bay Mon Only
        new(  W2) { Species = 593, Level = 40, Location = 071, Ability = OnlyHidden, Gender = 1 }, // HA Jellicent @ Undella Bay Thurs Only
        new(B2W2) { Species = 593, Level = 40, Location = 071 }, // HA Jellicent @ Undella Bay EncounterSlot collision
        new(  W2) { Species = 628, Level = 25, Location = 017, Ability = OnlyHidden, Gender = 0 }, // HA Braviary @ Route 4 Mon Only
        new(B2  ) { Species = 630, Level = 25, Location = 017, Ability = OnlyHidden, Gender = 1 }, // HA Mandibuzz @ Route 4 Thurs Only
        new(B2W2) { Species = 637, Level = 35, Location = 035 }, // Volcarona @ Relic Castle
        new(B2W2) { Species = 637, Level = 65, Location = 035 }, // Volcarona @ Relic Castle
        new(B2W2) { Species = 558, Level = 42, Location = 141 }, // Crustle @ Seaside Cave
        new(B2W2) { Species = 612, Level = 60, Location = 147, Shiny = Shiny.Always}, // Haxorus @ Nature Preserve

        // Stationary Legendary
        new(B2W2) { Species = 377, Level = 65, Location = 150 }, // Regirock @ Rock Peak Chamber
        new(B2W2) { Species = 378, Level = 65, Location = 151 }, // Regice @ Iceberg Chamber
        new(B2W2) { Species = 379, Level = 65, Location = 152 }, // Registeel @ Iron Chamber
        new(  W2) { Species = 380, Level = 68, Location = 032 }, // Latias @ Dreamyard
        new(B2  ) { Species = 381, Level = 68, Location = 032 }, // Latios @ Dreamyard
        new(B2W2) { Species = 480, Level = 65, Location = 007 }, // Uxie @ Nacrene City
        new(B2W2) { Species = 481, Level = 65, Location = 056 }, // Mesprit @ Celestial Tower
        new(B2W2) { Species = 482, Level = 65, Location = 128 }, // Azelf @ Route 23
        new(B2W2) { Species = 485, Level = 68, Location = 132 }, // Heatran @ Reversal Mountain
        new(B2W2) { Species = 486, Level = 68, Location = 038 }, // Regigigas @ Twist Mountain
        new(B2W2) { Species = 488, Level = 68, Location = 068 }, // Cresselia @ Marvelous Bridge
        new(B2W2) { Species = 638, Level = 45, Location = 026 }, // Cobalion @ Route 13
        new(B2W2) { Species = 638, Level = 65, Location = 026 }, // Cobalion @ Route 13
        new(B2W2) { Species = 639, Level = 45, Location = 127 }, // Terrakion @ Route 22
        new(B2W2) { Species = 639, Level = 65, Location = 127 }, // Terrakion @ Route 22
        new(B2W2) { Species = 640, Level = 45, Location = 024 }, // Virizion @ Route 11
        new(B2W2) { Species = 640, Level = 65, Location = 024 }, // Virizion @ Route 11
        new(  W2) { Species = 643, Level = 70, Location = 039, Shiny = Shiny.Never }, // Reshiram @ Dragonspiral Tower
        new(B2  ) { Species = 644, Level = 70, Location = 039, Shiny = Shiny.Never }, // Zekrom @ Dragonspiral Tower
        new(B2W2) { Species = 646, Level = 70, Location = 061, Form = 0 }, // Kyurem @ Giant Chasm
    };

    private static readonly EncounterStatic5N[] Encounter_B2W2_N =
    {
        // N's Pokemon
        new(0xFF01007F) { Species = 509, Level = 07, Location = 015, Ability = OnlySecond, Nature = Nature.Timid }, // Purloin @ Route 2
        new(0xFF01007F) { Species = 519, Level = 13, Location = 033, Ability = OnlySecond, Nature = Nature.Sassy }, // Pidove @ Pinwheel Forest
        new(0xFF00003F) { Species = 532, Level = 13, Location = 033, Ability = OnlyFirst,  Nature = Nature.Rash }, // Timburr @ Pinwheel Forest
        new(0xFF01007F) { Species = 535, Level = 13, Location = 033, Ability = OnlySecond, Nature = Nature.Modest }, // Tympole @ Pinwheel Forest
        new(0xFF00007F) { Species = 527, Level = 55, Location = 053, Ability = OnlyFirst,  Nature = Nature.Timid }, // Woobat @ Wellspring Cave
        new(0xFF01007F) { Species = 551, Level = 22, Location = 034, Ability = OnlySecond, Nature = Nature.Docile }, // Sandile @ Desert Resort
        new(0xFF00007F) { Species = 554, Level = 22, Location = 034, Ability = OnlyFirst,  Nature = Nature.Naive }, // Darumaka @ Desert Resort
        new(0xFF00007F) { Species = 555, Level = 35, Location = 034, Ability = OnlyHidden, Nature = Nature.Calm }, // Darmanitan @ Desert Resort
        new(0xFF00007F) { Species = 559, Level = 22, Location = 034, Ability = OnlyFirst,  Nature = Nature.Lax }, // Scraggy @ Desert Resort
        new(0xFF01007F) { Species = 561, Level = 22, Location = 034, Ability = OnlySecond, Nature = Nature.Gentle }, // Sigilyph @ Desert Resort
        new(0xFF00007F) { Species = 525, Level = 28, Location = 037, Ability = OnlyFirst,  Nature = Nature.Naive }, // Boldore @ Chargestone Cave
        new(0xFF01007F) { Species = 595, Level = 28, Location = 037, Ability = OnlySecond, Nature = Nature.Docile }, // Joltik @ Chargestone Cave
        new(0xFF00007F) { Species = 597, Level = 28, Location = 037, Ability = OnlyFirst,  Nature = Nature.Bashful }, // Ferroseed @ Chargestone Cave
        new(0xFF000000) { Species = 599, Level = 28, Location = 037, Ability = OnlyFirst,  Nature = Nature.Rash }, // Klink @ Chargestone Cave
        new(0xFF00001F) { Species = 570, Level = 25, Location = 010, Ability = OnlyFirst,  Nature = Nature.Hasty, Gift = true }, // N's Zorua @ Driftveil City
    };

    private static readonly EncounterStatic5[] Encounter_B2W2 = ArrayUtil.ConcatAll(Encounter_B2W2_Regular, Encounter_B2W2_N, Encounter_DreamRadar);
    #endregion
    #region Trade Tables
    internal static readonly EncounterTrade5[] TradeGift_B2W2_Regular =
    {
        new(B2  ) { Species = 548, Level = 20, Ability = OnlySecond, TID16 = 65217, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(20,20,20,20,31,20), Nature = Nature.Timid }, // Petilil
        new(  W2) { Species = 546, Level = 20, Ability = OnlyFirst,  TID16 = 05720, SID16 = 00001, OTGender = 0, Gender = 0, IVs = new(20,20,20,20,31,20), Nature = Nature.Modest }, // Cottonee
        new(B2W2) { Species = 526, Level = 35, Ability = OnlyFirst,  TID16 = 11195, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(20,31,20,20,20,20), Nature = Nature.Adamant, IsNicknamed = false }, // Gigalith
        new(B2W2) { Species = 465, Level = 45, Ability = OnlyFirst,  TID16 = 27658, SID16 = 00001, OTGender = 0, Gender = 0, IVs = new(31,20,20,20,20,20), Nature = Nature.Hardy }, // Tangrowth
        new(B2W2) { Species = 479, Level = 60, Ability = OnlyFirst,  TID16 = 54673, SID16 = 00000, OTGender = 1, Gender = 2, IVs = new(20,20,20,20,20,31), Nature = Nature.Calm }, // Rotom
        new(B2W2) { Species = 424, Level = 40, Ability = OnlySecond, TID16 = 17074, SID16 = 00001, OTGender = 1, Gender = 0, IVs = new(20,20,20,31,20,20), Nature = Nature.Jolly }, // Ambipom
        new(B2W2) { Species = 065, Level = 40, Ability = OnlyFirst,  TID16 = 17074, SID16 = 00001, OTGender = 1, Gender = 0, IVs = new(20,20,20,31,20,20), Nature = Nature.Timid }, // Alakazam
    };

    internal const int YancyTID = 10303;
    internal const int CurtisTID = 54118;
    private static readonly string[] TradeOT_B2W2_F = { string.Empty, "ルリ", "Yancy", "Brenda", "Lilì", "Sabine", string.Empty, "Belinda", "루리" };
    private static readonly string[] TradeOT_B2W2_M = { string.Empty, "テツ", "Curtis", "Julien", "Dadi", "Markus", string.Empty, "Julián", "철권" };

    private static readonly EncounterTrade5[] TradeGift_B2W2_YancyCurtis =
    {
        // Player is Male
        new(B2W2) { Species = 052, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Meowth
        new(B2W2) { Species = 202, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Wobbuffet
        new(B2W2) { Species = 280, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Ralts
        new(B2W2) { Species = 410, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Shieldon
        new(B2W2) { Species = 111, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Rhyhorn
        new(B2W2) { Species = 422, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, Form = 0 }, // Shellos-West
        new(B2W2) { Species = 303, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Mawile
        new(B2W2) { Species = 442, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Spiritomb
        new(B2W2) { Species = 143, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Snorlax
        new(B2W2) { Species = 216, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Teddiursa
        new(B2W2) { Species = 327, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Spinda
        new(B2W2) { Species = 175, Level = 50, Ability = OnlyHidden, TID16 = 10303, SID16 = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F }, // Togepi

        // Player is Female
        new(B2W2) { Species = 056, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Mankey
        new(B2W2) { Species = 202, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Wobbuffet
        new(B2W2) { Species = 280, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Ralts
        new(B2W2) { Species = 408, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Cranidos
        new(B2W2) { Species = 111, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Rhyhorn
        new(B2W2) { Species = 422, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, Form = 1 }, // Shellos-East
        new(B2W2) { Species = 302, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Sableye
        new(B2W2) { Species = 442, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Spiritomb
        new(B2W2) { Species = 143, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Snorlax
        new(B2W2) { Species = 231, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Phanpy
        new(B2W2) { Species = 327, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Spinda
        new(B2W2) { Species = 175, Level = 50, Ability = OnlyHidden, TID16 = 54118, SID16 = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M }, // Togepi
    };

    private const string tradeB2W2 = "tradeb2w2";
    private static readonly string[][] TradeB2W2 = Util.GetLanguageStrings8(tradeB2W2);

    internal static readonly EncounterTrade5[] TradeGift_B2W2 = ArrayUtil.ConcatAll(TradeGift_B2W2_Regular, TradeGift_B2W2_YancyCurtis);

    #endregion

    internal static readonly EncounterStatic5[] StaticB2 = ArrayUtil.ConcatAll(GetEncounters(Encounter_B2W2, B2), DreamWorld_Common, DreamWorld_B2W2);
    internal static readonly EncounterStatic5[] StaticW2 = ArrayUtil.ConcatAll(GetEncounters(Encounter_B2W2, W2), DreamWorld_Common, DreamWorld_B2W2);
}
