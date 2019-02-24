namespace ScottLane.DataTidy.Client.Forms
{
    partial class DataSourceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourceForm));
            this.dataSourceTreeView = new ScottLane.DataTidy.Client.Controls.BindableTreeView();
            this.dataSourceTreeViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionImageList = new System.Windows.Forms.ImageList(this.components);
            this.dataSourceContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewDataSourceSampleDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getRecordCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDataSourceInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.removeDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConnectionInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testOpenToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.addDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.clearCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.removeConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSourceTreeViewContextMenuStrip.SuspendLayout();
            this.dataSourceContextMenuStrip.SuspendLayout();
            this.connectionContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataSourceTreeView
            // 
            this.dataSourceTreeView.AllowDrop = true;
            this.dataSourceTreeView.ContextMenuStrip = this.dataSourceTreeViewContextMenuStrip;
            this.dataSourceTreeView.DataMember = null;
            this.dataSourceTreeView.DataSource = null;
            this.dataSourceTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataSourceTreeView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataSourceTreeView.ImageIndex = 0;
            this.dataSourceTreeView.ImageList = this.connectionImageList;
            this.dataSourceTreeView.Location = new System.Drawing.Point(0, 0);
            this.dataSourceTreeView.Margin = new System.Windows.Forms.Padding(1);
            this.dataSourceTreeView.Name = "dataSourceTreeView";
            this.dataSourceTreeView.SelectedImageIndex = 0;
            this.dataSourceTreeView.Size = new System.Drawing.Size(309, 286);
            this.dataSourceTreeView.TabIndex = 1;
            this.dataSourceTreeView.NodeDataBinding += new ScottLane.DataTidy.Client.Controls.NodeEventHandler(this.DataSourceTreeView_DataBinding);
            this.dataSourceTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.DataSourceTreeView_AfterCollapse);
            this.dataSourceTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.DataSourceTreeView_AfterExpand);
            this.dataSourceTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.DataSourceTreeView_ItemDrag);
            this.dataSourceTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DataSourceTreeView_AfterSelect);
            this.dataSourceTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.DataSourceTreeView_NodeMouseClick);
            this.dataSourceTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.DataSourceTreeView_DragDrop);
            this.dataSourceTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.DataSourceTreeView_DragEnter);
            this.dataSourceTreeView.Enter += new System.EventHandler(this.DataSourceTreeView_Enter);
            this.dataSourceTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DataSourceTreeView_KeyUp);
            // 
            // dataSourceTreeViewContextMenuStrip
            // 
            this.dataSourceTreeViewContextMenuStrip.Enabled = false;
            this.dataSourceTreeViewContextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.dataSourceTreeViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addConnectionToolStripMenuItem});
            this.dataSourceTreeViewContextMenuStrip.Name = "connectionRootContextMenuStrip";
            this.dataSourceTreeViewContextMenuStrip.Size = new System.Drawing.Size(178, 42);
            this.dataSourceTreeViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.AddConnectionTreeViewContextMenuStrip_Opening);
            // 
            // addConnectionToolStripMenuItem
            // 
            this.addConnectionToolStripMenuItem.Enabled = false;
            this.addConnectionToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Connection;
            this.addConnectionToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addConnectionToolStripMenuItem.Name = "addConnectionToolStripMenuItem";
            this.addConnectionToolStripMenuItem.Size = new System.Drawing.Size(177, 38);
            this.addConnectionToolStripMenuItem.Text = "Add Connection";
            // 
            // connectionImageList
            // 
            this.connectionImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("connectionImageList.ImageStream")));
            this.connectionImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.connectionImageList.Images.SetKeyName(0, "Connection.png");
            this.connectionImageList.Images.SetKeyName(1, "Clouds.png");
            this.connectionImageList.Images.SetKeyName(2, "CSV.png");
            this.connectionImageList.Images.SetKeyName(3, "XLS.png");
            this.connectionImageList.Images.SetKeyName(4, "Database.png");
            this.connectionImageList.Images.SetKeyName(5, "Dynamics.png");
            // 
            // dataSourceContextMenuStrip
            // 
            this.dataSourceContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.dataSourceContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewDataSourceSampleDataToolStripMenuItem,
            this.getRecordCountToolStripMenuItem,
            this.openDataSourceInBrowserToolStripMenuItem,
            this.duplicateToolStripSeparator,
            this.duplicateDataSourceToolStripMenuItem,
            this.toolStripSeparator11,
            this.removeDataSourceToolStripMenuItem});
            this.dataSourceContextMenuStrip.Name = "dataSourceContextMenuStrip";
            this.dataSourceContextMenuStrip.Size = new System.Drawing.Size(185, 206);
            this.dataSourceContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.DataSourceContextMenuStrip_Opening);
            // 
            // viewDataSourceSampleDataToolStripMenuItem
            // 
            this.viewDataSourceSampleDataToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.DataTable;
            this.viewDataSourceSampleDataToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.viewDataSourceSampleDataToolStripMenuItem.Name = "viewDataSourceSampleDataToolStripMenuItem";
            this.viewDataSourceSampleDataToolStripMenuItem.Size = new System.Drawing.Size(184, 38);
            this.viewDataSourceSampleDataToolStripMenuItem.Text = "View Sample Data";
            this.viewDataSourceSampleDataToolStripMenuItem.Click += new System.EventHandler(this.ViewDataSourceSampleDataToolStripMenuItem_Click);
            // 
            // getRecordCountToolStripMenuItem
            // 
            this.getRecordCountToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.RowCount;
            this.getRecordCountToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.getRecordCountToolStripMenuItem.Name = "getRecordCountToolStripMenuItem";
            this.getRecordCountToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.getRecordCountToolStripMenuItem.Size = new System.Drawing.Size(184, 38);
            this.getRecordCountToolStripMenuItem.Text = "Get Record Count";
            this.getRecordCountToolStripMenuItem.Click += new System.EventHandler(this.GetRecordCountToolStripMenuItem_Click);
            // 
            // openDataSourceInBrowserToolStripMenuItem
            // 
            this.openDataSourceInBrowserToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Browser;
            this.openDataSourceInBrowserToolStripMenuItem.Name = "openDataSourceInBrowserToolStripMenuItem";
            this.openDataSourceInBrowserToolStripMenuItem.Size = new System.Drawing.Size(184, 38);
            this.openDataSourceInBrowserToolStripMenuItem.Text = "Open In Browser";
            this.openDataSourceInBrowserToolStripMenuItem.Click += new System.EventHandler(this.OpenDataSourceInBrowserToolStripMenuItem_Click);
            // 
            // duplicateToolStripSeparator
            // 
            this.duplicateToolStripSeparator.Name = "duplicateToolStripSeparator";
            this.duplicateToolStripSeparator.Size = new System.Drawing.Size(181, 6);
            // 
            // duplicateDataSourceToolStripMenuItem
            // 
            this.duplicateDataSourceToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.duplicateDataSourceToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.duplicateDataSourceToolStripMenuItem.Name = "duplicateDataSourceToolStripMenuItem";
            this.duplicateDataSourceToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.duplicateDataSourceToolStripMenuItem.Size = new System.Drawing.Size(184, 38);
            this.duplicateDataSourceToolStripMenuItem.Text = "Duplicate";
            this.duplicateDataSourceToolStripMenuItem.Click += new System.EventHandler(this.DuplicateDataSourceToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(181, 6);
            // 
            // removeDataSourceToolStripMenuItem
            // 
            this.removeDataSourceToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Trash;
            this.removeDataSourceToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeDataSourceToolStripMenuItem.Name = "removeDataSourceToolStripMenuItem";
            this.removeDataSourceToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.removeDataSourceToolStripMenuItem.Size = new System.Drawing.Size(184, 38);
            this.removeDataSourceToolStripMenuItem.Text = "Remove";
            this.removeDataSourceToolStripMenuItem.Click += new System.EventHandler(this.RemoveDataSourceToolStripMenuItem_Click);
            // 
            // connectionContextMenuStrip
            // 
            this.connectionContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.connectionContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testConnectionToolStripMenuItem,
            this.openConnectionInBrowserToolStripMenuItem,
            this.testOpenToolStripSeparator,
            this.addDataSourceToolStripMenuItem,
            this.toolStripSeparator6,
            this.duplicateConnectionToolStripMenuItem,
            this.toolStripSeparator10,
            this.clearCacheToolStripMenuItem,
            this.toolStripSeparator7,
            this.removeConnectionToolStripMenuItem});
            this.connectionContextMenuStrip.Name = "connectionContextMenuStrip";
            this.connectionContextMenuStrip.Size = new System.Drawing.Size(183, 256);
            this.connectionContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ConnectionContextMenuStrip_Opening);
            // 
            // testConnectionToolStripMenuItem
            // 
            this.testConnectionToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.CheckConnectivity;
            this.testConnectionToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.testConnectionToolStripMenuItem.Name = "testConnectionToolStripMenuItem";
            this.testConnectionToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.testConnectionToolStripMenuItem.Text = "Test Connectivity";
            this.testConnectionToolStripMenuItem.Click += new System.EventHandler(this.TestConnectionToolStripMenuItem_Click);
            // 
            // openConnectionInBrowserToolStripMenuItem
            // 
            this.openConnectionInBrowserToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Browser;
            this.openConnectionInBrowserToolStripMenuItem.Name = "openConnectionInBrowserToolStripMenuItem";
            this.openConnectionInBrowserToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.openConnectionInBrowserToolStripMenuItem.Text = "Open In Browser";
            this.openConnectionInBrowserToolStripMenuItem.Click += new System.EventHandler(this.OpenConnectionInBrowserToolStripMenuItem_Click);
            // 
            // testOpenToolStripSeparator
            // 
            this.testOpenToolStripSeparator.Name = "testOpenToolStripSeparator";
            this.testOpenToolStripSeparator.Size = new System.Drawing.Size(179, 6);
            // 
            // addDataSourceToolStripMenuItem
            // 
            this.addDataSourceToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.AddDataSource;
            this.addDataSourceToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addDataSourceToolStripMenuItem.Name = "addDataSourceToolStripMenuItem";
            this.addDataSourceToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.addDataSourceToolStripMenuItem.Text = "Add Data Source";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(179, 6);
            // 
            // duplicateConnectionToolStripMenuItem
            // 
            this.duplicateConnectionToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.duplicateConnectionToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.duplicateConnectionToolStripMenuItem.Name = "duplicateConnectionToolStripMenuItem";
            this.duplicateConnectionToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.duplicateConnectionToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.duplicateConnectionToolStripMenuItem.Text = "Duplicate";
            this.duplicateConnectionToolStripMenuItem.Click += new System.EventHandler(this.DuplicateConnectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(179, 6);
            // 
            // clearCacheToolStripMenuItem
            // 
            this.clearCacheToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ClearCache;
            this.clearCacheToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.clearCacheToolStripMenuItem.Name = "clearCacheToolStripMenuItem";
            this.clearCacheToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.clearCacheToolStripMenuItem.Text = "Clear Cache";
            this.clearCacheToolStripMenuItem.Click += new System.EventHandler(this.ClearCacheToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(179, 6);
            // 
            // removeConnectionToolStripMenuItem
            // 
            this.removeConnectionToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Trash;
            this.removeConnectionToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeConnectionToolStripMenuItem.Name = "removeConnectionToolStripMenuItem";
            this.removeConnectionToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.removeConnectionToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.removeConnectionToolStripMenuItem.Text = "Remove";
            this.removeConnectionToolStripMenuItem.Click += new System.EventHandler(this.RemoveConnectionToolStripMenuItem_Click);
            // 
            // DataSourceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 286);
            this.Controls.Add(this.dataSourceTreeView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "DataSourceForm";
            this.Text = "Data Sources";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DataSourceForm_FormClosed);
            this.Load += new System.EventHandler(this.DataSourceForm_Load);
            this.dataSourceTreeViewContextMenuStrip.ResumeLayout(false);
            this.dataSourceContextMenuStrip.ResumeLayout(false);
            this.connectionContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScottLane.DataTidy.Client.Controls.BindableTreeView dataSourceTreeView;
        private System.Windows.Forms.ContextMenuStrip dataSourceContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewDataSourceSampleDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getRecordCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem removeDataSourceToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip dataSourceTreeViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addConnectionToolStripMenuItem;
        private System.Windows.Forms.ImageList connectionImageList;
        private System.Windows.Forms.ContextMenuStrip connectionContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem testConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem addDataSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem duplicateConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem removeConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator duplicateToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem duplicateDataSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openConnectionInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator testOpenToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem openDataSourceInBrowserToolStripMenuItem;
    }
}