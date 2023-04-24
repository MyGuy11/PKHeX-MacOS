using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen2"/>.
/// </summary>
/// <remarks>
/// Referenced Area object contains Time data which is used for <see cref="GameVersion.C"/> origin data.
/// </remarks>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot2 : EncounterSlot, INumberedSlot
{
    public override int Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    public byte SlotNumber { get; }
    public override Ball FixedBall => Ball.Poke;
    public bool IsHeadbutt => SlotType == SlotType.Headbutt;

    public EncounterSlot2(EncounterArea2 area, byte species, byte min, byte max, byte slot) : base(area, species, species == 201 ? FormRandom : (byte)0, min, max)
    {
        SlotNumber = slot;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        var pk2 = (PK2)pk;

        if (IsHeadbutt)
        {
            var id = pk2.TID16;
            if (!IsTreeAvailable(id))
            {
                // Get a random TID that satisfies this slot.
                do { id = (ushort)Util.Rand.Next(); }
                while (!IsTreeAvailable(id));
                pk2.TID16 = id;
            }
        }

        if (Version == GameVersion.C)
            pk2.Met_TimeOfDay = ((EncounterArea2)Area).Time.RandomValidTime();
    }

    private static readonly Dictionary<int, int> Trees = new()
    {
        { 02, 0x3FF_3FF }, // Route 29
        { 04, 0x0FF_3FF }, // Route 30
        { 05, 0x3FE_3FF }, // Route 31
        { 08, 0x3EE_3FF }, // Route 32
        { 11, 0x240_3FF }, // Route 33
        { 12, 0x37F_3FF }, // Azalea Town
        { 14, 0x3FF_3FF }, // Ilex Forest
        { 15, 0x001_3FE }, // Route 34
        { 18, 0x261_3FF }, // Route 35
        { 20, 0x3FF_3FF }, // Route 36
        { 21, 0x2B9_3FF }, // Route 37
        { 25, 0x3FF_3FF }, // Route 38
        { 26, 0x184_3FF }, // Route 39
        { 34, 0x3FF_3FF }, // Route 42
        { 37, 0x3FF_3FF }, // Route 43
        { 38, 0x3FF_3FF }, // Lake of Rage
        { 39, 0x2FF_3FF }, // Route 44
        { 91, 0x200_1FF }, // Route 26
        { 92, 0x2BB_3FF }, // Route 27
    };

    public bool IsTreeAvailable(ushort trainerID)
    {
        if (!Trees.TryGetValue(Location, out var permissions))
            return false;

        var pivot = trainerID % 10;
        var type = Area.Type;
        return type switch
        {
            SlotType.Headbutt => (permissions & (1 << pivot)) != 0,
            /*special*/ _ => (permissions & (1 << (pivot + 12))) != 0,
        };
    }

    // we have "Special" bitflag. Strip it out.
    public SlotType SlotType => Area.Type & (SlotType)0xF;
}
