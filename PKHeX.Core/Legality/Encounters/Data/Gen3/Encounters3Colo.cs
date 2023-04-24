using static PKHeX.Core.Encounters3ColoTeams;

namespace PKHeX.Core;

internal static class Encounters3Colo
{
    internal static readonly EncounterStatic3[] Encounter_ColoGift =
    {
        // Colosseum Starters: Gender locked to male
        new(196, 25, GameVersion.COLO) { Gift = true, Location = 254, Gender = 0 }, // Espeon
        new(197, 26, GameVersion.COLO) { Gift = true, Location = 254, Gender = 0, Moves = new(044) }, // Umbreon (Bite)
    };

    internal static readonly EncounterStaticShadow[] Encounter_Colo =
    {
        new(GameVersion.COLO, 01, 03000, ColoMakuhita) { Species = 296, Level = 30, Moves = new(193,116,233,238), Location = 005 }, // Makuhita: Miror B.Peon Trudly @ Phenac City

        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 003  }, // Bayleef: Cipher Peon Verde @ Phenac City
        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 069  }, // Bayleef: Cipher Peon Verde @ Shadow PKMN Lab
        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 115  }, // Bayleef: Cipher Peon Verde @ Realgam Tower
        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 132  }, // Bayleef: Cipher Peon Verde @ Snagem Hideout
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 003  }, // Quilava: Cipher Peon Rosso @ Phenac City
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 069  }, // Quilava: Cipher Peon Rosso @ Shadow PKMN Lab
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 115  }, // Quilava: Cipher Peon Rosso @ Realgam Tower
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 132  }, // Quilava: Cipher Peon Rosso @ Snagem Hideout
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 003  }, // Croconaw: Cipher Peon Bluno @ Phenac City
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 069  }, // Croconaw: Cipher Peon Bluno @ Shadow PKMN Lab
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 115  }, // Croconaw: Cipher Peon Bluno @ Realgam Tower
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 132  }, // Croconaw: Cipher Peon Bluno @ Snagem Hideout
        new(GameVersion.COLO, 05, 03000, First)     { Species = 164, Level = 30, Moves = new(211,095,115,019), Location = 015 }, // Noctowl: Rider Nover @ Pyrite Town
        new(GameVersion.COLO, 06, 03000, First)     { Species = 180, Level = 30, Moves = new(085,086,178,084), Location = 015 }, // Flaaffy: St.Performer Diogo @ Pyrite Town
        new(GameVersion.COLO, 07, 03000, First)     { Species = 188, Level = 30, Moves = new(235,079,178,072), Location = 015 }, // Skiploom: Rider Leba @ Pyrite Town
        new(GameVersion.COLO, 08, 04000, First)     { Species = 195, Level = 30, Moves = new(341,133,021,057), Location = 015 }, // Quagsire: Bandana Guy Divel @ Pyrite Town
        new(GameVersion.COLO, 09, 04000, First)     { Species = 200, Level = 30, Moves = new(060,109,212,247), Location = 015 }, // Misdreavus: Rider Vant @ Pyrite Town
        new(GameVersion.COLO, 10, 05000, First)     { Species = 193, Level = 33, Moves = new(197,048,049,253), Location = 025  }, // Yanma: Cipher Peon Nore @ Pyrite Bldg
        new(GameVersion.COLO, 10, 05000, First)     { Species = 193, Level = 33, Moves = new(197,048,049,253), Location = 132  }, // Yanma: Cipher Peon Nore @ Snagem Hideout
        new(GameVersion.COLO, 11, 05000, First)     { Species = 162, Level = 33, Moves = new(231,270,098,070), Location = 015  }, // Furret: Rogue Cail @ Pyrite Town
        new(GameVersion.COLO, 12, 04000, First)     { Species = 218, Level = 30, Moves = new(241,281,088,053), Location = 015  }, // Slugma: Roller Boy Lon @ Pyrite Town
        new(GameVersion.COLO, 13, 04000, First)     { Species = 223, Level = 20, Moves = new(061,199,060,062), Location = 028 }, // Remoraid: Miror B.Peon Reath @ Pyrite Bldg
        new(GameVersion.COLO, 13, 04000, First)     { Species = 223, Level = 20, Moves = new(061,199,060,062), Location = 030 }, // Remoraid: Miror B.Peon Reath @ Pyrite Cave
        new(GameVersion.COLO, 14, 05000, First)     { Species = 226, Level = 33, Moves = new(017,048,061,036), Location = 028 }, // Mantine: Miror B.Peon Ferma @ Pyrite Bldg
        new(GameVersion.COLO, 14, 05000, First)     { Species = 226, Level = 33, Moves = new(017,048,061,036), Location = 030 }, // Mantine: Miror B.Peon Ferma @ Pyrite Cave
        new(GameVersion.COLO, 15, 05000, First)     { Species = 211, Level = 33, Moves = new(042,107,040,057), Location = 015 }, // Qwilfish: Hunter Doken @ Pyrite Bldg
        new(GameVersion.COLO, 16, 05000, First)     { Species = 307, Level = 33, Moves = new(197,347,093,136), Location = 031 }, // Meditite: Rider Twan @ Pyrite Cave
        new(GameVersion.COLO, 17, 05000, First)     { Species = 206, Level = 33, Moves = new(180,137,281,036), Location = 029 }, // Dunsparce: Rider Sosh @ Pyrite Cave
        new(GameVersion.COLO, 18, 05000, First)     { Species = 333, Level = 33, Moves = new(119,047,219,019), Location = 032 }, // Swablu: Hunter Zalo @ Pyrite Cave
        new(GameVersion.COLO, 19, 10000, First)     { Species = 185, Level = 35, Moves = new(175,335,067,157), Location = 104 }, // Sudowoodo: Cipher Admin Miror B. @ Realgam Tower
        new(GameVersion.COLO, 19, 10000, First)     { Species = 185, Level = 35, Moves = new(175,335,067,157), Location = 125 }, // Sudowoodo: Cipher Admin Miror B. @ Deep Colosseum
        new(GameVersion.COLO, 19, 10000, First)     { Species = 185, Level = 35, Moves = new(175,335,067,157), Location = 030 }, // Sudowoodo: Cipher Admin Miror B. @ Pyrite Cave
        new(GameVersion.COLO, 20, 06000, First)     { Species = 237, Level = 38, Moves = new(097,116,167,229), Location = 039 }, // Hitmontop: Cipher Peon Skrub @ Agate Village
        new(GameVersion.COLO, 20, 06000, First)     { Species = 237, Level = 38, Moves = new(097,116,167,229), Location = 132 }, // Hitmontop: Cipher Peon Skrub @ Snagem Hideout
        new(GameVersion.COLO, 20, 06000, First)     { Species = 237, Level = 38, Moves = new(097,116,167,229), Location = 068 }, // Hitmontop: Cipher Peon Skrub @ Shadow PKMN Lab
        new(GameVersion.COLO, 21, 13000, First)     { Species = 244, Level = 40, Moves = new(241,043,044,126), Location = 106 }, // Entei: Cipher Admin Dakim @ Realgam Tower
        new(GameVersion.COLO, 21, 13000, First)     { Species = 244, Level = 40, Moves = new(241,043,044,126), Location = 125 }, // Entei: Cipher Admin Dakim @ Deep Colosseum
        new(GameVersion.COLO, 21, 13000, First)     { Species = 244, Level = 40, Moves = new(241,043,044,126), Location = 076 }, // Entei: Cipher Admin Dakim @ Mt. Battle
        new(GameVersion.COLO, 22, 06000, First)     { Species = 166, Level = 40, Moves = new(226,219,048,004), Location = 047 }, // Ledian: Cipher Peon Kloak @ The Under
        new(GameVersion.COLO, 22, 06000, First)     { Species = 166, Level = 40, Moves = new(226,219,048,004), Location = 132 }, // Ledian: Cipher Peon Kloak @ Snagem Hideout
        new(GameVersion.COLO, 23, 13000, First)     { Species = 245, Level = 40, Moves = new(240,043,016,057), Location = 110 }, // Suicune (Surf): Cipher Admin Venus @ Realgam Tower
        new(GameVersion.COLO, 23, 13000, First)     { Species = 245, Level = 40, Moves = new(240,043,016,056), Location = 125 }, // Suicune (Hydro Pump): Cipher Admin Venus @ Deep Colosseum
        new(GameVersion.COLO, 23, 13000, First)     { Species = 245, Level = 40, Moves = new(240,043,016,057), Location = 055 }, // Suicune (Surf): Cipher Admin Venus @ The Under
        new(GameVersion.COLO, 24, 06000, Gligar)    { Species = 207, Level = 43, Moves = new(185,028,040,163), Location = 058 }, // Gligar: Hunter Frena @ The Under Subway
        new(GameVersion.COLO, 24, 06000, Gligar)    { Species = 207, Level = 43, Moves = new(185,028,040,163), Location = 133 }, // Gligar: Hunter Frena @ Snagem Hideout
        new(GameVersion.COLO, 25, 06000, First)     { Species = 234, Level = 43, Moves = new(310,095,043,036), Location = 058 }, // Stantler: Chaser Liaks @ The Under Subway
        new(GameVersion.COLO, 25, 06000, First)     { Species = 234, Level = 43, Moves = new(310,095,043,036), Location = 133 }, // Stantler: Chaser Liaks @ Snagem Hideout
        new(GameVersion.COLO, 25, 06000, First)     { Species = 221, Level = 43, Moves = new(203,316,091,059), Location = 058 }, // Piloswine: Bodybuilder Lonia @ The Under Subway
        new(GameVersion.COLO, 26, 06000, First)     { Species = 221, Level = 43, Moves = new(203,316,091,059), Location = 134 }, // Piloswine: Bodybuilder Lonia @ Snagem Hideout
        new(GameVersion.COLO, 27, 06000, First)     { Species = 215, Level = 43, Moves = new(185,103,154,196), Location = 058 }, // Sneasel: Rider Nelis @ The Under Subway
        new(GameVersion.COLO, 27, 06000, First)     { Species = 215, Level = 43, Moves = new(185,103,154,196), Location = 134 }, // Sneasel: Rider Nelis @ Snagem Hideout
        new(GameVersion.COLO, 28, 06000, First)     { Species = 190, Level = 43, Moves = new(226,321,154,129), Location = 067 }, // Aipom: Cipher Peon Cole @ Shadow PKMN Lab
        new(GameVersion.COLO, 29, 06000, Murkrow)   { Species = 198, Level = 43, Moves = new(185,212,101,019), Location = 067 }, // Murkrow: Cipher Peon Lare @ Shadow PKMN Lab
        new(GameVersion.COLO, 30, 06000, First)     { Species = 205, Level = 43, Moves = new(153,182,117,229), Location = 067 }, // Forretress: Cipher Peon Vana @ Shadow PKMN Lab
        new(GameVersion.COLO, 31, 06000, First)     { Species = 210, Level = 43, Moves = new(044,184,046,070), Location = 069 }, // Granbull: Cipher Peon Tanie @ Shadow PKMN Lab
        new(GameVersion.COLO, 32, 06000, First)     { Species = 329, Level = 43, Moves = new(242,103,328,225), Location = 068 }, // Vibrava: Cipher Peon Remil @ Shadow PKMN Lab
        new(GameVersion.COLO, 33, 06000, First)     { Species = 168, Level = 43, Moves = new(169,184,141,188), Location = 069 }, // Ariados: Cipher Peon Lesar @ Shadow PKMN Lab

        new(GameVersion.COLO, 34, 13000, First)     { Species = 243, Level = 40, Moves = new(240,043,098,087), Location = 113 }, // Raikou: Cipher Admin Ein @ Realgam Tower
        new(GameVersion.COLO, 34, 13000, First)     { Species = 243, Level = 40, Moves = new(240,043,098,087), Location = 125 }, // Raikou: Cipher Admin Ein @ Deep Colosseum
        new(GameVersion.COLO, 34, 13000, First)     { Species = 243, Level = 40, Moves = new(240,043,098,087), Location = 069 }, // Raikou: Cipher Admin Ein @ Shadow PKMN Lab

        new(GameVersion.COLO, 35, 07000, First)     { Species = 192, Level = 45, Moves = new(241,074,275,076), Location = 109 }, // Sunflora: Cipher Peon Baila @ Realgam Tower
        new(GameVersion.COLO, 35, 07000, First)     { Species = 192, Level = 45, Moves = new(241,074,275,076), Location = 132 }, // Sunflora: Cipher Peon Baila @ Snagem Hideout
        new(GameVersion.COLO, 36, 07000, First)     { Species = 225, Level = 45, Moves = new(059,213,217,019), Location = 109 }, // Delibird: Cipher Peon Arton @ Realgam Tower
        new(GameVersion.COLO, 36, 07000, First)     { Species = 225, Level = 45, Moves = new(059,213,217,019), Location = 132 }, // Delibird: Cipher Peon Arton @ Snagem Hideout
        new(GameVersion.COLO, 37, 07000, Heracross) { Species = 214, Level = 45, Moves = new(179,203,068,280), Location = 111 }, // Heracross: Cipher Peon Dioge @ Realgam Tower
        new(GameVersion.COLO, 37, 07000, Heracross) { Species = 214, Level = 45, Moves = new(179,203,068,280), Location = 132 }, // Heracross: Cipher Peon Dioge @ Snagem Hideout
        new(GameVersion.COLO, 38, 13000, First)     { Species = 227, Level = 47, Moves = new(065,319,314,211), Location = 117 }, // Skarmory: Snagem Head Gonzap @ Realgam Tower
        new(GameVersion.COLO, 38, 13000, First)     { Species = 227, Level = 47, Moves = new(065,319,314,211), Location = 133 }, // Skarmory: Snagem Head Gonzap @ Snagem Hideout

        new(GameVersion.COLO, 39, 07000, First)     { Species = 241, Level = 48, Moves = new(208,111,205,034), Location = 118 }, // Miltank: Bodybuilder Jomas @ Tower Colosseum
        new(GameVersion.COLO, 40, 07000, First)     { Species = 359, Level = 48, Moves = new(195,014,163,185), Location = 118 }, // Absol: Rider Delan @ Tower Colosseum
        new(GameVersion.COLO, 41, 07000, First)     { Species = 229, Level = 48, Moves = new(185,336,123,053), Location = 118 }, // Houndoom: Cipher Peon Nella @ Tower Colosseum
        new(GameVersion.COLO, 42, 07000, First)     { Species = 357, Level = 49, Moves = new(076,235,345,019), Location = 118 }, // Tropius: Cipher Peon Ston @ Tower Colosseum
        new(GameVersion.COLO, 43, 15000, First)     { Species = 376, Level = 50, Moves = new(063,334,232,094), Location = 118 }, // Metagross: Cipher Nascour @ Tower Colosseum
        new(GameVersion.COLO, 44, 20000, First)     { Species = 248, Level = 55, Moves = new(242,087,157,059), Location = 118 }, // Tyranitar: Cipher Head Evice @ Tower Colosseum

        new(GameVersion.COLO, 55, 07000, First)     { Species = 235, Level = 45, Moves = new(166,039,003,231), Location = 132 }, // Smeargle: Team Snagem Biden @ Snagem Hideout
        new(GameVersion.COLO, 56, 07000, Ursaring)  { Species = 217, Level = 45, Moves = new(185,313,122,163), Location = 132 }, // Ursaring: Team Snagem Agrev @ Snagem Hideout
        new(GameVersion.COLO, 57, 07000, First)     { Species = 213, Level = 45, Moves = new(219,227,156,117), Location = 125 }, // Shuckle: Deep King Agnol @ Deep Colosseum

        new(GameVersion.COLO, 67, 05000, First)     { Species = 176, Level = 20, Moves = new(118,204,186,281), Location = 001 }, // Togetic: Cipher Peon Fein @ Outskirt Stand

        new(GameVersion.COLO, 00, 00000, CTogepi)   { Species = 175, Level = 20, Moves = new(118,204,186,281), Location = 128, IVs = new(0, 0, 0, 0, 0, 0) }, // Togepi: Chaser ボデス @ Card e Room (Japanese games only)
        new(GameVersion.COLO, 00, 00000, CMareep)   { Species = 179, Level = 37, Moves = new(087,084,086,178), Location = 128, IVs = new(0, 0, 0, 0, 0, 0) }, // Mareep: Hunter ホル @ Card e Room (Japanese games only)
        new(GameVersion.COLO, 00, 00000, CScizor)   { Species = 212, Level = 50, Moves = new(210,232,014,163), Location = 128, IVs = new(0, 0, 0, 0, 0, 0) }, // Scizor: Bodybuilder ワーバン @ Card e Room (Japanese games only)
    };
}
