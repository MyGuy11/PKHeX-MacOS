using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class FakeSaveFile : SaveFile
{
    public static readonly FakeSaveFile Default = new();
    protected internal override string ShortSummary => "Fake Save File";
    protected override FakeSaveFile CloneInternal() => this;
    public override string Extension => string.Empty;
    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    public override int Generation => 3;
    public override string GetString(ReadOnlySpan<byte> data) => string.Empty;
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option) => 0;
    public override PersonalTable3 Personal => PersonalTable.RS;
    public override int MaxEV => 0;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;
    public override int GetBoxOffset(int box) => -1;
    public override string GetBoxName(int box) => $"Box {box:00}";
    public override void SetBoxName(int box, ReadOnlySpan<char> value) { }
    public override int MaxStringLengthOT => 5;
    public override int MaxStringLengthNickname => 5;
    public override ushort MaxMoveID => 5;
    public override ushort MaxSpeciesID => 1;
    public override int MaxItemID => 5;
    public override int MaxBallID => 5;
    public override int MaxGameID => 5;
    public override int MaxAbilityID => 0;
    public override int BoxCount => 1;
    public override int GetPartyOffset(int slot) => -1;
    protected override void SetChecksums() { }

    public override Type PKMType => typeof(PK3);
    protected override PK3 GetPKM(byte[] data) => BlankPKM;
    protected override byte[] DecryptPKM(byte[] data) => data;
    public override PK3 BlankPKM => new();
    public override EntityContext Context => EntityContext.Gen3;
    protected override int SIZE_STORED => 0;
    protected override int SIZE_PARTY => 0;
}
