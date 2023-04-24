using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo6AO"/> used in Generation 6 games.
/// </summary>
public sealed class PersonalTable6AO : IPersonalTable, IPersonalTable<PersonalInfo6AO>
{
    private readonly PersonalInfo6AO[] Table;
    private const int SIZE = PersonalInfo6AO.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_6;
    public int MaxSpeciesID => MaxSpecies;

    public PersonalTable6AO(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo6AO[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo6AO(slice);
        }
    }

    public PersonalInfo6AO this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo6AO this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo6AO GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form)
    {
        if (species <= MaxSpeciesID)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(ushort species) => species <= MaxSpecies;
    public bool IsPresentInGame(ushort species, byte form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        if (Table[species].HasForm(form))
            return true;
        return species switch
        {
            (int)Unown => form < 28,
            (int)Burmy => form < 3,
            (int)Mothim => form < 3,
            (int)Shellos or (int)Gastrodon => form == 1,
            (int)Arceus => form < 18,
            (int)Deerling or (int)Sawsbuck => form < 4,
            (int)Genesect => form < 5,
            (int)Scatterbug or (int)Spewpa => form < 18,
            (int)Vivillon => form <= 19,
            (int)Flabébé or (int)Florges => form < 5,
            (int)Floette => form <= 5,
            (int)Xerneas => form == 1,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);
}
