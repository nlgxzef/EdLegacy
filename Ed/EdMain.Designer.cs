namespace Ed
{
    partial class EdMain
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EdMain));
            this.LabelWorkingDir = new System.Windows.Forms.Label();
            this.DialogWorkingFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ButtonAddCars = new System.Windows.Forms.Button();
            this.ButtonViewCars = new System.Windows.Forms.Button();
            this.StatusBarEd = new System.Windows.Forms.StatusStrip();
            this.StatusTextEd = new System.Windows.Forms.ToolStripStatusLabel();
            this.MenuBarEd = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemBrowseConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemBrowseResources = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.createConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCarsFromConfigFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuUnlockFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreBackupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemLog = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemTempFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutEdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListCarsToAdd = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StatusBarEd.SuspendLayout();
            this.MenuBarEd.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelWorkingDir
            // 
            this.LabelWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelWorkingDir.AutoSize = true;
            this.LabelWorkingDir.Location = new System.Drawing.Point(12, 305);
            this.LabelWorkingDir.Name = "LabelWorkingDir";
            this.LabelWorkingDir.Size = new System.Drawing.Size(170, 13);
            this.LabelWorkingDir.TabIndex = 0;
            this.LabelWorkingDir.Text = "Select your game directory to start.";
            // 
            // DialogWorkingFolder
            // 
            this.DialogWorkingFolder.Description = "Select the main directory of the game you want to work on. Only NFS Carbon is sup" +
    "ported for now.";
            this.DialogWorkingFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // ButtonAddCars
            // 
            this.ButtonAddCars.Enabled = false;
            this.ButtonAddCars.Location = new System.Drawing.Point(495, 300);
            this.ButtonAddCars.Name = "ButtonAddCars";
            this.ButtonAddCars.Size = new System.Drawing.Size(114, 23);
            this.ButtonAddCars.TabIndex = 3;
            this.ButtonAddCars.Text = "Add Cars";
            this.ButtonAddCars.UseVisualStyleBackColor = true;
            this.ButtonAddCars.Click += new System.EventHandler(this.addCarsFromConfigFilesToolStripMenuItem_Click);
            // 
            // ButtonViewCars
            // 
            this.ButtonViewCars.Enabled = false;
            this.ButtonViewCars.Location = new System.Drawing.Point(377, 300);
            this.ButtonViewCars.Name = "ButtonViewCars";
            this.ButtonViewCars.Size = new System.Drawing.Size(112, 23);
            this.ButtonViewCars.TabIndex = 3;
            this.ButtonViewCars.TabStop = false;
            this.ButtonViewCars.Text = "View Cars (Debug)";
            this.ButtonViewCars.UseVisualStyleBackColor = true;
            this.ButtonViewCars.Visible = false;
            this.ButtonViewCars.Click += new System.EventHandler(this.ButtonViewCars_Click);
            // 
            // StatusBarEd
            // 
            this.StatusBarEd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusTextEd});
            this.StatusBarEd.Location = new System.Drawing.Point(0, 329);
            this.StatusBarEd.Name = "StatusBarEd";
            this.StatusBarEd.Size = new System.Drawing.Size(624, 22);
            this.StatusBarEd.SizingGrip = false;
            this.StatusBarEd.TabIndex = 5;
            // 
            // StatusTextEd
            // 
            this.StatusTextEd.Name = "StatusTextEd";
            this.StatusTextEd.Size = new System.Drawing.Size(609, 17);
            this.StatusTextEd.Spring = true;
            this.StatusTextEd.Text = "Ready.";
            this.StatusTextEd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MenuBarEd
            // 
            this.MenuBarEd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.MenuTools,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.MenuBarEd.Location = new System.Drawing.Point(0, 0);
            this.MenuBarEd.Name = "MenuBarEd";
            this.MenuBarEd.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.MenuBarEd.Size = new System.Drawing.Size(624, 24);
            this.MenuBarEd.TabIndex = 6;
            this.MenuBarEd.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.MenuItemBrowseConfig,
            this.MenuItemBrowseResources,
            this.toolStripSeparator2,
            this.MenuItemExit});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(46, 20);
            this.toolStripMenuItem1.Text = "Main";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.ToolTipText = "Opens a game installation directory to work on.";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(251, 6);
            // 
            // MenuItemBrowseConfig
            // 
            this.MenuItemBrowseConfig.Name = "MenuItemBrowseConfig";
            this.MenuItemBrowseConfig.ShortcutKeyDisplayString = "";
            this.MenuItemBrowseConfig.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.MenuItemBrowseConfig.Size = new System.Drawing.Size(254, 22);
            this.MenuItemBrowseConfig.Text = "Browse Config Folder...";
            this.MenuItemBrowseConfig.ToolTipText = "Opens config folder for the game you are working on.";
            this.MenuItemBrowseConfig.Click += new System.EventHandler(this.ButtonBrowseConfigFolder_Click);
            // 
            // MenuItemBrowseResources
            // 
            this.MenuItemBrowseResources.Name = "MenuItemBrowseResources";
            this.MenuItemBrowseResources.ShortcutKeyDisplayString = "";
            this.MenuItemBrowseResources.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.MenuItemBrowseResources.Size = new System.Drawing.Size(254, 22);
            this.MenuItemBrowseResources.Text = "Browse Resources Folder...";
            this.MenuItemBrowseResources.ToolTipText = "Opens resources folder for the game you are working on.";
            this.MenuItemBrowseResources.Click += new System.EventHandler(this.MenuItemBrowseResources_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(251, 6);
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.ShortcutKeyDisplayString = "";
            this.MenuItemExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.MenuItemExit.Size = new System.Drawing.Size(254, 22);
            this.MenuItemExit.Text = "Exit";
            this.MenuItemExit.ToolTipText = "Closes the program.";
            this.MenuItemExit.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // MenuTools
            // 
            this.MenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemRefresh,
            this.toolStripSeparator3,
            this.createConfigFileToolStripMenuItem,
            this.addCarsFromConfigFilesToolStripMenuItem,
            this.toolStripSeparator4,
            this.menuUnlockFiles,
            this.restoreBackupsToolStripMenuItem});
            this.MenuTools.Name = "MenuTools";
            this.MenuTools.Size = new System.Drawing.Size(47, 20);
            this.MenuTools.Text = "Tools";
            // 
            // MenuItemRefresh
            // 
            this.MenuItemRefresh.Name = "MenuItemRefresh";
            this.MenuItemRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.MenuItemRefresh.Size = new System.Drawing.Size(281, 22);
            this.MenuItemRefresh.Text = "Refresh Config View";
            this.MenuItemRefresh.Click += new System.EventHandler(this.MenuItemRefresh_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(278, 6);
            // 
            // createConfigFileToolStripMenuItem
            // 
            this.createConfigFileToolStripMenuItem.Enabled = false;
            this.createConfigFileToolStripMenuItem.Name = "createConfigFileToolStripMenuItem";
            this.createConfigFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.createConfigFileToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.createConfigFileToolStripMenuItem.Text = "Create Config File...";
            this.createConfigFileToolStripMenuItem.ToolTipText = "Not yet implemented.";
            // 
            // addCarsFromConfigFilesToolStripMenuItem
            // 
            this.addCarsFromConfigFilesToolStripMenuItem.Name = "addCarsFromConfigFilesToolStripMenuItem";
            this.addCarsFromConfigFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.addCarsFromConfigFilesToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.addCarsFromConfigFilesToolStripMenuItem.Text = "Add Cars From Config File(s)";
            this.addCarsFromConfigFilesToolStripMenuItem.Click += new System.EventHandler(this.addCarsFromConfigFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(278, 6);
            // 
            // menuUnlockFiles
            // 
            this.menuUnlockFiles.Name = "menuUnlockFiles";
            this.menuUnlockFiles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.U)));
            this.menuUnlockFiles.Size = new System.Drawing.Size(281, 22);
            this.menuUnlockFiles.Text = "Unlock Game Files For Modding";
            this.menuUnlockFiles.ToolTipText = "Unlocks game files for modding by invalidating memory files in Global folder.\r\nIt" +
    "\'s suggested to use this option for once.";
            this.menuUnlockFiles.Click += new System.EventHandler(this.menuUnlockFiles_Click);
            // 
            // restoreBackupsToolStripMenuItem
            // 
            this.restoreBackupsToolStripMenuItem.Name = "restoreBackupsToolStripMenuItem";
            this.restoreBackupsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.restoreBackupsToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.restoreBackupsToolStripMenuItem.Text = "Restore Backups";
            this.restoreBackupsToolStripMenuItem.ToolTipText = "Restores backups taken by Ed, which have \".edbackup\" extension.";
            this.restoreBackupsToolStripMenuItem.Click += new System.EventHandler(this.ButtonRestoreBackups_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemBackup,
            this.MenuItemLog,
            this.MenuItemTempFiles});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(61, 20);
            this.toolStripMenuItem3.Text = "Options";
            // 
            // MenuItemBackup
            // 
            this.MenuItemBackup.Checked = true;
            this.MenuItemBackup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuItemBackup.Name = "MenuItemBackup";
            this.MenuItemBackup.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
            this.MenuItemBackup.Size = new System.Drawing.Size(269, 22);
            this.MenuItemBackup.Text = "Create Backups Automatically";
            this.MenuItemBackup.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // MenuItemLog
            // 
            this.MenuItemLog.Checked = true;
            this.MenuItemLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuItemLog.Name = "MenuItemLog";
            this.MenuItemLog.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.MenuItemLog.Size = new System.Drawing.Size(269, 22);
            this.MenuItemLog.Text = "Enable Logging";
            this.MenuItemLog.Click += new System.EventHandler(this.MenuItemLog_Click);
            // 
            // MenuItemTempFiles
            // 
            this.MenuItemTempFiles.Name = "MenuItemTempFiles";
            this.MenuItemTempFiles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.MenuItemTempFiles.Size = new System.Drawing.Size(269, 22);
            this.MenuItemTempFiles.Text = "Keep Temporary Files";
            this.MenuItemTempFiles.Click += new System.EventHandler(this.keepTemporaryFilesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutEdToolStripMenuItem});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem4.Text = "Help";
            // 
            // aboutEdToolStripMenuItem
            // 
            this.aboutEdToolStripMenuItem.Name = "aboutEdToolStripMenuItem";
            this.aboutEdToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutEdToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutEdToolStripMenuItem.Text = "About Ed...";
            this.aboutEdToolStripMenuItem.Click += new System.EventHandler(this.aboutEdToolStripMenuItem_Click);
            // 
            // ListCarsToAdd
            // 
            this.ListCarsToAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListCarsToAdd.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader11});
            this.ListCarsToAdd.FullRowSelect = true;
            this.ListCarsToAdd.GridLines = true;
            this.ListCarsToAdd.Location = new System.Drawing.Point(12, 37);
            this.ListCarsToAdd.Name = "ListCarsToAdd";
            this.ListCarsToAdd.ShowItemToolTips = true;
            this.ListCarsToAdd.Size = new System.Drawing.Size(597, 257);
            this.ListCarsToAdd.TabIndex = 7;
            this.ListCarsToAdd.UseCompatibleStateImageBehavior = false;
            this.ListCarsToAdd.View = System.Windows.Forms.View.Details;
            this.ListCarsToAdd.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListCarsToAdd_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File Name";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Description";
            this.columnHeader11.Width = 420;
            // 
            // EdMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 351);
            this.Controls.Add(this.ListCarsToAdd);
            this.Controls.Add(this.StatusBarEd);
            this.Controls.Add(this.MenuBarEd);
            this.Controls.Add(this.ButtonViewCars);
            this.Controls.Add(this.ButtonAddCars);
            this.Controls.Add(this.LabelWorkingDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuBarEd;
            this.MaximizeBox = false;
            this.Name = "EdMain";
            this.Text = "Ed - The Car Dealer";
            this.Load += new System.EventHandler(this.EdMain_Load);
            this.StatusBarEd.ResumeLayout(false);
            this.StatusBarEd.PerformLayout();
            this.MenuBarEd.ResumeLayout(false);
            this.MenuBarEd.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelWorkingDir;
        private System.Windows.Forms.FolderBrowserDialog DialogWorkingFolder;
        private System.Windows.Forms.Button ButtonAddCars;
        private System.Windows.Forms.Button ButtonViewCars;
        private System.Windows.Forms.StatusStrip StatusBarEd;
        private System.Windows.Forms.ToolStripStatusLabel StatusTextEd;
        private System.Windows.Forms.MenuStrip MenuBarEd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuItemBrowseConfig;
        private System.Windows.Forms.ToolStripMenuItem MenuItemBrowseResources;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.ToolStripMenuItem MenuTools;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem MenuItemBackup;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem aboutEdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createConfigFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addCarsFromConfigFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuUnlockFiles;
        private System.Windows.Forms.ToolStripMenuItem MenuItemLog;
        private System.Windows.Forms.ToolStripMenuItem restoreBackupsToolStripMenuItem;
        private System.Windows.Forms.ListView ListCarsToAdd;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ToolStripMenuItem MenuItemRefresh;
        private System.Windows.Forms.ToolStripMenuItem MenuItemTempFiles;
    }
}

