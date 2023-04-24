using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_Raid9 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;
    private readonly RaidSpawnList9 Raids;

    public SAV_Raid9(SaveFile sav, RaidSpawnList9 raid)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();
        Raids = raid;
        CB_Raid.Items.AddRange(Enumerable.Range(1, raid.CountUsed).Select(z => (object)$"Raid {z:000}").ToArray());
        CB_Raid.SelectedIndex = 0;

        TB_SeedToday.Text = Raids.CurrentSeed.ToString("X16");
        TB_SeedTomorrow.Text = Raids.TomorrowSeed.ToString("X16");

        TB_SeedToday.Validated += UpdateStringSeed;
        TB_SeedTomorrow.Validated += UpdateStringSeed;
    }

    private void LoadRaid(int index) => PG_Raid.SelectedObject = Raids.GetRaid(index);

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        // We've been editing the original save file blocks. Restore the clone's data.
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        ValidateChildren();
        Validate();
        Close();
    }

    private void CB_Raid_SelectedIndexChanged(object sender, EventArgs e) => LoadRaid(CB_Raid.SelectedIndex);

    private void UpdateStringSeed(object? sender, EventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        if (string.IsNullOrWhiteSpace(tb.Text))
        {
            tb.Undo();
            return;
        }

        string filterText = Util.GetOnlyHex(tb.Text);
        if (string.IsNullOrWhiteSpace(filterText) || filterText.Length != tb.Text.Length)
        {
            WinFormsUtil.Alert(MsgProgramErrorExpectedHex, tb.Text);
            tb.Undo();
            return;
        }

        // Write final value back to the save
        var value = ulong.Parse(tb.Text, System.Globalization.NumberStyles.HexNumber);
        if (tb == TB_SeedToday)
            Raids.CurrentSeed = value;
        else if (tb == TB_SeedTomorrow)
            Raids.TomorrowSeed = value;
    }

    private void B_CopyToOthers_Click(object sender, EventArgs e)
    {
        Raids.Propagate(CB_Raid.SelectedIndex, seedToo: ModifierKeys == Keys.Shift);
        System.Media.SystemSounds.Asterisk.Play();
    }
}
