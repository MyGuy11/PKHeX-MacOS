using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_PokedexGG : Form
{
    private readonly SaveFile Origin;
    private readonly SAV7b SAV;

    public SAV_PokedexGG(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV7b)(Origin = sav).Clone();
        CP = new[] { CHK_P1, CHK_P2, CHK_P3, CHK_P4, CHK_P5, CHK_P6, CHK_P7, CHK_P8, CHK_P9 };
        CL = new[] { CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, CHK_L8, CHK_L9 };

        editing = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species.Skip(1).ToList(), null);

        Dex = SAV.Blocks.Zukan;

        var Species = GameInfo.Strings.Species;
        var names = Dex.GetEntryNames(Species);
        foreach (var n in names)
            LB_Species.Items.Add(n);

        RecordUsed = new[] { CHK_RMinHeight, CHK_RMaxHeight, CHK_RMinWeight, CHK_RMaxWeight };
        RecordHeight = new[] { NUD_RHeightMin, NUD_RHeightMax, NUD_RWeightMinHeight, NUD_RWeightMaxHeight};
        RecordWeight = new[] { NUD_RHeightMinWeight, NUD_RHeightMaxWeight, NUD_RWeightMin, NUD_RWeightMax};

        editing = false;
        LB_Species.SelectedIndex = 0;
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
    }

    private readonly Zukan7b Dex;
    private bool editing;
    private bool allModifying;
    private ushort currentSpecies = ushort.MaxValue;
    private readonly CheckBox[] CP, CL;

    private readonly CheckBox[] RecordUsed;
    private readonly NumericUpDown[] RecordHeight, RecordWeight;

    private void B_Counts_Click(object sender, EventArgs e)
    {
        SetEntry();
        new SAV_Capture7GG(SAV).ShowDialog();
    }

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetEntry();

        editing = true;
        currentSpecies = (ushort)WinFormsUtil.GetIndex(CB_Species);
        LB_Species.SelectedIndex = currentSpecies - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
        LB_Species.TopIndex = LB_Species.SelectedIndex;
        if (!allModifying)
            FillLBForms();
        GetEntry();
        editing = false;
    }

    private void ChangeLBSpecies(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetEntry();

        editing = true;
        currentSpecies = (ushort)(LB_Species.SelectedIndex + 1);
        CB_Species.SelectedValue = (int)currentSpecies;
        if (!allModifying) FillLBForms();
        GetEntry();
        editing = false;
    }

    private void ChangeLBForms(object sender, EventArgs e)
    {
        if (allModifying)
            return;
        if (editing)
            return;
        SetEntry();

        editing = true;
        var fspecies = (ushort)(LB_Species.SelectedIndex + 1);
        var bspecies = Dex.GetBaseSpecies(fspecies);
        int form = LB_Forms.SelectedIndex;
        if (form > 0)
        {
            var fc = SAV.Personal[bspecies].FormCount;
            if (fc > 1) // actually has forms
            {
                int f = Dex.DexFormIndexFetcher(bspecies, fc, SAV.MaxSpeciesID - 1);
                if (f >= 0) // bit index valid
                    currentSpecies = (ushort)(f + form + 1);
                else
                    currentSpecies = bspecies;
            }
            else
            {
                currentSpecies = bspecies;
            }
        }
        else
        {
            currentSpecies = bspecies;
        }

        CB_Species.SelectedValue = (int)currentSpecies;
        LB_Species.SelectedIndex = currentSpecies - 1;
        LB_Species.TopIndex = LB_Species.SelectedIndex;
        GetEntry();
        editing = false;
    }

    private bool FillLBForms()
    {
        if (allModifying)
            return false;
        LB_Forms.DataSource = null;
        LB_Forms.Items.Clear();

        var fspecies = (ushort)(LB_Species.SelectedIndex + 1);
        var bspecies = Dex.GetBaseSpecies(fspecies);
        bool hasForms = FormInfo.HasFormSelection(SAV.Personal[bspecies], bspecies, 7);
        LB_Forms.Enabled = hasForms;
        if (!hasForms)
            return false;
        var ds = FormConverter.GetFormList(bspecies, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context).ToList();
        if (ds.Count == 1 && string.IsNullOrEmpty(ds[0]))
        {
            // empty
            LB_Forms.Enabled = false;
            return false;
        }

        // sanity check forms -- SM does not have totem form dex bits
        int count = SAV.Personal[bspecies].FormCount;
        if (count < ds.Count)
            ds.RemoveAt(count); // remove last

        LB_Forms.DataSource = ds;
        if (fspecies <= SAV.MaxSpeciesID)
        {
            LB_Forms.SelectedIndex = 0;
        }
        else
        {
            var fc = SAV.Personal[bspecies].FormCount;
            if (fc <= 1)
                return true;

            int f = Dex.DexFormIndexFetcher(bspecies, fc, SAV.MaxSpeciesID - 1);
            if (f < 0)
                return true; // bit index valid

            if (f > fspecies - LB_Forms.Items.Count - 1)
                LB_Forms.SelectedIndex = fspecies - f - 1;
            else
                LB_Forms.SelectedIndex = -1;
        }
        return true;
    }

    private void ChangeDisplayed(object sender, EventArgs e)
    {
        if (!((CheckBox) sender).Checked)
            return;

        CHK_P6.Checked = sender == CHK_P6;
        CHK_P7.Checked = sender == CHK_P7;
        CHK_P8.Checked = sender == CHK_P8;
        CHK_P9.Checked = sender == CHK_P9;

        CHK_P2.Checked |= CHK_P6.Checked;
        CHK_P3.Checked |= CHK_P7.Checked;
        CHK_P4.Checked |= CHK_P8.Checked;
        CHK_P5.Checked |= CHK_P9.Checked;
    }

    private void ChangeEncountered(object sender, EventArgs e)
    {
        if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked))
        {
            CHK_P6.Checked = CHK_P7.Checked = CHK_P8.Checked = CHK_P9.Checked = false;
        }
        else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
        {
            if (sender == CHK_P2 && CHK_P2.Checked)
                CHK_P6.Checked = true;
            else if (sender == CHK_P3 && CHK_P3.Checked)
                CHK_P7.Checked = true;
            else if (sender == CHK_P4 && CHK_P4.Checked)
                CHK_P8.Checked = true;
            else if (sender == CHK_P5 && CHK_P5.Checked)
                CHK_P9.Checked = true;
        }
    }

    private void GetEntry()
    {
        var pk = (ushort)(currentSpecies - 1);
        editing = true;
        CHK_P1.Enabled = currentSpecies <= SAV.MaxSpeciesID;
        CHK_P1.Checked = CHK_P1.Enabled && Dex.GetCaught(currentSpecies);

        var gt = Dex.GetBaseSpeciesGenderValue(LB_Species.SelectedIndex);

        bool canBeMale = gt != PersonalInfo.RatioMagicFemale;
        bool canBeFemale = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);
        CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = canBeMale; // Not Female-Only
        CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = canBeFemale; // Not Male-Only and Not Genderless

        for (int i = 0; i < 4; i++)
            CP[i + 1].Checked = Dex.GetSeen(currentSpecies, i);

        for (int i = 0; i < 4; i++)
            CP[i + 5].Checked = Dex.GetDisplayed(currentSpecies - 1, i);

        for (int i = 0; i < 9; i++)
        {
            CL[i].Enabled = currentSpecies <= SAV.MaxSpeciesID;
            CL[i].Checked = CL[i].Enabled && Dex.GetLanguageFlag(pk, i);
        }

        var speciesID = Dex.GetBaseSpecies(currentSpecies);
        LoadRecord(speciesID, (byte)Math.Max(0, LB_Forms.SelectedIndex));

        editing = false;
    }

    private void SetEntry()
    {
        if (currentSpecies == 0)
            return;

        int pk = currentSpecies - 1;

        for (int i = 0; i < 4; i++)
            Dex.SetSeen(currentSpecies, i, CP[i + 1].Checked);

        for (int i = 0; i < 4; i++)
            Dex.SetDisplayed(pk, i, CP[i + 5].Checked);

        if (currentSpecies > SAV.MaxSpeciesID)
            return;

        Dex.SetCaught(currentSpecies, CHK_P1.Checked);

        for (int i = 0; i < 9; i++)
            Dex.SetLanguageFlag(pk, i, CL[i].Checked);

        var speciesID = Dex.GetBaseSpecies(currentSpecies);
        SetRecord(speciesID, (byte)Math.Max(0, LB_Forms.SelectedIndex));
    }

    private void LoadRecord(ushort species, byte form)
    {
        bool hasRecord = Zukan7b.TryGetSizeEntryIndex(species, form, out var index);
        GB_SizeRecords.Visible = hasRecord;
        if (!hasRecord)
            return;

        void set(DexSizeType type, NumericUpDown nudH, NumericUpDown nudW, CheckBox ck)
        {
            nudH.Enabled = nudW.Enabled = ck.Checked = Dex.GetSizeData(type, index, out byte h, out byte w);
            nudH.Value = h;
            nudW.Value = w;
        }
        set(DexSizeType.MinHeight, NUD_RHeightMin, NUD_RHeightMinWeight, CHK_RMinHeight);
        set(DexSizeType.MaxHeight, NUD_RHeightMax, NUD_RHeightMaxWeight, CHK_RMaxHeight);
        set(DexSizeType.MinWeight, NUD_RWeightMinHeight, NUD_RWeightMin, CHK_RMinWeight);
        set(DexSizeType.MaxWeight, NUD_RWeightMaxHeight, NUD_RWeightMax, CHK_RMaxWeight);
    }

    private void SetRecord(ushort species, byte form)
    {
        bool hasRecord = Zukan7b.TryGetSizeEntryIndex(species, form, out var index);
        if (!hasRecord)
            return;

        static byte get(NumericUpDown nud, CheckBox ck) => !ck.Checked ? Zukan7b.DefaultEntryValue : (byte)nud.Value;

        Dex.SetSizeData(DexSizeType.MinHeight, index, get(NUD_RHeightMin, CHK_RMinHeight), get(NUD_RHeightMinWeight, CHK_RMinHeight));
        Dex.SetSizeData(DexSizeType.MaxHeight, index, get(NUD_RHeightMax, CHK_RMaxHeight), get(NUD_RHeightMaxWeight, CHK_RMaxHeight));
        Dex.SetSizeData(DexSizeType.MinWeight, index, get(NUD_RWeightMinHeight, CHK_RMinWeight), get(NUD_RWeightMin, CHK_RMinWeight));
        Dex.SetSizeData(DexSizeType.MaxWeight, index, get(NUD_RWeightMaxHeight, CHK_RMaxWeight), get(NUD_RWeightMax, CHK_RMaxWeight));
    }

    private void CHK_RUsed_CheckedChanged(object sender, EventArgs e)
    {
        var ck = (CheckBox) sender;
        int index = Array.IndexOf(RecordUsed, ck);
        var h = RecordHeight[index];
        var w = RecordWeight[index];

        h.Enabled = w.Enabled = ck.Checked;
        if (!editing && !ck.Checked)
            h.Value = w.Value = Zukan7b.DefaultEntryValue;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        if (CHK_L1.Enabled)
        {
            CHK_L1.Checked =
                CHK_L2.Checked =
                    CHK_L3.Checked =
                        CHK_L4.Checked =
                            CHK_L5.Checked =
                                CHK_L6.Checked =
                                    CHK_L7.Checked =
                                        CHK_L8.Checked =
                                            CHK_L9.Checked = ModifierKeys != Keys.Control;
        }
        if (CHK_P1.Enabled)
        {
            CHK_P1.Checked = ModifierKeys != Keys.Control;
        }
        byte gt = Dex.GetBaseSpeciesGenderValue(LB_Species.SelectedIndex);

        bool canBeMale = gt != PersonalInfo.RatioMagicFemale;
        bool canBeFemale = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);
        CHK_P2.Checked = CHK_P4.Checked = canBeMale && ModifierKeys != Keys.Control;
        CHK_P3.Checked = CHK_P5.Checked = canBeFemale && ModifierKeys != Keys.Control;

        if (ModifierKeys == Keys.Control)
        {
            foreach (var chk in new[] { CHK_P6, CHK_P7, CHK_P8, CHK_P9 })
                chk.Checked = false;
        }
        else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
        {
            (gt != PersonalInfo.RatioMagicFemale ? CHK_P6 : CHK_P7).Checked = true;
        }
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void ModifyAll(object sender, EventArgs e)
    {
        allModifying = true;
        LB_Forms.Enabled = LB_Forms.Visible = false;
        int lang = SAV.Language;
        if (lang > 5) lang--;
        lang--;

        if (sender == mnuSeenAll || sender == mnuCaughtAll || sender == mnuComplete)
            SetAll(sender, lang);
        else
            ClearAll(sender);

        SetEntry();
        GetEntry();
        allModifying = false;
        LB_Species.SelectedIndex = 0;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void ClearAll(object sender)
    {
        for (int i = 0; i < LB_Species.Items.Count; i++)
        {
            LB_Species.SelectedIndex = i;
            foreach (CheckBox chk in CL)
                chk.Checked = false;
            CHK_P1.Checked = false; // not caught
            if (sender == mnuCaughtNone)
                continue;
            // remove seen/displayed
            CHK_P2.Checked = CHK_P4.Checked = CHK_P3.Checked = CHK_P5.Checked = false;
            CHK_P6.Checked = CHK_P7.Checked = CHK_P8.Checked = CHK_P9.Checked = false;

            foreach (var ck in RecordUsed)
                ck.Checked = false;
        }
    }

    private void SetAll(object sender, int lang)
    {
        var indexes = GetLegalSpecies();
        foreach (var species in indexes)
        {
            int index = species - 1;
            var gt = Dex.GetBaseSpeciesGenderValue(index);
            LB_Species.SelectedIndex = index;
            SetSeen(sender, gt, false);
            if (sender != mnuSeenAll)
                SetCaught(sender, gt, lang, false);

            if (sender != mnuSeenAll)
                SetRecords();

            if (species is (int)Species.Pikachu or (int)Species.Eevee)
                continue; // ignore starter (setdex doesn't set buddy bit; totem raticate is not emitted below).

            // Set form flags
            var entries = Dex.GetAllFormEntries(species).Where(z => z >= SAV.MaxSpeciesID).Distinct();
            foreach (var f in entries)
            {
                LB_Species.SelectedIndex = f;
                SetSeen(sender, gt, true);
                if (sender != mnuSeenAll)
                    SetCaught(sender, gt, lang, true);
            }
        }
    }

    private void SetRecords()
    {
        if (!GB_SizeRecords.Enabled)
            return;
        for (var i = 0; i < RecordUsed.Length; i++)
        {
            var ck = RecordUsed[i];
            if (ck.Checked)
                continue;
            ck.Checked = true;
            RecordHeight[i].Value = i % 2 == 0 ? 0 : 255;
            RecordWeight[i].Value = i % 2 == 0 ? 0 : 255;
        }
    }

    private static IEnumerable<ushort> GetLegalSpecies()
    {
        foreach (var z in Enumerable.Range(1, 151))
            yield return (ushort)z;
        yield return 808;
        yield return 809;
    }

    private void SetCaught(object sender, byte gt, int lang, bool isForm)
    {
        CHK_P1.Checked = mnuCaughtNone != sender;
        for (int j = 0; j < CL.Length; j++)
            CL[j].Checked = CL[j].Enabled && (sender == mnuComplete || (mnuCaughtNone != sender && j == lang));

        if (mnuCaughtNone == sender)
        {
            if (isForm)
                return;
            if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked)) // if seen
            {
                if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked)) // not displayed
                    (gt != PersonalInfo.RatioMagicFemale ? CHK_P6 : CHK_P7).Checked = true; // check one
            }

            return;
        }

        if (mnuComplete == sender)
        {
            // Seen All
            foreach (var chk in new[] { CHK_P2, CHK_P3, CHK_P4, CHK_P5 })
                chk.Checked = chk.Enabled;
        }
        else
        {
            // ensure at least one SEEN
            if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked))
                (gt != PersonalInfo.RatioMagicFemale ? CHK_P2 : CHK_P3).Checked = true;
        }

        // ensure at least one Displayed except for forms
        if (isForm)
            return;
        if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
            (gt != PersonalInfo.RatioMagicFemale ? CHK_P6 : CHK_P7).Checked = CHK_P1.Enabled;
    }

    private void SetSeen(object sender, byte gt, bool isForm)
    {
        foreach (CheckBox t in new[] { CHK_P2, CHK_P3, CHK_P4, CHK_P5 })
            t.Checked = mnuSeenNone != sender && t.Enabled;

        if (mnuSeenNone != sender)
        {
            // ensure at least one Displayed except for forms
            if (isForm)
                return;
            if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                (gt != PersonalInfo.RatioMagicFemale ? CHK_P6 : CHK_P7).Checked = true;
        }
        else
        {
            foreach (CheckBox t in CP)
                t.Checked = false;
        }

        if (!CHK_P1.Checked)
        {
            foreach (CheckBox t in CL)
                t.Checked = false;
        }
    }
}
