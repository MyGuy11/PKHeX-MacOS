using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class ReportGrid : Form
{
    public ReportGrid()
    {
        InitializeComponent();
        CenterToParent();
        GetContextMenu();
    }

    private void GetContextMenu()
    {
        var mnuHide = new ToolStripMenuItem { Name = "mnuHide", Text = MsgReportColumnHide };
        mnuHide.Click += (sender, e) =>
        {
            int c = dgData.SelectedCells.Count;
            if (c == 0)
            { WinFormsUtil.Alert(MsgReportColumnHideFail); return; }

            for (int i = 0; i < c; i++)
                dgData.Columns[dgData.SelectedCells[i].ColumnIndex].Visible = false;
        };
        var mnuRestore = new ToolStripMenuItem { Name = "mnuRestore", Text = MsgReportColumnRestore };
        mnuRestore.Click += (sender, e) =>
        {
            int c = dgData.ColumnCount;
            for (int i = 0; i < c; i++)
                dgData.Columns[i].Visible = true;

            WinFormsUtil.Alert(MsgReportColumnRestoreSuccess);
        };

        ContextMenuStrip mnu = new();
        mnu.Items.Add(mnuHide);
        mnu.Items.Add(mnuRestore);

        dgData.ContextMenuStrip = mnu;
    }

    private sealed class PokemonList<T> : SortableBindingList<T> where T : class { }

    public void PopulateData(IList<SlotCache> Data)
    {
        SuspendLayout();
        var PL = new PokemonList<EntitySummaryImage>();
        var strings = GameInfo.Strings;
        foreach (var entry in Data)
        {
            var pk = entry.Entity;
            if ((uint)(pk.Species - 1) >= pk.MaxSpeciesID)
            {
                continue;
            }
            pk.Stat_Level = pk.CurrentLevel; // recalc Level
            PL.Add(new EntitySummaryImage(pk, strings, entry.Identify()));
        }

        dgData.DataSource = PL;
        dgData.AutoGenerateColumns = true;
        for (int i = 0; i < dgData.Columns.Count; i++)
        {
            var col = dgData.Columns[i];
            if (col is DataGridViewImageColumn)
                continue; // Don't add sorting for Sprites
            col.SortMode = DataGridViewColumnSortMode.Automatic;
        }

        // Trigger Resizing
        dgData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        for (int i = 0; i < dgData.Columns.Count; i++)
        {
            int w = dgData.Columns[i].Width;
            dgData.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgData.Columns[i].Width = w;
        }
        dgData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        Data_Sorted(this, EventArgs.Empty); // trigger row resizing

        ResumeLayout();
    }

    private void Data_Sorted(object sender, EventArgs e)
    {
        int height = SpriteUtil.Spriter.Height + 1; // max height of a row, +1px
        for (int i = 0; i < dgData.Rows.Count; i++)
            dgData.Rows[i].Height = height;
    }

    private void PromptSaveCSV(object sender, FormClosingEventArgs e)
    {
        if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgReportExportCSV) != DialogResult.Yes)
            return;
        using var savecsv = new SaveFileDialog
        {
            Filter = "Spreadsheet|*.csv",
            FileName = "Box Data Dump.csv",
        };
        if (savecsv.ShowDialog() == DialogResult.OK)
        {
            Hide();
            var path = savecsv.FileName;
            var t = Task.Run(() => Export_CSV(path));
            t.Wait(); // don't start disposing until the saving is complete
        }
    }

    private async Task Export_CSV(string path)
    {
        await using var fs = new FileStream(path, FileMode.Create);
        await using var s = new StreamWriter(fs, System.Text.Encoding.Unicode);

        var headers = dgData.Columns.Cast<DataGridViewColumn>();
        await s.WriteLineAsync(string.Join(",", headers.Skip(1).Select(column => $"\"{column.HeaderText}\""))).ConfigureAwait(false);

        foreach (var cells in dgData.Rows.Cast<DataGridViewRow>().Select(row => row.Cells.Cast<DataGridViewCell>()))
            await s.WriteLineAsync(string.Join(",", cells.Skip(1).Select(cell => $"\"{cell.Value}\""))).ConfigureAwait(false);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool cp = keyData == (Keys.Control | Keys.C) && ActiveControl is DataGridView;
        if (!cp)
            return base.ProcessCmdKey(ref msg, keyData);

        var content = dgData.GetClipboardContent();
        if (content == null)
            return base.ProcessCmdKey(ref msg, keyData);

        string data = content.GetText();
        var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgReportExportTable);
        if (dr != DialogResult.Yes)
        {
            WinFormsUtil.SetClipboardText(data);
            return true;
        }

        // Reformat datagrid clipboard content
        string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        string[] newlines = ConvertTabbedToRedditTable(lines);
        WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, newlines));
        return true;
    }

    private static string[] ConvertTabbedToRedditTable(string[] lines)
    {
        string[] newlines = new string[lines.Length + 1];
        int tabcount = lines[0].Count(c => c == '\t');

        newlines[0] = lines[0].Replace('\t', '|');
        newlines[1] = string.Join(":--:", Enumerable.Repeat('|', tabcount + 2)); // 2 pipes for each end
        for (int i = 1; i < lines.Length; i++)
            newlines[i + 1] = lines[i].Replace('\t', '|');
        return newlines;
    }
}
