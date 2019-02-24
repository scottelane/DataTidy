namespace ScottLane.DataTidy.Client.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainDockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2005Theme1 = new WeifenLuo.WinFormsUI.Docking.VS2005Theme();
            this.menuToolStrip = new System.Windows.Forms.ToolStrip();
            this.fileToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.executeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.windowsToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.dataSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPerformanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewProgressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.recentProjectContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeRecentProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStrip.SuspendLayout();
            this.recentProjectContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainDockPanel
            // 
            this.mainDockPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mainDockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainDockPanel.Location = new System.Drawing.Point(0, 39);
            this.mainDockPanel.Name = "mainDockPanel";
            this.mainDockPanel.Size = new System.Drawing.Size(1300, 817);
            this.mainDockPanel.TabIndex = 1;
            // 
            // menuToolStrip
            // 
            this.menuToolStrip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripButton,
            this.toolStripSeparator12,
            this.executeToolStripButton,
            this.stopToolStripButton,
            this.toolStripSeparator6,
            this.windowsToolStripButton,
            this.helpToolStripButton});
            this.menuToolStrip.Location = new System.Drawing.Point(0, 0);
            this.menuToolStrip.Name = "menuToolStrip";
            this.menuToolStrip.Size = new System.Drawing.Size(1300, 39);
            this.menuToolStrip.TabIndex = 5;
            this.menuToolStrip.Text = "toolStrip1";
            // 
            // fileToolStripButton
            // 
            this.fileToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.toolStripSeparator4,
            this.closeProjectToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveProjectToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.toolStripSeparator3,
            this.recentProjectsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormFile;
            this.fileToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.fileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileToolStripButton.Name = "fileToolStripButton";
            this.fileToolStripButton.Size = new System.Drawing.Size(70, 36);
            this.fileToolStripButton.Text = "File";
            this.fileToolStripButton.DropDownOpening += new System.EventHandler(this.ProjectToolStripButton_DropDownOpening);
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.AddProject;
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+N";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.newProjectToolStripMenuItem.Text = "New";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.NewProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.OpenProject;
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
            this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.openProjectToolStripMenuItem.Text = "Open";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OpenProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(169, 6);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.CloseProject;
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+W";
            this.closeProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.closeProjectToolStripMenuItem.Text = "Close";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.CloseProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(169, 6);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormSaveProject;
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
            this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.saveProjectToolStripMenuItem.Text = "Save";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveProjectAsToolStripMenuItem
            // 
            this.saveProjectAsToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormSaveProjectAs;
            this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.saveProjectAsToolStripMenuItem.Text = "Save As...";
            this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(169, 6);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.RecentProjects;
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.recentProjectsToolStripMenuItem.Text = "Recent Projects...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormExit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 30);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 39);
            // 
            // executeToolStripButton
            // 
            this.executeToolStripButton.Enabled = false;
            this.executeToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Go;
            this.executeToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.executeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.executeToolStripButton.Name = "executeToolStripButton";
            this.executeToolStripButton.Size = new System.Drawing.Size(83, 36);
            this.executeToolStripButton.Text = "Execute";
            this.executeToolStripButton.ToolTipText = "Execute all batches and operations in the project";
            this.executeToolStripButton.Click += new System.EventHandler(this.ExecuteToolStripButton_Click);
            // 
            // stopToolStripButton
            // 
            this.stopToolStripButton.Enabled = false;
            this.stopToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("stopToolStripButton.Image")));
            this.stopToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopToolStripButton.Name = "stopToolStripButton";
            this.stopToolStripButton.Size = new System.Drawing.Size(67, 36);
            this.stopToolStripButton.Text = "Stop";
            this.stopToolStripButton.ToolTipText = "Stop the current operation";
            this.stopToolStripButton.Click += new System.EventHandler(this.StopToolStripButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 39);
            // 
            // windowsToolStripButton
            // 
            this.windowsToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataSourcesToolStripMenuItem,
            this.viewNotificationsToolStripMenuItem,
            this.viewPerformanceToolStripMenuItem,
            this.viewProgressToolStripMenuItem,
            this.viewPropertiesToolStripMenuItem});
            this.windowsToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormWindows;
            this.windowsToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.windowsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.windowsToolStripButton.Name = "windowsToolStripButton";
            this.windowsToolStripButton.Size = new System.Drawing.Size(101, 36);
            this.windowsToolStripButton.Text = "Windows";
            // 
            // dataSourcesToolStripMenuItem
            // 
            this.dataSourcesToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ConnectionsForm24;
            this.dataSourcesToolStripMenuItem.Name = "dataSourcesToolStripMenuItem";
            this.dataSourcesToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.dataSourcesToolStripMenuItem.Text = "Data Sources";
            this.dataSourcesToolStripMenuItem.Click += new System.EventHandler(this.DataSourcesToolStripMenuItem_Click);
            // 
            // viewNotificationsToolStripMenuItem
            // 
            this.viewNotificationsToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsForm24;
            this.viewNotificationsToolStripMenuItem.Name = "viewNotificationsToolStripMenuItem";
            this.viewNotificationsToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.viewNotificationsToolStripMenuItem.Text = "Notifications";
            this.viewNotificationsToolStripMenuItem.Click += new System.EventHandler(this.ViewNotificationsToolStripMenuItem_Click);
            // 
            // viewPerformanceToolStripMenuItem
            // 
            this.viewPerformanceToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.PerformanceForm24;
            this.viewPerformanceToolStripMenuItem.Name = "viewPerformanceToolStripMenuItem";
            this.viewPerformanceToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.viewPerformanceToolStripMenuItem.Text = "Performance";
            this.viewPerformanceToolStripMenuItem.Click += new System.EventHandler(this.ViewPerformanceToolStripMenuItem_Click);
            // 
            // viewProgressToolStripMenuItem
            // 
            this.viewProgressToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ProgressForm24;
            this.viewProgressToolStripMenuItem.Name = "viewProgressToolStripMenuItem";
            this.viewProgressToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.viewProgressToolStripMenuItem.Text = "Progress";
            this.viewProgressToolStripMenuItem.Click += new System.EventHandler(this.ViewProgressToolStripMenuItem_Click);
            // 
            // viewPropertiesToolStripMenuItem
            // 
            this.viewPropertiesToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Properties24;
            this.viewPropertiesToolStripMenuItem.Name = "viewPropertiesToolStripMenuItem";
            this.viewPropertiesToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.viewPropertiesToolStripMenuItem.Text = "Properties";
            this.viewPropertiesToolStripMenuItem.Click += new System.EventHandler(this.ViewPropertiesToolStripMenuItem_Click);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormHelp;
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(69, 36);
            this.helpToolStripButton.Text = "Help";
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormDocumentation;
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            this.documentationToolStripMenuItem.Size = new System.Drawing.Size(212, 30);
            this.documentationToolStripMenuItem.Text = "Online Documentation...";
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.DocumentationToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.MainFormAbout;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(212, 30);
            this.aboutToolStripMenuItem.Text = "About Data Tidy";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // recentProjectContextMenuStrip
            // 
            this.recentProjectContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeRecentProjectToolStripMenuItem});
            this.recentProjectContextMenuStrip.Name = "recentProjectContextMenuStrip";
            this.recentProjectContextMenuStrip.Size = new System.Drawing.Size(126, 34);
            // 
            // removeRecentProjectToolStripMenuItem
            // 
            this.removeRecentProjectToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.RemoveRecentProject;
            this.removeRecentProjectToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeRecentProjectToolStripMenuItem.Name = "removeRecentProjectToolStripMenuItem";
            this.removeRecentProjectToolStripMenuItem.Size = new System.Drawing.Size(125, 30);
            this.removeRecentProjectToolStripMenuItem.Text = "Remove";
            this.removeRecentProjectToolStripMenuItem.Click += new System.EventHandler(this.RemoveRecentProjectToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 856);
            this.Controls.Add(this.mainDockPanel);
            this.Controls.Add(this.menuToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Tidy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.menuToolStrip.ResumeLayout(false);
            this.menuToolStrip.PerformLayout();
            this.recentProjectContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel mainDockPanel;
        private WeifenLuo.WinFormsUI.Docking.VS2005Theme vS2005Theme1;
        private System.Windows.Forms.ToolStrip menuToolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton executeToolStripButton;
        private System.Windows.Forms.ToolStripButton stopToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripDropDownButton windowsToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem viewProgressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewNotificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPerformanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataSourcesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openProjectDialog;
        private System.Windows.Forms.ToolStripDropDownButton fileToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip recentProjectContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeRecentProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton helpToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}