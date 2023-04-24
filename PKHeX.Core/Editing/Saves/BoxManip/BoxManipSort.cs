﻿using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Sorts contents of boxes by using a <see cref="Sorter"/> to determine the order.
/// </summary>
public sealed class BoxManipSort : BoxManipBase
{
    private readonly Func<IEnumerable<PKM>, IEnumerable<PKM>> Sorter;
    public BoxManipSort(BoxManipType type, Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter) : this(type, sorter, _ => true) { }
    public BoxManipSort(BoxManipType type, Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter, Func<SaveFile, bool> usable) : base(type, usable) => Sorter = sorter;

    public override string GetPrompt(bool all) => all ? MessageStrings.MsgSaveBoxSortAll : MessageStrings.MsgSaveBoxSortCurrent;
    public override string GetFail(bool all) => all ? MessageStrings.MsgSaveBoxSortAllFailBattle: MessageStrings.MsgSaveBoxSortCurrentFailBattle;
    public override string GetSuccess(bool all) => all ? MessageStrings.MsgSaveBoxSortAllSuccess : MessageStrings.MsgSaveBoxSortCurrentSuccess;

    public override int Execute(SaveFile sav, BoxManipParam param)
    {
        IEnumerable<PKM> Method(IEnumerable<PKM> p, int index) => Sorter(p);
        var (start, stop, reverse) = param;
        return sav.SortBoxes(start, stop, Method, reverse);
    }
}
