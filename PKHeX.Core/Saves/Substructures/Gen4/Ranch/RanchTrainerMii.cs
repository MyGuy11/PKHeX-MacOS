using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchTrainerMii
{
    public const int SIZE = 0x2C;
    public readonly byte[] Data;

    public RanchTrainerMii(byte[] data) => Data = data;

    public uint MiiId    { get => ReadUInt32BigEndian(Data.AsSpan(0x00)); set => WriteUInt32BigEndian(Data.AsSpan(0x00), value); }
    public uint SystemId { get => ReadUInt32BigEndian(Data.AsSpan(0x04)); set => WriteUInt32BigEndian(Data.AsSpan(0x04), value); }

    public ushort TrainerId { get => ReadUInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), value); }
    public ushort SecretId  { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value); }

    private Span<byte> Trainer_Trash => Data.AsSpan(0x10, 0x10);

    // 0x20-23: ??
    // 0x24:    ??
    // 0x25:    ??
    // 0x26-27: ??
    // 0x28-29: ??
    // 0x2A-2B: ??

    public string TrainerName
    {
        get => StringConverter4.GetString(Trainer_Trash);
        set => StringConverter4.SetString(Trainer_Trash, value, 7);
    }
}
