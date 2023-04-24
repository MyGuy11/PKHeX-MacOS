using System;

namespace PKHeX.Core;

/// <summary>
/// Utility logic for dealing with bitflags in a byte array.
/// </summary>
public static class FlagUtil
{
    /// <summary>
    /// Gets the requested <see cref="bitIndex"/> from the byte at <see cref="offset"/>.
    /// </summary>
    /// <param name="arr">Buffer to read</param>
    /// <param name="offset">Offset of the byte</param>
    /// <param name="bitIndex">Bit to read</param>
    public static bool GetFlag(ReadOnlySpan<byte> arr, int offset, int bitIndex)
    {
        bitIndex &= 7; // ensure bit access is 0-7
        return ((arr[offset] >> bitIndex) & 1) != 0;
    }

    /// <summary>
    /// Sets the requested <see cref="bitIndex"/> value to the byte at <see cref="offset"/>.
    /// </summary>
    /// <param name="arr">Buffer to modify</param>
    /// <param name="offset">Offset of the byte</param>
    /// <param name="bitIndex">Bit to write</param>
    /// <param name="value">Bit flag value to set</param>
    public static void SetFlag(Span<byte> arr, int offset, int bitIndex, bool value)
    {
        bitIndex &= 7; // ensure bit access is 0-7
        var current = arr[offset] & ~(1 << bitIndex);
        var newValue = current | ((value ? 1 : 0) << bitIndex);
        arr[offset] = (byte)newValue;
    }

    public static bool[] GitBitFlagArray(ReadOnlySpan<byte> data, int count)
    {
        var result = new bool[count];
        for (int i = 0; i < count; i++)
            result[i] = (data[i >> 3] & (1 << (i & 7))) != 0;
        return result;
    }

    public static bool[] GitBitFlagArray(ReadOnlySpan<byte> data) => GitBitFlagArray(data, data.Length << 3);

    public static void SetBitFlagArray(Span<byte> data, ReadOnlySpan<bool> value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            var ofs = i >> 3;
            var mask = (1 << (i & 7));
            if (value[i])
                data[ofs] |= (byte)mask;
            else
                data[ofs] &= (byte)~mask;
        }
    }
}
