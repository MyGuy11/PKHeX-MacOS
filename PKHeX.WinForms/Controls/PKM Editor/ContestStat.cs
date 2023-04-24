using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class ContestStat : UserControl, IContestStats
{
    public ContestStat()
    {
        InitializeComponent();
    }

    public byte CNT_Cool
    {
        get => (byte)Util.ToInt32(TB_Cool.Text);
        set => TB_Cool.Text = value.ToString();
    }

    public byte CNT_Beauty
    {
        get => (byte)Util.ToInt32(TB_Beauty.Text);
        set => TB_Beauty.Text = value.ToString();
    }

    public byte CNT_Cute
    {
        get => (byte)Util.ToInt32(TB_Cute.Text);
        set => TB_Cute.Text = value.ToString();
    }

    public byte CNT_Smart
    {
        get => (byte)Util.ToInt32(TB_Smart.Text);
        set => TB_Smart.Text = value.ToString();
    }

    public byte CNT_Tough
    {
        get => (byte)Util.ToInt32(TB_Tough.Text);
        set => TB_Tough.Text = value.ToString();
    }

    public byte CNT_Sheen
    {
        get => (byte)Util.ToInt32(TB_Sheen.Text);
        set => TB_Sheen.Text = value.ToString();
    }

    private void Update255_MTB(object sender, EventArgs e)
    {
        if (sender is not MaskedTextBox tb)
            return;
        if (Util.ToInt32(tb.Text) > byte.MaxValue)
            tb.Text = "255";
    }

    public void ToggleInterface(object o, EntityContext context)
    {
        if (o is not IContestStatsReadOnly)
        {
            Visible = TabStop = false;
            return;
        }

        Visible = TabStop = true;
        bool smart = context.Generation() < 6;
        Label_Smart.Visible = smart; // show smart gen3-5
        Label_Clever.Visible = !smart; // show clever gen6+
    }

    private void ClickTextBox(object sender, EventArgs e)
    {
        var keys = ModifierKeys;
        if (keys == Keys.None)
            return;

        if (sender is not MaskedTextBox tb)
            return;

        if (keys == Keys.Control)
            tb.Text = "255";
        else if (keys == Keys.Alt)
            tb.Text = "0";
    }
}
