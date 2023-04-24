using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SwishCrypto"/> block accessor, where blocks are ordered by ascending <see cref="SCBlock.Key"/>.
/// </summary>
public abstract class SCBlockAccessor : ISaveBlockAccessor<SCBlock>
{
    public abstract IReadOnlyList<SCBlock> BlockInfo { get; }

    /// <summary> Checks if there is any <see cref="SCBlock"/> with the requested <see cref="key"/>. </summary>
    public bool HasBlock(uint key) => FindIndex(BlockInfo, key) != -1;

    #region Direct Block Accessing
    /// <summary> Returns the <see cref="SCBlock"/> reference with the corresponding <see cref="key"/>. </summary>
    public SCBlock GetBlock(uint key) => Find(BlockInfo, key);

    /// <inheritdoc cref="SCBlock.GetValue"/>
    public object GetBlockValue(uint key) => GetBlock(key).GetValue();

    /// <inheritdoc cref="SCBlock.GetValue"/>
    public T GetBlockValue<T>(uint key) where T : struct
    {
        var value = GetBlockValue(key);
        if (value is T v)
            return v;
        throw new ArgumentException($"Incorrect type request! Expected {typeof(T).Name}, received {value.GetType().Name}", nameof(T));
    }

    /// <inheritdoc cref="SCBlock.SetValue"/>
    public void SetBlockValue(uint key, object value) => GetBlock(key).SetValue(value);
    #endregion

    #region Ease of Use Overloads
    /// <inheritdoc cref="GetBlock(uint)"/>
    /// <param name="name">Block name (un-hashed)</param>
    public SCBlock GetBlock(ReadOnlySpan<char> name) => GetBlock(Hash(name));
    private static uint Hash(ReadOnlySpan<char> name) => (uint)FnvHash.HashFnv1a_64(name);

    /// <inheritdoc cref="GetBlock(ReadOnlySpan{char})"/>
    public SCBlock GetBlock(ReadOnlySpan<byte> name) => GetBlock(Hash(name));
    private static uint Hash(ReadOnlySpan<byte> name) => (uint)FnvHash.HashFnv1a_64(name);
    #endregion

    #region Safe Block Operations (no exceptions thrown)
    /// <summary>
    /// Tries to grab the actual block, and returns a new dummy if the block does not exist.
    /// </summary>
    /// <param name="key">Block Key</param>
    /// <returns>Block if exists, dummy if not. Dummy key will not match requested key.</returns>
    public SCBlock GetBlockSafe(uint key) => FindOrDefault(BlockInfo, key);

    /// <remarks> If the block does not exist, the method will return the default value. </remarks>
    /// <inheritdoc cref="SCBlock.GetValue"/>
    public T GetBlockValueSafe<T>(uint key) where T : struct
    {
        var index = FindIndex(BlockInfo, key);
        if (index == -1)
            return default;
        var block = BlockInfo[index];
        if (block.Type != SCTypeCode.None) // not fake block
            return (T)block.GetValue();
        return default;
    }

    /// <remarks> If the block does not exist, the method will do nothing. </remarks>
    /// <inheritdoc cref="SCBlock.SetValue"/>
    public void SetBlockValueSafe(uint key, object value)
    {
        var index = FindIndex(BlockInfo, key);
        if (index == -1)
            return;
        var block = BlockInfo[index];
        if (block.Type != SCTypeCode.None) // not fake block
            block.SetValue(value);
    }
    #endregion

    #region Block Fetching
    private static SCBlock Find(IReadOnlyList<SCBlock> array, uint key)
    {
        var index = FindIndex(array, key);
        if (index != -1)
            return array[index];
        throw new KeyNotFoundException(nameof(key));
    }

    private static SCBlock FindOrDefault(IReadOnlyList<SCBlock> array, uint key)
    {
        var index = FindIndex(array, key);
        if (index != -1)
            return array[index];
        return new SCBlock(0, SCTypeCode.None);
    }

    /// <summary>
    /// Finds a specified <see cref="key"/> within the <see cref="array"/>.
    /// </summary>
    /// <remarks>
    /// Rather than storing a dictionary of keys, we can abuse the fact that the <see cref="BlockInfo"/> is stored in order of ascending block key.
    /// <br></br>
    /// Binary Search doesn't require extra memory like a Dictionary would; also, we usually only need to find a few blocks.
    /// </remarks>
    /// <param name="array">Index-able collection</param>
    /// <param name="key"><see cref="SCBlock.Key"/> to find.</param>
    /// <returns>Returns -1 if no match found.</returns>
    private static int FindIndex(IReadOnlyList<SCBlock> array, uint key)
    {
        int min = 0;
        int max = array.Count - 1;
        do
        {
            int mid = min + ((max - min) >> 1);
            var entry = array[mid];
            var ek = entry.Key;
            if (key == ek)
                return mid;

            if (key < ek)
                max = mid - 1;
            else
                min = mid + 1;
        } while (min <= max);
        return -1;
    }
    #endregion
}
