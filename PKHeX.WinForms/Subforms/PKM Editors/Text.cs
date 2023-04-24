using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class TrashEditor : Form
{
    private readonly SaveFile SAV;

    public TrashEditor(TextBoxBase TB_NN, SaveFile sav) : this(TB_NN, Array.Empty<byte>(), sav) { }

    public TrashEditor(TextBoxBase TB_NN, Span<byte> raw, SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        FinalString = TB_NN.Text;

        editing = true;
        if (raw.Length != 0)
        {
            Raw = FinalBytes = raw.ToArray();
            AddTrashEditing(raw.Length);
        }
        else
        {
            Raw = FinalBytes = Array.Empty<byte>();
        }

        var f = FontUtil.GetPKXFont();
        AddCharEditing(f);
        TB_Text.MaxLength = TB_NN.MaxLength;
        TB_Text.Text = TB_NN.Text;
        TB_Text.Font = f;

        if (FLP_Characters.Controls.Count == 0)
        {
            FLP_Characters.Visible = false;
            FLP_Hex.Height *= 2;
        }
        else if (FLP_Hex.Controls.Count == 0)
        {
            FLP_Characters.Location = FLP_Hex.Location;
            FLP_Characters.Height *= 2;
        }

        editing = false;
        CenterToParent();
    }

    private readonly List<NumericUpDown> Bytes = new();
    public string FinalString;
    public byte[] FinalBytes;
    private readonly byte[] Raw;
    private bool editing;
    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        FinalString = TB_Text.Text;
        if (FinalBytes.Length == 0)
            FinalBytes = Raw;
        Close();
    }

    private void AddCharEditing(Font f)
    {
        var chars = GetChars(SAV.Generation);
        if (chars.Length == 0)
            return;

        FLP_Characters.Visible = true;
        foreach (ushort c in chars)
        {
            var l = GetLabel(((char)c).ToString());
            l.Font = f;
            l.AutoSize = false;
            l.Size = new Size(20, 20);
            l.Click += (s, e) => { if (TB_Text.Text.Length < TB_Text.MaxLength) TB_Text.AppendText(l.Text); };
            FLP_Characters.Controls.Add(l);
        }
    }

    private void AddTrashEditing(int count)
    {
        FLP_Hex.Visible = true;
        GB_Trash.Visible = true;
        NUD_Generation.Value = SAV.Generation;
        for (int i = 0; i < count; i++)
        {
            var l = GetLabel($"${i:X2}");
            l.Font = NUD_Generation.Font;
            var n = GetNUD(min: 0, max: 255, hex: true);
            n.Click += (s, e) =>
            {
                switch (ModifierKeys)
                {
                    case Keys.Shift: n.Value = n.Maximum; break;
                    case Keys.Alt: n.Value = n.Minimum; break;
                }
            };
            n.Value = Raw[i];
            n.ValueChanged += (o, args) => UpdateNUD(n, args);

            FLP_Hex.Controls.Add(l);
            FLP_Hex.Controls.Add(n);
            Bytes.Add(n);
        }
        TB_Text.TextChanged += (o, args) => UpdateString(TB_Text, args);

        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource, null);

        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
    }

    private void UpdateNUD(object sender, EventArgs e)
    {
        if (editing)
            return;
        editing = true;
        // build bytes
        if (sender is not NumericUpDown nud)
            throw new Exception();
        int index = Bytes.IndexOf(nud);
        Raw[index] = (byte)nud.Value;

        TB_Text.Text = GetString();
        editing = false;
    }

    private void UpdateString(object sender, EventArgs e)
    {
        if (editing)
            return;
        editing = true;
        // build bytes
        byte[] data = SetString(TB_Text.Text);
        Array.Copy(data, Raw, Math.Min(data.Length, Raw.Length));
        for (int i = 0; i < Raw.Length; i++)
            Bytes[i].Value = Raw[i];
        editing = false;
    }

    private void B_ApplyTrash_Click(object sender, EventArgs e)
    {
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var language = WinFormsUtil.GetIndex(CB_Language);
        var gen = (int)NUD_Generation.Value;
        string speciesName = SpeciesName.GetSpeciesNameGeneration(species, language, gen);

        if (string.IsNullOrEmpty(speciesName)) // no result
            speciesName = CB_Species.Text;

        byte[] current = SetString(TB_Text.Text);
        byte[] data = SetString(speciesName);
        if (data.Length <= current.Length)
        {
            WinFormsUtil.Alert("Trash byte layer is hidden by current text.",
                $"Current Bytes: {current.Length}" + Environment.NewLine + $"Layer Bytes: {data.Length}");
            return;
        }
        if (data.Length > Bytes.Count)
        {
            WinFormsUtil.Alert("Trash byte layer is too long to apply.");
            return;
        }
        for (int i = current.Length; i < data.Length; i++)
            Bytes[i].Value = data[i];
    }

    private void B_ClearTrash_Click(object sender, EventArgs e)
    {
        byte[] current = SetString(TB_Text.Text);
        for (int i = current.Length; i < Bytes.Count; i++)
            Bytes[i].Value = 0;
    }

    private byte[] SetString(ReadOnlySpan<char> text)
    {
        Span<byte> temp = stackalloc byte[Raw.Length];
        var written = SAV.SetString(temp, text, text.Length, StringConverterOption.None);
        return temp[..written].ToArray();
    }

    private string GetString() => SAV.GetString(Raw);

    // Helpers
    private static Label GetLabel(string str) => new() { Text = str, AutoSize = false, Size = new Size(40, 24), TextAlign = ContentAlignment.MiddleRight };

    private static NumericUpDown GetNUD(byte min, byte max, bool hex) => new()
    {
        Maximum = max,
        Minimum = min,
        Hexadecimal = hex,
        Width = 40,
        Padding = new Padding(0),
        Margin = new Padding(0),
    };

    private static ReadOnlySpan<ushort> GetChars(int generation) => generation switch
    {
        6 => SpecialCharsGen67,
        7 => SpecialCharsGen67,
        _ => Array.Empty<ushort>(), // Undocumented
    };

    private static ReadOnlySpan<ushort> SpecialCharsGen67 => new ushort[]
    {
        0xE081, 0xE082, 0xE083, 0xE084, 0xE085, 0xE086, 0xE087, 0xE08D,
        0xE08E, 0xE08F, 0xE090, 0xE091, 0xE092, 0xE093, 0xE094, 0xE095,
        0xE096, 0xE097, 0xE098, 0xE099, 0xE09A, 0xE09B, 0xE09C, 0xE09D,
        0xE09E, 0xE09F, 0xE0A0, 0xE0A1, 0xE0A2, 0xE0A3, 0xE0A4, 0xE0A5,
    };
}
