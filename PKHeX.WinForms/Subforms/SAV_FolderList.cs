using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_FolderList : Form
{
    private readonly Action<SaveFile> OpenSaveFile;
    private readonly List<INamedFolderPath> Paths;
    private readonly SortableBindingList<SavePreview> Recent;
    private readonly SortableBindingList<SavePreview> Backup;
    private readonly List<Label> TempTranslationLabels = new();

    public SAV_FolderList(Action<SaveFile> openSaveFile)
    {
        InitializeComponent();
        OpenSaveFile = openSaveFile;

        var drives = Environment.GetLogicalDrives();
        Paths = GetPathList(drives);

        dgDataRecent.ContextMenuStrip = GetContextMenu(dgDataRecent);
        dgDataBackup.ContextMenuStrip = GetContextMenu(dgDataBackup);

        var extra = Paths.Select(z => z.Path).Where(z => z != Main.BackupPath).Distinct();
        var recent = SaveFinder.GetSaveFiles(drives, false, extra, true).ToList();
        var loaded = Main.Settings.Startup.RecentlyLoaded
            .Where(z => recent.All(x => x.Metadata.FilePath != z))
            .Where(File.Exists).Select(SaveUtil.GetVariantSAV).Where(z => z is not null);
        recent.InsertRange(0, loaded!);
        Recent = PopulateData(dgDataRecent, recent);
        var backup = SaveFinder.GetSaveFiles(drives, false, new [] {Main.BackupPath}, false);
        Backup = PopulateData(dgDataBackup, backup);

        CB_FilterColumn.Items.Add(MsgAny);
        var dgv = Recent.Count >= 1 ? dgDataRecent : dgDataBackup;
        int count = dgv.ColumnCount;
        for (int i = 0; i < count; i++)
        {
            var text = dgv.Columns[i].HeaderText;
            CB_FilterColumn.Items.Add(text);
            var tempLabel = new Label {Name = "DGV_" + text, Text = text, Visible = false};
            Controls.Add(tempLabel);
            TempTranslationLabels.Add(tempLabel);
        }
        CB_FilterColumn.SelectedIndex = 0;

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        // Update Translated headers
        for (int i = 0; i < TempTranslationLabels.Count; i++)
        {
            var text = TempTranslationLabels[i].Text;
            if (i < dgDataRecent.ColumnCount)
                dgDataRecent.Columns[i].HeaderText = text;
            if (i < dgDataBackup.ColumnCount)
                dgDataBackup.Columns[i].HeaderText = text;
            CB_FilterColumn.Items[i+1] = text;
        }

        // Preprogrammed folders
        foreach (var loc in Paths)
            AddButton(loc.DisplayText, loc.Path);

        CenterToParent();
    }

    private static List<INamedFolderPath> GetPathList(IReadOnlyList<string> drives)
    {
        var locs = new List<INamedFolderPath>
        {
            new CustomFolderPath(Main.BackupPath, "PKHeX Backups"),
        };
        locs.AddRange(GetUserPaths());
        locs.AddRange(GetConsolePaths(drives));
        locs.AddRange(GetSwitchPaths(drives));
        return locs.DistinctBy(z => z.Path)
            .OrderByDescending(z => Directory.Exists(z.Path)).ToList();
    }

    private const int ButtonHeight = 40;
    private const int ButtonWidth = 130;

    private void AddButton(string name, string path)
    {
        Button button = GetCustomButton(name);
        button.Enabled = Directory.Exists(path);
        button.Click += (_, _) =>
        {
            if (!Directory.Exists(path))
            {
                WinFormsUtil.Alert(MsgFolderNotFound, path);
                return;
            }
            Process.Start("explorer.exe", path);
            Close();
        };
        FLP_Buttons.Controls.Add(button);

        var hover = new ToolTip {AutoPopDelay = 30_000};
        button.MouseHover += (_, _) => hover.Show(path, button);
    }

    private static Button GetCustomButton(string name) => new()
    {
        Size = new Size { Height = ButtonHeight, Width = ButtonWidth },
        Text = name,
        Name = $"B_{name}",
    };

    private static IEnumerable<CustomFolderPath> GetUserPaths()
    {
        var paths = Main.Settings.Backup.OtherBackupPaths;
        return paths.Select(x => new CustomFolderPath(x, true));
    }

    private static IEnumerable<CustomFolderPath> GetConsolePaths(IEnumerable<string> drives)
    {
        var path3DS = SaveFinder.Get3DSLocation(drives);
        if (path3DS == null)
            return Array.Empty<CustomFolderPath>();

        var root = Path.GetPathRoot(path3DS);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable once HeuristicUnreachableCode
        if (root == null)
            return Array.Empty<CustomFolderPath>();

        var paths = SaveFinder.Get3DSBackupPaths(root);
        return paths.Select(z => new CustomFolderPath(z));
    }

    private static IEnumerable<CustomFolderPath> GetSwitchPaths(IEnumerable<string> drives)
    {
        var pathNX = SaveFinder.GetSwitchLocation(drives);
        if (pathNX == null)
            return Array.Empty<CustomFolderPath>();

        var root = Path.GetPathRoot(pathNX);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable once HeuristicUnreachableCode
        if (root == null)
            return Array.Empty<CustomFolderPath>();

        var paths = SaveFinder.GetSwitchBackupPaths(root);
        return paths.Select(z => new CustomFolderPath(z));
    }

    private sealed class CustomFolderPath : INamedFolderPath
    {
        public string Path { get; }
        public string DisplayText { get; }
        public bool Custom { get; }

        public CustomFolderPath(string z, bool custom = false)
        {
            var di = new DirectoryInfo(z);
            var root = di.Root.Name;
            var folder = di.Parent?.Name ?? di.Name;
            if (root == folder)
                folder = di.Name;

            Path = z;
            DisplayText = folder;
            Custom = custom;
        }

        public CustomFolderPath(string path, string display, bool custom = false)
        {
            Path = path;
            DisplayText = display;
            Custom = custom;
        }

        public override string ToString() => $"{DisplayText}\t{Path}";
    }

    private sealed class SaveList<T> : SortableBindingList<T> where T : class { }

    private ContextMenuStrip GetContextMenu(DataGridView dgv)
    {
        var mnuOpen = new ToolStripMenuItem
        {
            Name = "mnuOpen",
            Text = "Open",
            Image = Resources.open,
        };
        mnuOpen.Click += (_, _) => ClickOpenFile(dgv);

        var mnuBrowseAt = new ToolStripMenuItem
        {
            Name = "mnuBrowseAt",
            Text = "Browse...",
            Image = Resources.folder,
        };
        mnuBrowseAt.Click += (_, _) => ClickOpenFolder(dgv);

        ContextMenuStrip mnu = new();
        mnu.Items.Add(mnuOpen);
        mnu.Items.Add(mnuBrowseAt);
        return mnu;
    }

    private void ClickOpenFile(DataGridView dgv)
    {
        var sav = GetSaveFile(dgv);
        if (sav == null || !File.Exists(sav.FilePath))
        {
            WinFormsUtil.Alert(MsgFileLoadFail);
            return;
        }

        OpenSaveFile(sav.Save);
    }

    private void ClickOpenFolder(DataGridView dgv)
    {
        var sav = GetSaveFile(dgv);
        if (sav == null || !File.Exists(sav.FilePath))
        {
            WinFormsUtil.Alert(MsgFileLoadFail);
            return;
        }

        var path = sav.Save.Metadata.FilePath;
        Process.Start("explorer.exe", $"/select, \"{path}\"");
    }

    private SavePreview? GetSaveFile(DataGridView dgData)
    {
        var c = dgData.SelectedCells;
        if (c.Count != 1)
            return null;

        var item = c[0].RowIndex;
        var parent = dgData == dgDataRecent ? Recent : Backup;
        return parent[item];
    }

    private void DataGridCellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex == -1 || e.RowIndex == -1 || e.Button != MouseButtons.Right)
            return;
        var dgv = (DataGridView)sender;
        var c = dgv[e.ColumnIndex, e.RowIndex];
        dgv.ClearSelection();
        dgv.CurrentCell = c;
        c.Selected = true;
    }

    private SaveList<SavePreview> PopulateData(DataGridView dgData, IEnumerable<SaveFile> saves)
    {
        var list = new SaveList<SavePreview>();

        var enumerator = saves.GetEnumerator();
        while (enumerator.Current == null)
        {
            if (!enumerator.MoveNext())
                return list;
        }

        var first = enumerator.Current;
        var sav1 = new SavePreview(first, Paths);
        LoadEntryInitial(dgData, list, sav1);

        int ctr = 1; // refresh every 7 until 15+ are loaded

        void RefreshResize() => Refresh(dgData);
        Task.Run(async () => // load the rest async
        {
            while (!dgData.IsHandleCreated)
                await Task.Delay(15).ConfigureAwait(false);
            while (enumerator.MoveNext())
            {
                var next = enumerator.Current;
                var sav = new SavePreview(next, Paths);
                void Load() => LoadEntry(dgData, list, sav);

                dgData.Invoke(Load);
                ctr++;
                if (ctr < 15 && ctr % 7 == 0)
                    dgData.Invoke(RefreshResize);
            }
            dgData.Invoke(RefreshResize);
            enumerator.Dispose();
        });

        return list;
    }

    private static void Refresh(DataGridView dgData)
    {
        dgData.SuspendLayout();
        dgData.AutoResizeColumns();
        dgData.ResumeLayout(true);
    }

    private static void LoadEntryInitial(DataGridView dgData, ICollection<SavePreview> list, SavePreview sav)
    {
        list.Add(sav);
        dgData.DataSource = list;
        dgData.AutoGenerateColumns = true;
        for (int i = 0; i < dgData.Columns.Count; i++)
            dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
        dgData.AutoResizeColumns(); // Trigger Resizing
    }

    private void LoadEntry(DataGridView dgData, ICollection<SavePreview> list, SavePreview sav)
    {
        list.Add(sav);
        int count = list.Count;
        if (CB_FilterColumn.SelectedIndex != 0)
            ToggleRowVisibility(dgData, CB_FilterColumn.SelectedIndex - 1, TB_FilterTextContains.Text, count - 1);
    }

    private void ChangeFilterIndex(object sender, EventArgs e)
    {
        TB_FilterTextContains.Enabled = CB_FilterColumn.SelectedIndex != 0;
        SetRowFilter();
    }

    private void ChangeFilterText(object sender, EventArgs e)
    {
        if (CB_FilterColumn.SelectedIndex != 0)
            SetRowFilter();
    }

    private void SetRowFilter()
    {
        GetFilterText(dgDataRecent);
        GetFilterText(dgDataBackup);
    }

    private void GetFilterText(DataGridView dg)
    {
        if (dg.RowCount == 0)
            return;
        var cm = (CurrencyManager?)BindingContext?[dg.DataSource];
        cm?.SuspendBinding();
        int column = CB_FilterColumn.SelectedIndex - 1;
        var text = TB_FilterTextContains.Text;

        for (int i = 0; i < dg.RowCount; i++)
            ToggleRowVisibility(dg, column, text, i);
        cm?.ResumeBinding();
    }

    private static void ToggleRowVisibility(DataGridView dg, int column, string text, int rowIndex)
    {
        var row = dg.Rows[rowIndex];
        if (text.Length == 0 || column < 0)
        {
            row.Visible = true;
            return;
        }
        var cell = row.Cells[column];
        var value = cell.Value?.ToString();
        if (value == null)
        {
            row.Visible = false;
            return;
        }
        row.Visible = value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0; // case insensitive contains
    }
}
