﻿using System;

namespace PKHeX.Core;

/// <summary>
/// Clears contents of boxes by deleting all that satisfy a <see cref="Criteria"/> based on a <see cref="SaveFile"/>.
/// </summary>
public sealed class BoxManipClearComplex : BoxManipBase
{
    private readonly Func<PKM, SaveFile, bool> Criteria;
    public BoxManipClearComplex(BoxManipType type, Func<PKM, SaveFile, bool> criteria) : this(type, criteria, _ => true) { }
    public BoxManipClearComplex(BoxManipType type, Func<PKM, SaveFile, bool> criteria, Func<SaveFile, bool> usable) : base(type, usable) => Criteria = criteria;

    public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxClearAll : MessageStrings.MsgSaveBoxClearCurrent;
    public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxClearAllFailBattle : MessageStrings.MsgSaveBoxClearCurrentFailBattle;
    public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxClearAllSuccess : MessageStrings.MsgSaveBoxClearCurrentSuccess;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        var (start, stop, reverse) = param;
        bool Method(PKM p) => reverse ^ Criteria(p, sav);
        return sav.ClearBoxes(start, stop, Method);
    }
}
