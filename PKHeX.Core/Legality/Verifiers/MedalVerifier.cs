﻿using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="ISuperTrain.SuperTrainingMedalCount"/> and associated values.
/// </summary>
public sealed class MedalVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Training;

    public override void Verify(LegalityAnalysis data)
    {
        VerifyMedalsRegular(data);
        VerifyMedalsEvent(data);
    }

    private void VerifyMedalsRegular(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var train = (ISuperTrain)pk;
        var Info = data.Info;
        uint value = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(data.Entity.Data.AsSpan(0x2C));
        if ((value & 3) != 0) // 2 unused flags
            data.AddLine(GetInvalid(LSuperUnused));
        int TrainCount = train.SuperTrainingMedalCount();

        if (pk.IsEgg)
        {
            // Can't have any super training data as an egg.
            if (TrainCount > 0)
                data.AddLine(GetInvalid(LSuperEgg));
            if (train.SecretSuperTrainingUnlocked)
                data.AddLine(GetInvalid(LSuperNoUnlocked));
            if (train.SecretSuperTrainingComplete)
                data.AddLine(GetInvalid(LSuperNoComplete));
            return;
        }

        if (Info.Generation is >= 7 or <= 2)
        {
            // Can't have any super training data if it never visited Gen6.
            if (TrainCount > 0)
                data.AddLine(GetInvalid(LSuperUnavailable));
            if (train.SecretSuperTrainingUnlocked)
                data.AddLine(GetInvalid(LSuperNoUnlocked));
            if (train.SecretSuperTrainingComplete)
                data.AddLine(GetInvalid(LSuperNoComplete));
            return;
        }

        if (pk.Format >= 7)
        {
            // Gen6->Gen7 transfer wipes the two Secret flags.
            if (train.SecretSuperTrainingUnlocked)
                data.AddLine(GetInvalid(LSuperNoUnlocked));
            if (train.SecretSuperTrainingComplete)
                data.AddLine(GetInvalid(LSuperNoComplete));
            return;
        }

        // Only reach here if Format==6.
        if (TrainCount == 30 ^ train.SecretSuperTrainingComplete)
            data.AddLine(GetInvalid(LSuperComplete));
    }

    private void VerifyMedalsEvent(LegalityAnalysis data)
    {
        var pk = data.Entity;
        byte value = pk.Data[0x3A];
        if (value != 0)
            data.AddLine(GetInvalid(LSuperDistro));
    }
}
