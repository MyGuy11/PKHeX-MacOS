using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SettingsEditor : Form
{
    public bool BlankChanged { get; private set; }

    public SettingsEditor(object obj)
    {
        InitializeComponent();
        LoadSettings(obj);

        if (obj is PKHeXSettings s)
        {
            var noSelectVersions = new[] {GameVersion.GO, (GameVersion)0};
            CB_Blank.InitializeBinding();
            CB_Blank.DataSource = GameInfo.VersionDataSource.Where(z => !noSelectVersions.Contains((GameVersion)z.Value)).ToList();
            CB_Blank.SelectedValue = (int) s.Startup.DefaultSaveVersion;
            CB_Blank.SelectedValueChanged += (_, _) => s.Startup.DefaultSaveVersion = (GameVersion)WinFormsUtil.GetIndex(CB_Blank);
            CB_Blank.SelectedIndexChanged += (_, _) => BlankChanged = true;
            B_Reset.Click += (x, e) => DeleteSettings();
        }
        else
        {
            FLP_Blank.Visible = false;
            B_Reset.Visible = false;
        }

        this.CenterToForm(FindForm());
    }

    private void LoadSettings(object obj)
    {
        var type = obj.GetType();
        var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(type)
            .OrderBy(z => z);
        foreach (var p in props)
        {
            var state = ReflectUtil.GetValue(obj, p);
            if (state is null)
                continue;

            var tab = new TabPage(p);
            var pg = new PropertyGrid { SelectedObject = state, Dock = DockStyle.Fill };
            tab.Controls.Add(pg);
            tabControl1.TabPages.Add(tab);
        }
    }

    private void SettingsEditor_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
            Close();
    }

    private static void DeleteSettings()
    {
        try
        {
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Resetting settings requires the program to exit.", MessageStrings.MsgContinue);
            if (dr != DialogResult.Yes)
                return;
            var path = Main.ConfigPath;
            if (File.Exists(path))
                File.Delete(path);
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error("Failed to delete settings.", ex.Message);
        }
    }
}
