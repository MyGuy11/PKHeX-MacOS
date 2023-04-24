using System;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Distributed Data)
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8ND : EncounterStatic8Nest<EncounterStatic8ND>
{
    /// <summary>
    /// Distribution raid index for <see cref="GameVersion.SWSH"/>
    /// </summary>
    public byte Index { get; }

    public EncounterStatic8ND(byte lvl, byte dyna, byte flawless, byte index, GameVersion game) : base(game)
    {
        Level = lvl;
        DynamaxLevel = dyna;
        FlawlessIVCount = flawless;
        Index = index;
    }

    public static EncounterStatic8ND Read(ReadOnlySpan<byte> data, GameVersion game)
    {
        var d = data[13];
        var dlvl = (byte)(d & 0x7F);
        var gmax = d >= 0x80;
        var f = data[14];
        var flawless = (byte)(f & 0xF);
        var shiny = (f >> 4) switch
        {
            1 => Shiny.Never,
            2 => Shiny.Always,
            _ => Shiny.Random,
        };

        var move1 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[4..]);
        var move2 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[6..]);
        var move3 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[8..]);
        var move4 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[10..]);
        var moves = new Moveset(move1, move2, move3, move4);

        return new EncounterStatic8ND(data[12], dlvl, flawless, data[15], game)
        {
            Species = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data),
            Form = data[2],
            Ability = (AbilityPermission)data[3],
            CanGigantamax = gmax,
            Moves = moves,
            Shiny = shiny,
        };
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        var lvl = pk.Met_Level;

        if (lvl <= 25) // 1 or 2 stars
        {
            var met = pk.Met_Location;
            if (met <= byte.MaxValue && InaccessibleRank12DistributionLocations.Contains((byte)met))
                return false;
        }

        if (lvl == Level)
            return true;

        // Check downleveled (20-55)
        if (lvl > Level)
            return false;
        if (lvl is < 20 or > 55)
            return false;

        if (lvl % 5 != 0)
            return false;

        // shared nests can be down-leveled to any
        if (pk.Met_Location == SharedNest)
            return true;

        // native down-levels: only allow 1 rank down (1 badge 2star -> 25), (3badge 3star -> 35)
        var badges = (lvl - 20) / 5;
        return badges is 1 or 3 && !pk.IsShiny;
    }

    private const byte IndexMinDLC2 = 40;
    private const byte IndexMinDLC1 = 25;

    protected override bool IsMatchLocation(PKM pk)
    {
        var loc = pk.Met_Location;
        return loc is SharedNest || Index switch
        {
            >= IndexMinDLC2 => EncounterArea8.IsWildArea(loc),
            >= IndexMinDLC1 => EncounterArea8.IsWildArea8(loc) || EncounterArea8.IsWildArea8Armor(loc),
            _ => EncounterArea8.IsWildArea8(loc),
        };
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.FlawlessIVCount < FlawlessIVCount)
            return false;

        return base.IsMatchExact(pk, evo);
    }
}
