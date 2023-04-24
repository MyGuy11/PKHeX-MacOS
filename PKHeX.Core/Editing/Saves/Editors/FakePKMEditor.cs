﻿namespace PKHeX.Core;

/// <summary>
/// Fakes the <see cref="IPKMView"/> interface interactions.
/// </summary>
public sealed class FakePKMEditor : IPKMView
{
    public FakePKMEditor(PKM template) => Data = template;

    public PKM Data { get; private set; }
    public bool Unicode => true;
    public bool HaX => false;
    public bool ChangingFields { get; set; }
    public bool EditsComplete => true;

    public PKM PreparePKM(bool click = true) => Data;
    public void PopulateFields(PKM pk, bool focus = true, bool skipConversionCheck = false) => Data = pk;
}
