﻿namespace PKHeX.WinForms
{
    partial class SAV_PokedexSWSH
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.LB_Species = new System.Windows.Forms.ListBox();
            this.CHK_Caught = new System.Windows.Forms.CheckBox();
            this.CHK_L7 = new System.Windows.Forms.CheckBox();
            this.CHK_L6 = new System.Windows.Forms.CheckBox();
            this.CHK_L5 = new System.Windows.Forms.CheckBox();
            this.CHK_L4 = new System.Windows.Forms.CheckBox();
            this.CHK_L3 = new System.Windows.Forms.CheckBox();
            this.CHK_L2 = new System.Windows.Forms.CheckBox();
            this.CHK_L1 = new System.Windows.Forms.CheckBox();
            this.L_goto = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Modify = new System.Windows.Forms.Button();
            this.GB_Language = new System.Windows.Forms.GroupBox();
            this.CHK_L9 = new System.Windows.Forms.CheckBox();
            this.CHK_L8 = new System.Windows.Forms.CheckBox();
            this.modifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBattleCount = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            this.NUD_Battled = new System.Windows.Forms.NumericUpDown();
            this.L_Battled = new System.Windows.Forms.Label();
            this.L_DisplayedForm = new System.Windows.Forms.Label();
            this.GB_Displayed = new System.Windows.Forms.GroupBox();
            this.CB_Gender = new System.Windows.Forms.ComboBox();
            this.CHK_S = new System.Windows.Forms.CheckBox();
            this.CHK_G = new System.Windows.Forms.CheckBox();
            this.CHK_Gigantamaxed = new System.Windows.Forms.CheckBox();
            this.NUD_Form = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.L_Male = new System.Windows.Forms.Label();
            this.L_Female = new System.Windows.Forms.Label();
            this.L_MaleShiny = new System.Windows.Forms.Label();
            this.L_FemaleShiny = new System.Windows.Forms.Label();
            this.CLB_3 = new System.Windows.Forms.CheckedListBox();
            this.CLB_4 = new System.Windows.Forms.CheckedListBox();
            this.CLB_1 = new System.Windows.Forms.CheckedListBox();
            this.CLB_2 = new System.Windows.Forms.CheckedListBox();
            this.CHK_Gigantamaxed1 = new System.Windows.Forms.CheckBox();
            this.GB_Language.SuspendLayout();
            this.modifyMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Battled)).BeginInit();
            this.GB_Displayed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Form)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(642, 421);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(80, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LB_Species
            // 
            this.LB_Species.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LB_Species.FormattingEnabled = true;
            this.LB_Species.Location = new System.Drawing.Point(12, 40);
            this.LB_Species.Name = "LB_Species";
            this.LB_Species.Size = new System.Drawing.Size(160, 394);
            this.LB_Species.TabIndex = 2;
            this.LB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeLBSpecies);
            // 
            // CHK_Caught
            // 
            this.CHK_Caught.AutoSize = true;
            this.CHK_Caught.Location = new System.Drawing.Point(284, 15);
            this.CHK_Caught.Name = "CHK_Caught";
            this.CHK_Caught.Size = new System.Drawing.Size(60, 17);
            this.CHK_Caught.TabIndex = 3;
            this.CHK_Caught.Text = "Owned";
            this.CHK_Caught.UseVisualStyleBackColor = true;
            // 
            // CHK_L7
            // 
            this.CHK_L7.AutoSize = true;
            this.CHK_L7.Location = new System.Drawing.Point(18, 117);
            this.CHK_L7.Name = "CHK_L7";
            this.CHK_L7.Size = new System.Drawing.Size(60, 17);
            this.CHK_L7.TabIndex = 19;
            this.CHK_L7.Text = "Korean";
            this.CHK_L7.UseVisualStyleBackColor = true;
            // 
            // CHK_L6
            // 
            this.CHK_L6.AutoSize = true;
            this.CHK_L6.Location = new System.Drawing.Point(18, 101);
            this.CHK_L6.Name = "CHK_L6";
            this.CHK_L6.Size = new System.Drawing.Size(64, 17);
            this.CHK_L6.TabIndex = 18;
            this.CHK_L6.Text = "Spanish";
            this.CHK_L6.UseVisualStyleBackColor = true;
            // 
            // CHK_L5
            // 
            this.CHK_L5.AutoSize = true;
            this.CHK_L5.Location = new System.Drawing.Point(18, 83);
            this.CHK_L5.Name = "CHK_L5";
            this.CHK_L5.Size = new System.Drawing.Size(63, 17);
            this.CHK_L5.TabIndex = 17;
            this.CHK_L5.Text = "German";
            this.CHK_L5.UseVisualStyleBackColor = true;
            // 
            // CHK_L4
            // 
            this.CHK_L4.AutoSize = true;
            this.CHK_L4.Location = new System.Drawing.Point(18, 66);
            this.CHK_L4.Name = "CHK_L4";
            this.CHK_L4.Size = new System.Drawing.Size(54, 17);
            this.CHK_L4.TabIndex = 16;
            this.CHK_L4.Text = "Italian";
            this.CHK_L4.UseVisualStyleBackColor = true;
            // 
            // CHK_L3
            // 
            this.CHK_L3.AutoSize = true;
            this.CHK_L3.Location = new System.Drawing.Point(18, 49);
            this.CHK_L3.Name = "CHK_L3";
            this.CHK_L3.Size = new System.Drawing.Size(59, 17);
            this.CHK_L3.TabIndex = 15;
            this.CHK_L3.Text = "French";
            this.CHK_L3.UseVisualStyleBackColor = true;
            // 
            // CHK_L2
            // 
            this.CHK_L2.AutoSize = true;
            this.CHK_L2.Location = new System.Drawing.Point(18, 33);
            this.CHK_L2.Name = "CHK_L2";
            this.CHK_L2.Size = new System.Drawing.Size(60, 17);
            this.CHK_L2.TabIndex = 14;
            this.CHK_L2.Text = "English";
            this.CHK_L2.UseVisualStyleBackColor = true;
            // 
            // CHK_L1
            // 
            this.CHK_L1.AutoSize = true;
            this.CHK_L1.Location = new System.Drawing.Point(18, 15);
            this.CHK_L1.Name = "CHK_L1";
            this.CHK_L1.Size = new System.Drawing.Size(72, 17);
            this.CHK_L1.TabIndex = 13;
            this.CHK_L1.Text = "Japanese";
            this.CHK_L1.UseVisualStyleBackColor = true;
            // 
            // L_goto
            // 
            this.L_goto.AutoSize = true;
            this.L_goto.Location = new System.Drawing.Point(12, 16);
            this.L_goto.Name = "L_goto";
            this.L_goto.Size = new System.Drawing.Size(31, 13);
            this.L_goto.TabIndex = 20;
            this.L_goto.Text = "goto:";
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.DropDownWidth = 95;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Items.AddRange(new object[] {
            "0"});
            this.CB_Species.Location = new System.Drawing.Point(50, 13);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(122, 21);
            this.CB_Species.TabIndex = 21;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeCBSpecies);
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.ChangeCBSpecies);
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(178, 11);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(60, 23);
            this.B_GiveAll.TabIndex = 23;
            this.B_GiveAll.Text = "Check All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(642, 392);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Modify
            // 
            this.B_Modify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Modify.Location = new System.Drawing.Point(600, 9);
            this.B_Modify.Name = "B_Modify";
            this.B_Modify.Size = new System.Drawing.Size(60, 23);
            this.B_Modify.TabIndex = 25;
            this.B_Modify.Text = "Modify...";
            this.B_Modify.UseVisualStyleBackColor = true;
            this.B_Modify.Click += new System.EventHandler(this.B_Modify_Click);
            // 
            // GB_Language
            // 
            this.GB_Language.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Language.Controls.Add(this.CHK_L9);
            this.GB_Language.Controls.Add(this.CHK_L8);
            this.GB_Language.Controls.Add(this.CHK_L7);
            this.GB_Language.Controls.Add(this.CHK_L6);
            this.GB_Language.Controls.Add(this.CHK_L5);
            this.GB_Language.Controls.Add(this.CHK_L4);
            this.GB_Language.Controls.Add(this.CHK_L3);
            this.GB_Language.Controls.Add(this.CHK_L2);
            this.GB_Language.Controls.Add(this.CHK_L1);
            this.GB_Language.Location = new System.Drawing.Point(600, 40);
            this.GB_Language.Name = "GB_Language";
            this.GB_Language.Size = new System.Drawing.Size(122, 172);
            this.GB_Language.TabIndex = 26;
            this.GB_Language.TabStop = false;
            this.GB_Language.Text = "Languages";
            // 
            // CHK_L9
            // 
            this.CHK_L9.AutoSize = true;
            this.CHK_L9.Location = new System.Drawing.Point(18, 152);
            this.CHK_L9.Name = "CHK_L9";
            this.CHK_L9.Size = new System.Drawing.Size(70, 17);
            this.CHK_L9.TabIndex = 21;
            this.CHK_L9.Text = "Chinese2";
            this.CHK_L9.UseVisualStyleBackColor = true;
            // 
            // CHK_L8
            // 
            this.CHK_L8.AutoSize = true;
            this.CHK_L8.Location = new System.Drawing.Point(18, 134);
            this.CHK_L8.Name = "CHK_L8";
            this.CHK_L8.Size = new System.Drawing.Size(64, 17);
            this.CHK_L8.TabIndex = 20;
            this.CHK_L8.Text = "Chinese";
            this.CHK_L8.UseVisualStyleBackColor = true;
            // 
            // modifyMenu
            // 
            this.modifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSeenNone,
            this.mnuSeenAll,
            this.mnuCaughtNone,
            this.mnuCaughtAll,
            this.mnuComplete,
            this.mnuBattleCount});
            this.modifyMenu.Name = "modifyMenu";
            this.modifyMenu.Size = new System.Drawing.Size(202, 136);
            // 
            // mnuSeenNone
            // 
            this.mnuSeenNone.Name = "mnuSeenNone";
            this.mnuSeenNone.Size = new System.Drawing.Size(201, 22);
            this.mnuSeenNone.Text = "Seen none";
            this.mnuSeenNone.Click += new System.EventHandler(this.SeenNone);
            // 
            // mnuSeenAll
            // 
            this.mnuSeenAll.Name = "mnuSeenAll";
            this.mnuSeenAll.Size = new System.Drawing.Size(201, 22);
            this.mnuSeenAll.Text = "Seen all";
            this.mnuSeenAll.Click += new System.EventHandler(this.SeenAll);
            // 
            // mnuCaughtNone
            // 
            this.mnuCaughtNone.Name = "mnuCaughtNone";
            this.mnuCaughtNone.Size = new System.Drawing.Size(201, 22);
            this.mnuCaughtNone.Text = "Caught none";
            this.mnuCaughtNone.Click += new System.EventHandler(this.CaughtNone);
            // 
            // mnuCaughtAll
            // 
            this.mnuCaughtAll.Name = "mnuCaughtAll";
            this.mnuCaughtAll.Size = new System.Drawing.Size(201, 22);
            this.mnuCaughtAll.Text = "Caught all";
            this.mnuCaughtAll.Click += new System.EventHandler(this.CaughtAll);
            // 
            // mnuComplete
            // 
            this.mnuComplete.Name = "mnuComplete";
            this.mnuComplete.Size = new System.Drawing.Size(201, 22);
            this.mnuComplete.Text = "Complete Dex";
            this.mnuComplete.Click += new System.EventHandler(this.CompleteDex);
            // 
            // mnuBattleCount
            // 
            this.mnuBattleCount.Name = "mnuBattleCount";
            this.mnuBattleCount.Size = new System.Drawing.Size(201, 22);
            this.mnuBattleCount.Text = "Change All Battle Count";
            this.mnuBattleCount.Click += new System.EventHandler(this.ChangeAllCounts);
            // 
            // mnuFormNone
            // 
            this.mnuFormNone.Name = "mnuFormNone";
            this.mnuFormNone.Size = new System.Drawing.Size(32, 19);
            // 
            // mnuForm1
            // 
            this.mnuForm1.Name = "mnuForm1";
            this.mnuForm1.Size = new System.Drawing.Size(32, 19);
            // 
            // mnuFormAll
            // 
            this.mnuFormAll.Name = "mnuFormAll";
            this.mnuFormAll.Size = new System.Drawing.Size(32, 19);
            // 
            // NUD_Battled
            // 
            this.NUD_Battled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Battled.Location = new System.Drawing.Point(603, 358);
            this.NUD_Battled.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUD_Battled.Name = "NUD_Battled";
            this.NUD_Battled.Size = new System.Drawing.Size(113, 20);
            this.NUD_Battled.TabIndex = 28;
            // 
            // L_Battled
            // 
            this.L_Battled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Battled.AutoSize = true;
            this.L_Battled.Location = new System.Drawing.Point(600, 342);
            this.L_Battled.Name = "L_Battled";
            this.L_Battled.Size = new System.Drawing.Size(43, 13);
            this.L_Battled.TabIndex = 29;
            this.L_Battled.Text = "Battled:";
            // 
            // L_DisplayedForm
            // 
            this.L_DisplayedForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_DisplayedForm.AutoSize = true;
            this.L_DisplayedForm.Location = new System.Drawing.Point(600, 219);
            this.L_DisplayedForm.Name = "L_DisplayedForm";
            this.L_DisplayedForm.Size = new System.Drawing.Size(82, 13);
            this.L_DisplayedForm.TabIndex = 32;
            this.L_DisplayedForm.Text = "Displayed Form:";
            // 
            // GB_Displayed
            // 
            this.GB_Displayed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Displayed.Controls.Add(this.CB_Gender);
            this.GB_Displayed.Controls.Add(this.CHK_S);
            this.GB_Displayed.Controls.Add(this.CHK_G);
            this.GB_Displayed.Location = new System.Drawing.Point(600, 262);
            this.GB_Displayed.Name = "GB_Displayed";
            this.GB_Displayed.Size = new System.Drawing.Size(122, 76);
            this.GB_Displayed.TabIndex = 33;
            this.GB_Displayed.TabStop = false;
            this.GB_Displayed.Text = "Displayed";
            // 
            // CB_Gender
            // 
            this.CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Gender.FormattingEnabled = true;
            this.CB_Gender.Items.AddRange(new object[] {
            "♂",
            "♀",
            "-"});
            this.CB_Gender.Location = new System.Drawing.Point(6, 49);
            this.CB_Gender.Name = "CB_Gender";
            this.CB_Gender.Size = new System.Drawing.Size(40, 21);
            this.CB_Gender.TabIndex = 23;
            // 
            // CHK_S
            // 
            this.CHK_S.AutoSize = true;
            this.CHK_S.Location = new System.Drawing.Point(5, 27);
            this.CHK_S.Name = "CHK_S";
            this.CHK_S.Size = new System.Drawing.Size(52, 17);
            this.CHK_S.TabIndex = 9;
            this.CHK_S.Text = "Shiny";
            this.CHK_S.UseVisualStyleBackColor = true;
            // 
            // CHK_G
            // 
            this.CHK_G.AutoSize = true;
            this.CHK_G.Location = new System.Drawing.Point(5, 13);
            this.CHK_G.Name = "CHK_G";
            this.CHK_G.Size = new System.Drawing.Size(82, 17);
            this.CHK_G.TabIndex = 8;
            this.CHK_G.Text = "Gigantamax";
            this.CHK_G.UseVisualStyleBackColor = true;
            // 
            // CHK_Gigantamaxed
            // 
            this.CHK_Gigantamaxed.AutoSize = true;
            this.CHK_Gigantamaxed.Location = new System.Drawing.Point(388, 15);
            this.CHK_Gigantamaxed.Name = "CHK_Gigantamaxed";
            this.CHK_Gigantamaxed.Size = new System.Drawing.Size(94, 17);
            this.CHK_Gigantamaxed.TabIndex = 34;
            this.CHK_Gigantamaxed.Text = "Gigantamaxed";
            this.CHK_Gigantamaxed.UseVisualStyleBackColor = true;
            // 
            // NUD_Form
            // 
            this.NUD_Form.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_Form.Location = new System.Drawing.Point(603, 236);
            this.NUD_Form.Name = "NUD_Form";
            this.NUD_Form.Size = new System.Drawing.Size(113, 20);
            this.NUD_Form.TabIndex = 35;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.L_Male, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.L_Female, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.L_MaleShiny, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.L_FemaleShiny, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.CLB_3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CLB_4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CLB_1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CLB_2, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(178, 40);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(416, 396);
            this.tableLayoutPanel1.TabIndex = 36;
            // 
            // L_Male
            // 
            this.L_Male.AutoSize = true;
            this.L_Male.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_Male.Location = new System.Drawing.Point(3, 0);
            this.L_Male.Name = "L_Male";
            this.L_Male.Size = new System.Drawing.Size(98, 20);
            this.L_Male.TabIndex = 37;
            this.L_Male.Text = "Male";
            this.L_Male.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Female
            // 
            this.L_Female.AutoSize = true;
            this.L_Female.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_Female.Location = new System.Drawing.Point(107, 0);
            this.L_Female.Name = "L_Female";
            this.L_Female.Size = new System.Drawing.Size(98, 20);
            this.L_Female.TabIndex = 38;
            this.L_Female.Text = "Female";
            this.L_Female.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_MaleShiny
            // 
            this.L_MaleShiny.AutoSize = true;
            this.L_MaleShiny.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_MaleShiny.Location = new System.Drawing.Point(211, 0);
            this.L_MaleShiny.Name = "L_MaleShiny";
            this.L_MaleShiny.Size = new System.Drawing.Size(98, 20);
            this.L_MaleShiny.TabIndex = 39;
            this.L_MaleShiny.Text = "*Male*";
            this.L_MaleShiny.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_FemaleShiny
            // 
            this.L_FemaleShiny.AutoSize = true;
            this.L_FemaleShiny.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_FemaleShiny.Location = new System.Drawing.Point(315, 0);
            this.L_FemaleShiny.Name = "L_FemaleShiny";
            this.L_FemaleShiny.Size = new System.Drawing.Size(98, 20);
            this.L_FemaleShiny.TabIndex = 40;
            this.L_FemaleShiny.Text = "*Female*";
            this.L_FemaleShiny.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CLB_3
            // 
            this.CLB_3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CLB_3.FormattingEnabled = true;
            this.CLB_3.Location = new System.Drawing.Point(209, 21);
            this.CLB_3.Margin = new System.Windows.Forms.Padding(1);
            this.CLB_3.Name = "CLB_3";
            this.CLB_3.Size = new System.Drawing.Size(102, 374);
            this.CLB_3.TabIndex = 34;
            // 
            // CLB_4
            // 
            this.CLB_4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CLB_4.FormattingEnabled = true;
            this.CLB_4.Location = new System.Drawing.Point(313, 21);
            this.CLB_4.Margin = new System.Windows.Forms.Padding(1);
            this.CLB_4.Name = "CLB_4";
            this.CLB_4.Size = new System.Drawing.Size(102, 374);
            this.CLB_4.TabIndex = 33;
            // 
            // CLB_1
            // 
            this.CLB_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CLB_1.FormattingEnabled = true;
            this.CLB_1.Location = new System.Drawing.Point(1, 21);
            this.CLB_1.Margin = new System.Windows.Forms.Padding(1);
            this.CLB_1.Name = "CLB_1";
            this.CLB_1.Size = new System.Drawing.Size(102, 374);
            this.CLB_1.TabIndex = 32;
            // 
            // CLB_2
            // 
            this.CLB_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CLB_2.FormattingEnabled = true;
            this.CLB_2.Location = new System.Drawing.Point(105, 21);
            this.CLB_2.Margin = new System.Windows.Forms.Padding(1);
            this.CLB_2.Name = "CLB_2";
            this.CLB_2.Size = new System.Drawing.Size(102, 374);
            this.CLB_2.TabIndex = 30;
            // 
            // CHK_Gigantamaxed1
            // 
            this.CHK_Gigantamaxed1.AutoSize = true;
            this.CHK_Gigantamaxed1.Location = new System.Drawing.Point(491, 15);
            this.CHK_Gigantamaxed1.Name = "CHK_Gigantamaxed1";
            this.CHK_Gigantamaxed1.Size = new System.Drawing.Size(103, 17);
            this.CHK_Gigantamaxed1.TabIndex = 37;
            this.CHK_Gigantamaxed1.Text = "Gigantamaxed 1";
            this.CHK_Gigantamaxed1.UseVisualStyleBackColor = true;
            // 
            // SAV_PokedexSWSH
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 454);
            this.Controls.Add(this.CHK_Gigantamaxed1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.NUD_Form);
            this.Controls.Add(this.CHK_Gigantamaxed);
            this.Controls.Add(this.GB_Displayed);
            this.Controls.Add(this.L_DisplayedForm);
            this.Controls.Add(this.L_Battled);
            this.Controls.Add(this.NUD_Battled);
            this.Controls.Add(this.CHK_Caught);
            this.Controls.Add(this.GB_Language);
            this.Controls.Add(this.B_Modify);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.L_goto);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(750, 492);
            this.Name = "SAV_PokedexSWSH";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokédex Editor";
            this.GB_Language.ResumeLayout(false);
            this.GB_Language.PerformLayout();
            this.modifyMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Battled)).EndInit();
            this.GB_Displayed.ResumeLayout(false);
            this.GB_Displayed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Form)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.CheckBox CHK_Caught;
        private System.Windows.Forms.CheckBox CHK_L7;
        private System.Windows.Forms.CheckBox CHK_L6;
        private System.Windows.Forms.CheckBox CHK_L5;
        private System.Windows.Forms.CheckBox CHK_L4;
        private System.Windows.Forms.CheckBox CHK_L3;
        private System.Windows.Forms.CheckBox CHK_L2;
        private System.Windows.Forms.CheckBox CHK_L1;
        private System.Windows.Forms.Label L_goto;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Modify;
        private System.Windows.Forms.GroupBox GB_Language;
        private System.Windows.Forms.ContextMenuStrip modifyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenNone;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtNone;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtAll;
        private System.Windows.Forms.ToolStripMenuItem mnuComplete;
        private System.Windows.Forms.ToolStripMenuItem mnuFormNone;
        private System.Windows.Forms.ToolStripMenuItem mnuForm1;
        private System.Windows.Forms.ToolStripMenuItem mnuFormAll;
        private System.Windows.Forms.CheckBox CHK_L8;
        private System.Windows.Forms.CheckBox CHK_L9;
        private System.Windows.Forms.NumericUpDown NUD_Battled;
        private System.Windows.Forms.Label L_Battled;
        private System.Windows.Forms.Label L_DisplayedForm;
        private System.Windows.Forms.GroupBox GB_Displayed;
        private System.Windows.Forms.CheckBox CHK_S;
        private System.Windows.Forms.CheckBox CHK_G;
        private System.Windows.Forms.CheckBox CHK_Gigantamaxed;
        private System.Windows.Forms.NumericUpDown NUD_Form;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label L_Male;
        private System.Windows.Forms.Label L_Female;
        private System.Windows.Forms.Label L_MaleShiny;
        private System.Windows.Forms.Label L_FemaleShiny;
        private System.Windows.Forms.CheckedListBox CLB_3;
        private System.Windows.Forms.CheckedListBox CLB_4;
        private System.Windows.Forms.CheckedListBox CLB_1;
        private System.Windows.Forms.CheckedListBox CLB_2;
        private System.Windows.Forms.ComboBox CB_Gender;
        private System.Windows.Forms.ToolStripMenuItem mnuBattleCount;
        private System.Windows.Forms.CheckBox CHK_Gigantamaxed1;
    }
}
