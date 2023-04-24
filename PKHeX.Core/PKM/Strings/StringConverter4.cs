using System;
using static PKHeX.Core.StringConverter4Util;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 4.
/// </summary>
public static class StringConverter4
{
    private const ushort Terminator = 0xFFFF;
    private const char TerminatorChar = (char)Terminator;

    /// <summary>Converts Generation 4 encoded data to decoded string.</summary>
    /// <param name="data">Encoded data</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        var length = LoadString(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte})"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result)
    {
        int i = 0;
        for (; i < data.Length; i += 2)
        {
            var value = ReadUInt16LittleEndian(data[i..]);
            if (value == Terminator)
                break;
            char chr = (char)ConvertValue2CharG4(value);
            if (chr == TerminatorChar)
                break;
            chr = StringConverter.SanitizeChar(chr);
            result[i/2] = chr;
        }
        return i/2;
    }

    /// <summary>Gets the bytes for a 4th Generation String</summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength,
        StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        for (int i = 0; i < value.Length; i++)
        {
            var chr = value[i];
            chr = StringConverter.UnSanitizeChar5(chr);
            ushort val = ConvertChar2ValueG4(chr);
            WriteUInt16LittleEndian(destBuffer[(i * 2)..], val);
        }

        int count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16LittleEndian(destBuffer[count..], Terminator);
        return count + 2;
    }
}
