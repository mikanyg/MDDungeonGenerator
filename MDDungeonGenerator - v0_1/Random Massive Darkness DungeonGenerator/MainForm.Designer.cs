namespace MassiveDarknessRandomDungeonGenerator
{
    partial class DungeonGeneratorForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateDungeonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyDungeonToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDungeonMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTilesets = new System.Windows.Forms.Label();
            this.lstTilesets = new System.Windows.Forms.ListBox();
            this.lblDungeonSize = new System.Windows.Forms.Label();
            this.cboDungeonSize = new System.Windows.Forms.ComboBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnSaveMap = new System.Windows.Forms.Button();
            this.grpDungeonPreview = new System.Windows.Forms.GroupBox();
            this.pnlDungeonPreview = new System.Windows.Forms.Panel();
            this.pbDungeonPreview = new System.Windows.Forms.PictureBox();
            this.chkFadedTiles = new System.Windows.Forms.CheckBox();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.chkDifficultyLevel = new System.Windows.Forms.CheckBox();
            this.chkIncludeLayout = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.grpDungeonPreview.SuspendLayout();
            this.pnlDungeonPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDungeonPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "msMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateDungeonToolStripMenuItem,
            this.copyDungeonToClipboardToolStripMenuItem,
            this.saveDungeonMapToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // generateDungeonToolStripMenuItem
            // 
            this.generateDungeonToolStripMenuItem.Name = "generateDungeonToolStripMenuItem";
            this.generateDungeonToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.generateDungeonToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.generateDungeonToolStripMenuItem.Text = "&Generate Random Dungeon";
            this.generateDungeonToolStripMenuItem.Click += new System.EventHandler(this.generateDungeonToolStripMenuItem_Click);
            // 
            // copyDungeonToClipboardToolStripMenuItem
            // 
            this.copyDungeonToClipboardToolStripMenuItem.Name = "copyDungeonToClipboardToolStripMenuItem";
            this.copyDungeonToClipboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyDungeonToClipboardToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.copyDungeonToClipboardToolStripMenuItem.Text = "&Copy Dungeon to Clipboard";
            this.copyDungeonToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyDungeonToClipboardToolStripMenuItem_Click);
            // 
            // saveDungeonMapToolStripMenuItem
            // 
            this.saveDungeonMapToolStripMenuItem.Name = "saveDungeonMapToolStripMenuItem";
            this.saveDungeonMapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveDungeonMapToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.saveDungeonMapToolStripMenuItem.Text = "&Save Dungeon Map File";
            this.saveDungeonMapToolStripMenuItem.Click += new System.EventHandler(this.saveDungeonMapToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(262, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // lblTilesets
            // 
            this.lblTilesets.AutoSize = true;
            this.lblTilesets.Location = new System.Drawing.Point(12, 36);
            this.lblTilesets.Name = "lblTilesets";
            this.lblTilesets.Size = new System.Drawing.Size(98, 13);
            this.lblTilesets.TabIndex = 1;
            this.lblTilesets.Text = "Tile Sets to Include";
            // 
            // lstTilesets
            // 
            this.lstTilesets.FormattingEnabled = true;
            this.lstTilesets.Location = new System.Drawing.Point(12, 52);
            this.lstTilesets.Name = "lstTilesets";
            this.lstTilesets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstTilesets.Size = new System.Drawing.Size(163, 56);
            this.lstTilesets.TabIndex = 2;
            // 
            // lblDungeonSize
            // 
            this.lblDungeonSize.AutoSize = true;
            this.lblDungeonSize.Location = new System.Drawing.Point(203, 36);
            this.lblDungeonSize.Name = "lblDungeonSize";
            this.lblDungeonSize.Size = new System.Drawing.Size(74, 13);
            this.lblDungeonSize.TabIndex = 3;
            this.lblDungeonSize.Text = "Dungeon Size";
            // 
            // cboDungeonSize
            // 
            this.cboDungeonSize.FormattingEnabled = true;
            this.cboDungeonSize.Location = new System.Drawing.Point(206, 52);
            this.cboDungeonSize.Name = "cboDungeonSize";
            this.cboDungeonSize.Size = new System.Drawing.Size(190, 21);
            this.cboDungeonSize.TabIndex = 4;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(586, 38);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(177, 46);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Generate Random Dungeon";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.Location = new System.Drawing.Point(807, 72);
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.Size = new System.Drawing.Size(157, 23);
            this.btnSaveMap.TabIndex = 7;
            this.btnSaveMap.Text = "Save Dungeon Map File";
            this.btnSaveMap.UseVisualStyleBackColor = true;
            this.btnSaveMap.Click += new System.EventHandler(this.btnSaveMap_Click);
            // 
            // grpDungeonPreview
            // 
            this.grpDungeonPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDungeonPreview.Controls.Add(this.pnlDungeonPreview);
            this.grpDungeonPreview.Location = new System.Drawing.Point(12, 118);
            this.grpDungeonPreview.Name = "grpDungeonPreview";
            this.grpDungeonPreview.Size = new System.Drawing.Size(984, 599);
            this.grpDungeonPreview.TabIndex = 8;
            this.grpDungeonPreview.TabStop = false;
            this.grpDungeonPreview.Text = "Dungeon Preview";
            // 
            // pnlDungeonPreview
            // 
            this.pnlDungeonPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDungeonPreview.AutoScroll = true;
            this.pnlDungeonPreview.BackColor = System.Drawing.Color.Transparent;
            this.pnlDungeonPreview.Controls.Add(this.pbDungeonPreview);
            this.pnlDungeonPreview.Location = new System.Drawing.Point(6, 19);
            this.pnlDungeonPreview.Name = "pnlDungeonPreview";
            this.pnlDungeonPreview.Size = new System.Drawing.Size(972, 574);
            this.pnlDungeonPreview.TabIndex = 0;
            // 
            // pbDungeonPreview
            // 
            this.pbDungeonPreview.BackColor = System.Drawing.Color.Transparent;
            this.pbDungeonPreview.Location = new System.Drawing.Point(0, 0);
            this.pbDungeonPreview.Name = "pbDungeonPreview";
            this.pbDungeonPreview.Size = new System.Drawing.Size(50, 50);
            this.pbDungeonPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbDungeonPreview.TabIndex = 0;
            this.pbDungeonPreview.TabStop = false;
            // 
            // chkFadedTiles
            // 
            this.chkFadedTiles.AutoSize = true;
            this.chkFadedTiles.Location = new System.Drawing.Point(422, 45);
            this.chkFadedTiles.Name = "chkFadedTiles";
            this.chkFadedTiles.Size = new System.Drawing.Size(116, 17);
            this.chkFadedTiles.TabIndex = 1;
            this.chkFadedTiles.Text = "Use Full Color Tiles";
            this.chkFadedTiles.UseVisualStyleBackColor = true;
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Location = new System.Drawing.Point(807, 38);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(157, 23);
            this.btnCopyToClipboard.TabIndex = 9;
            this.btnCopyToClipboard.Text = "Copy Dungeon to Clipboard";
            this.btnCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // chkDifficultyLevel
            // 
            this.chkDifficultyLevel.AutoSize = true;
            this.chkDifficultyLevel.Location = new System.Drawing.Point(422, 67);
            this.chkDifficultyLevel.Name = "chkDifficultyLevel";
            this.chkDifficultyLevel.Size = new System.Drawing.Size(143, 17);
            this.chkDifficultyLevel.TabIndex = 10;
            this.chkDifficultyLevel.Text = "Generate Easy Dungeon";
            this.chkDifficultyLevel.UseVisualStyleBackColor = true;
            // 
            // chkIncludeLayout
            // 
            this.chkIncludeLayout.AutoSize = true;
            this.chkIncludeLayout.Location = new System.Drawing.Point(422, 90);
            this.chkIncludeLayout.Name = "chkIncludeLayout";
            this.chkIncludeLayout.Size = new System.Drawing.Size(142, 17);
            this.chkIncludeLayout.TabIndex = 11;
            this.chkIncludeLayout.Text = "Include Layout Summary";
            this.chkIncludeLayout.UseVisualStyleBackColor = true;
            // 
            // DungeonGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.chkIncludeLayout);
            this.Controls.Add(this.chkDifficultyLevel);
            this.Controls.Add(this.btnCopyToClipboard);
            this.Controls.Add(this.chkFadedTiles);
            this.Controls.Add(this.grpDungeonPreview);
            this.Controls.Add(this.btnSaveMap);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.cboDungeonSize);
            this.Controls.Add(this.lblDungeonSize);
            this.Controls.Add(this.lstTilesets);
            this.Controls.Add(this.lblTilesets);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DungeonGeneratorForm";
            this.ShowIcon = false;
            this.Text = "Massive Darkness Random Dungeon Generator";
            this.Load += new System.EventHandler(this.DungeonGeneratorForm_Load);
            this.Shown += new System.EventHandler(this.DungeonGeneratorForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpDungeonPreview.ResumeLayout(false);
            this.pnlDungeonPreview.ResumeLayout(false);
            this.pnlDungeonPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDungeonPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateDungeonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label lblTilesets;
        private System.Windows.Forms.ListBox lstTilesets;
        private System.Windows.Forms.Label lblDungeonSize;
        private System.Windows.Forms.ComboBox cboDungeonSize;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnSaveMap;
        private System.Windows.Forms.GroupBox grpDungeonPreview;
        private System.Windows.Forms.ToolStripMenuItem saveDungeonMapToolStripMenuItem;
        private System.Windows.Forms.Panel pnlDungeonPreview;
        private System.Windows.Forms.PictureBox pbDungeonPreview;
        private System.Windows.Forms.CheckBox chkFadedTiles;
        private System.Windows.Forms.Button btnCopyToClipboard;
        private System.Windows.Forms.CheckBox chkDifficultyLevel;
        private System.Windows.Forms.ToolStripMenuItem copyDungeonToClipboardToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkIncludeLayout;
    }
}

