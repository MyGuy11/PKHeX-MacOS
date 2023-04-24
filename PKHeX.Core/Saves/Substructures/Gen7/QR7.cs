using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;
// anatomy of a QR7:
// u32 magic; // POKE
// u32 _0xFF;
// u32 box;
// u32 slot;
// u32 num_copies;
// u8  reserved[0x1C];
// u8  ek7[0x104];
// u8  dex_data[0x60];
// u16 crc16
// sizeof(QR7) == 0x1A2

/// <summary>
/// Generation 7 QR format (readable by the in-game QR Scanner feature)
/// </summary>
public static class QR7
{
    private static readonly HashSet<ushort> GenderDifferences = new()
    {
        003, 012, 019, 020, 025, 026, 041, 042, 044, 045,
        064, 065, 084, 085, 097, 111, 112, 118, 119, 123,
        129, 130, 154, 165, 166, 178, 185, 186, 190, 194,
        195, 198, 202, 203, 207, 208, 212, 214, 215, 217,
        221, 224, 229, 232, 255, 256, 257, 267, 269, 272,
        274, 275, 307, 308, 315, 316, 317, 322, 323, 332,
        350, 369, 396, 397, 398, 399, 400, 401, 402, 403,
        404, 405, 407, 415, 417, 418, 419, 424, 443, 444,
        445, 449, 450, 453, 454, 456, 457, 459, 460, 461,
        464, 465, 473, 521, 592, 593, 668, 678,
    };

    private static void GetRawQR(Span<byte> dest, ushort species, byte form, bool shiny, byte gender)
    {
        dest[..6].Fill(0xFF);
        WriteUInt16LittleEndian(dest[0x28..], species);

        var pi = PersonalTable.USUM.GetFormEntry(species, form);
        bool biGender = false;
        if (pi.OnlyMale)
            gender = 0;
        else if (pi.OnlyFemale)
            gender = 1;
        else if (pi.Genderless)
            gender = 2;
        else
            biGender = !GenderDifferences.Contains(species);

        dest[0x2A] = form;
        dest[0x2B] = gender;
        dest[0x2C] = shiny ? (byte)1 : (byte)0;
        dest[0x2D] = biGender ? (byte)1 : (byte)0;
    }

    public static byte[] GenerateQRData(PK7 pk7, int box = 0, int slot = 0, int num_copies = 1)
    {
        if (box > 31)
            box = 31;
        if (slot > 29)
            slot = 29;
        if (box < 0)
            box = 0;
        if (slot < 0)
            slot = 0;
        if (num_copies < 0)
            num_copies = 1;

        byte[] data = new byte[0x1A2];
        var span = data.AsSpan();
        WriteUInt32LittleEndian(span, 0x454B4F50); // POKE magic
        data[0x4] = 0xFF; // QR Type
        WriteInt32LittleEndian(span[0x08..], box);
        WriteInt32LittleEndian(span[0x0C..], slot);
        WriteInt32LittleEndian(span[0x10..], num_copies); // No need to check max num_copies, payload parser handles it on-console.

        pk7.EncryptedPartyData.CopyTo(span[0x30..]); // Copy in pokemon data
        GetRawQR(span[0x140..], pk7.Species, pk7.Form, pk7.IsShiny, (byte)pk7.Gender);

        var chk = Checksums.CRC16Invert(span[..0x1A0]);
        WriteUInt16LittleEndian(span[0x1A0..], chk);
        return data;
    }
}
