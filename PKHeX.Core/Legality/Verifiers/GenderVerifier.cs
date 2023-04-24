using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Gender"/>.
/// </summary>
public sealed class GenderVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Gender;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var pi = pk.PersonalInfo;
        if (pi.Genderless != (pk.Gender == 2))
        {
            // DP/HGSS shedinja glitch -- only generation 4 spawns
            bool ignore = pk is { Format: 4, Species: (int)Species.Shedinja } && pk.Met_Level != pk.CurrentLevel;
            if (!ignore)
                data.AddLine(GetInvalid(LGenderInvalidNone));
            return;
        }

        // Check for PID relationship to Gender & Nature if applicable
        int gen = data.Info.Generation;
        if (gen is 3 or 4 or 5)
        {
            // Gender-PID & Nature-PID relationship check
            var result = IsValidGenderPID(data) ? GetValid(LPIDGenderMatch) : GetInvalid(LPIDGenderMismatch);
            data.AddLine(result);

            if (gen != 5)
                VerifyNaturePID(data);
            return;
        }

        // Check fixed gender cases
        if ((pi.OnlyFemale && pk.Gender != 1) || (pi.OnlyMale && pk.Gender != 0))
            data.AddLine(GetInvalid(LGenderInvalidNone));
    }

    private static void VerifyNaturePID(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var result = pk.EncryptionConstant % 25 == pk.Nature
            ? GetValid(LPIDNatureMatch, CheckIdentifier.Nature)
            : GetInvalid(LPIDNatureMismatch, CheckIdentifier.Nature);
        data.AddLine(result);
    }

    private static bool IsValidGenderPID(LegalityAnalysis data)
    {
        var pk = data.Entity;
        bool genderValid = pk.IsGenderValid();
        if (!genderValid)
            return IsValidGenderMismatch(pk);

        // check for mixed->fixed gender incompatibility by checking the gender of the original species
        var original = data.EncounterMatch.Species;
        if (Legal.FixedGenderFromBiGender.Contains(original))
            return IsValidFixedGenderFromBiGender(pk, original);

        return true;
    }

    private static bool IsValidFixedGenderFromBiGender(PKM pk, ushort original)
    {
        var current = pk.Gender;
        if (current == 2) // shedinja, genderless
            return true;
        var gender = EntityGender.GetFromPID(original, pk.EncryptionConstant);
        return gender == current;
    }

    private static bool IsValidGenderMismatch(PKM pk) => pk.Species switch
    {
        // Shedinja evolution gender glitch, should match original Gender
        (int) Species.Shedinja when pk.Format == 4 => pk.Gender == EntityGender.GetFromPIDAndRatio(pk.EncryptionConstant, 0x7F), // 50M-50F

        // Evolved from Azurill after transferring to keep gender
        (int) Species.Marill or (int) Species.Azumarill when pk.Format >= 6 => pk.Gender == 1 && (pk.EncryptionConstant & 0xFF) > 0x3F,

        _ => false,
    };
}
