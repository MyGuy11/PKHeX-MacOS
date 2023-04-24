using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls;

public partial class ContextMenuSAV : UserControl
{
    public ContextMenuSAV() => InitializeComponent();

    public SaveDataEditor<PictureBox> Editor { private get; set; } = null!;
    public required SlotChangeManager Manager { get; init; }

    public Action<LegalityAnalysis>? RequestEditorLegality;

    public void OmniClick(object sender, EventArgs e, Keys z)
    {
        switch (z)
        {
            case Keys.Control: ClickView(sender, e); break;
            case Keys.Shift: ClickSet(sender, e); break;
            case Keys.Alt: ClickDelete(sender, e); break;
            default:
                return;
        }

        // restart hovering since the mouse event isn't fired
        Manager.MouseEnter(sender, e);
    }

    private void ClickView(object sender, EventArgs e)
    {
        var info = GetSenderInfo(ref sender);
        if ((sender as PictureBox)?.Image == null)
        { System.Media.SystemSounds.Asterisk.Play(); return; }

        Manager.Hover.Stop();
        var pk = Editor.Slots.Get(info.Slot);
        Editor.PKMEditor.PopulateFields(pk, false, true);
    }

    private void ClickSet(object sender, EventArgs e)
    {
        var editor = Editor.PKMEditor;
        if (!editor.EditsComplete)
            return;
        PKM pk = editor.PreparePKM();

        var info = GetSenderInfo(ref sender);
        var sav = info.View.SAV;

        if (!CheckDest(info, sav, pk))
            return;

        var errata = sav.EvaluateCompatibility(pk);
        if (errata.Count > 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Join(Environment.NewLine, errata), MsgContinue))
            return;

        Manager.Hover.Stop();
        Editor.Slots.Set(info.Slot, pk);
        Manager.SE.UpdateUndoRedo();
    }

    private void ClickDelete(object sender, EventArgs e)
    {
        var info = GetSenderInfo(ref sender);
        if ((sender as PictureBox)?.Image == null)
        { System.Media.SystemSounds.Asterisk.Play(); return; }

        var sav = info.View.SAV;
        var pk = sav.BlankPKM;
        if (!CheckDest(info, sav, pk))
            return;

        Manager.Hover.Stop();
        Editor.Slots.Delete(info.Slot);
        Manager.SE.UpdateUndoRedo();
    }

    private static bool CheckDest(SlotViewInfo<PictureBox> info, SaveFile sav, PKM pk)
    {
        var msg = info.Slot.CanWriteTo(sav, pk);
        if (msg == WriteBlockedMessage.None)
            return true;

        switch (msg)
        {
            case WriteBlockedMessage.InvalidPartyConfiguration:
                WinFormsUtil.Alert(MsgSaveSlotEmpty);
                break;
            case WriteBlockedMessage.IncompatibleFormat:
                break;
            case WriteBlockedMessage.InvalidDestination:
                WinFormsUtil.Alert(MsgSaveSlotLocked);
                break;
            default:
                throw new IndexOutOfRangeException(nameof(msg));
        }
        return false;
    }

    private void ClickShowLegality(object sender, EventArgs e)
    {
        var info = GetSenderInfo(ref sender);
        var sav = info.View.SAV;
        var pk = info.Slot.Read(sav);
        var type = info.Slot is SlotInfoBox ? SlotOrigin.Box : SlotOrigin.Party;
        var la = new LegalityAnalysis(pk, sav.Personal, type);
        RequestEditorLegality?.Invoke(la);
    }

    private void MenuOpening(object sender, CancelEventArgs e)
    {
        var items = ((ContextMenuStrip)sender).Items;

        object? ctrl = ((ContextMenuStrip)sender).SourceControl;
        if (ctrl is null)
            return;
        var info = GetSenderInfo(ref ctrl);
        bool SlotFull = (ctrl as PictureBox)?.Image != null;
        bool Editable = info.Slot.CanWriteTo(info.View.SAV);
        bool legality = ModifierKeys == Keys.Control;
        ToggleItem(items, mnuSet, Editable);
        ToggleItem(items, mnuDelete, Editable && SlotFull);
        ToggleItem(items, mnuLegality, legality && SlotFull && RequestEditorLegality != null);
        ToggleItem(items, mnuView, SlotFull || !Editable, true);

        if (items.Count == 0)
            e.Cancel = true;
    }

    private static SlotViewInfo<PictureBox> GetSenderInfo(ref object sender)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        if (pb == null)
            throw new InvalidCastException("Unable to find PictureBox");
        var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
        if (view == null)
            throw new InvalidCastException("Unable to find View Parent");
        var loc = view.GetSlotData(pb);
        sender = pb;
        return new SlotViewInfo<PictureBox>(loc, view);
    }

    private static void ToggleItem(ToolStripItemCollection items, ToolStripItem item, bool visible, bool first = false)
    {
        if (visible)
        {
            if (first)
                items.Insert(0, item);
            else
                items.Add(item);
        }
        else if (items.Contains(item))
        {
            items.Remove(item);
        }
    }
}
