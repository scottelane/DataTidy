namespace ScottLane.DataTidy.Client.Forms
{
    partial class ProjectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectForm));
            this.batchTreeViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchImageList = new System.Windows.Forms.ImageList(this.components);
            this.operationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.executeOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.enableOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customStartToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.executeBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectDialog = new System.Windows.Forms.SaveFileDialog();
            this.batchTreeView = new ScottLane.DataTidy.Client.Controls.BindableTreeView();
            this.customEndToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.batchTreeViewContextMenuStrip.SuspendLayout();
            this.operationContextMenuStrip.SuspendLayout();
            this.batchContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // batchTreeViewContextMenuStrip
            // 
            this.batchTreeViewContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.batchTreeViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBatchToolStripMenuItem});
            this.batchTreeViewContextMenuStrip.Name = "projectContextMenuStrip";
            this.batchTreeViewContextMenuStrip.Size = new System.Drawing.Size(146, 42);
            // 
            // addBatchToolStripMenuItem
            // 
            this.addBatchToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ProjectFormBatch;
            this.addBatchToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addBatchToolStripMenuItem.Name = "addBatchToolStripMenuItem";
            this.addBatchToolStripMenuItem.Size = new System.Drawing.Size(145, 38);
            this.addBatchToolStripMenuItem.Text = "Add Batch";
            this.addBatchToolStripMenuItem.Click += new System.EventHandler(this.AddBatchToolStripMenuItem_Click);
            // 
            // batchImageList
            // 
            this.batchImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.batchImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.batchImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // operationContextMenuStrip
            // 
            this.operationContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.operationContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executeOperationToolStripMenuItem,
            this.toolStripSeparator3,
            this.enableOperationToolStripMenuItem,
            this.disableOperationToolStripMenuItem,
            this.customStartToolStripSeparator,
            this.customEndToolStripSeparator,
            this.duplicateOperationToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeOperationToolStripMenuItem});
            this.operationContextMenuStrip.Name = "stepContextMenuStrip";
            this.operationContextMenuStrip.Size = new System.Drawing.Size(183, 240);
            this.operationContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.OperationContextMenuStrip_Opening);
            // 
            // executeOperationToolStripMenuItem
            // 
            this.executeOperationToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Go;
            this.executeOperationToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.executeOperationToolStripMenuItem.Name = "executeOperationToolStripMenuItem";
            this.executeOperationToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.executeOperationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.executeOperationToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.executeOperationToolStripMenuItem.Text = "Execute";
            this.executeOperationToolStripMenuItem.Click += new System.EventHandler(this.ExecuteOperationToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(179, 6);
            // 
            // enableOperationToolStripMenuItem
            // 
            this.enableOperationToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ConnectionEnable32;
            this.enableOperationToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.enableOperationToolStripMenuItem.Name = "enableOperationToolStripMenuItem";
            this.enableOperationToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.enableOperationToolStripMenuItem.Text = "Enable";
            this.enableOperationToolStripMenuItem.Visible = false;
            this.enableOperationToolStripMenuItem.Click += new System.EventHandler(this.EnableOperationToolStripMenuItem_Click);
            // 
            // disableOperationToolStripMenuItem
            // 
            this.disableOperationToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ConnectionDisable32;
            this.disableOperationToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.disableOperationToolStripMenuItem.Name = "disableOperationToolStripMenuItem";
            this.disableOperationToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.disableOperationToolStripMenuItem.Text = "Disable";
            this.disableOperationToolStripMenuItem.Click += new System.EventHandler(this.DisableOperationToolStripMenuItem_Click);
            // 
            // customStartToolStripSeparator
            // 
            this.customStartToolStripSeparator.Name = "customStartToolStripSeparator";
            this.customStartToolStripSeparator.Size = new System.Drawing.Size(179, 6);
            // 
            // duplicateOperationToolStripMenuItem
            // 
            this.duplicateOperationToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.duplicateOperationToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.duplicateOperationToolStripMenuItem.Name = "duplicateOperationToolStripMenuItem";
            this.duplicateOperationToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.duplicateOperationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateOperationToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.duplicateOperationToolStripMenuItem.Text = "Duplicate";
            this.duplicateOperationToolStripMenuItem.Click += new System.EventHandler(this.DuplicateOperationToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(179, 6);
            // 
            // removeOperationToolStripMenuItem
            // 
            this.removeOperationToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Trash;
            this.removeOperationToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeOperationToolStripMenuItem.Name = "removeOperationToolStripMenuItem";
            this.removeOperationToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.removeOperationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeOperationToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.removeOperationToolStripMenuItem.Text = "Remove";
            this.removeOperationToolStripMenuItem.Click += new System.EventHandler(this.RemoveOperationToolStripMenuItem_Click);
            // 
            // batchContextMenuStrip
            // 
            this.batchContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.batchContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executeBatchToolStripMenuItem,
            this.toolStripSeparator2,
            this.addOperationToolStripMenuItem,
            this.toolStripSeparator9,
            this.duplicateBatchToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeBatchToolStripMenuItem});
            this.batchContextMenuStrip.Name = "batchContextMenuStrip";
            this.batchContextMenuStrip.Size = new System.Drawing.Size(183, 174);
            this.batchContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.BatchContextMenuStrip_Opening);
            // 
            // executeBatchToolStripMenuItem
            // 
            this.executeBatchToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Go;
            this.executeBatchToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.executeBatchToolStripMenuItem.Name = "executeBatchToolStripMenuItem";
            this.executeBatchToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+R";
            this.executeBatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.executeBatchToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.executeBatchToolStripMenuItem.Text = "Execute";
            this.executeBatchToolStripMenuItem.Click += new System.EventHandler(this.ExecuteBatchToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // addOperationToolStripMenuItem
            // 
            this.addOperationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addOperationToolStripMenuItem.Image")));
            this.addOperationToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addOperationToolStripMenuItem.Name = "addOperationToolStripMenuItem";
            this.addOperationToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.addOperationToolStripMenuItem.Text = "Add Operation";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(179, 6);
            // 
            // duplicateBatchToolStripMenuItem
            // 
            this.duplicateBatchToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.duplicateBatchToolStripMenuItem.Name = "duplicateBatchToolStripMenuItem";
            this.duplicateBatchToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.duplicateBatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateBatchToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.duplicateBatchToolStripMenuItem.Text = "Duplicate";
            this.duplicateBatchToolStripMenuItem.Click += new System.EventHandler(this.DuplicateBatchToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // removeBatchToolStripMenuItem
            // 
            this.removeBatchToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Trash;
            this.removeBatchToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeBatchToolStripMenuItem.Name = "removeBatchToolStripMenuItem";
            this.removeBatchToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.removeBatchToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeBatchToolStripMenuItem.Size = new System.Drawing.Size(182, 38);
            this.removeBatchToolStripMenuItem.Text = "Remove";
            this.removeBatchToolStripMenuItem.Click += new System.EventHandler(this.RemoveBatchToolStripMenuItem_Click);
            // 
            // batchTreeView
            // 
            this.batchTreeView.AllowDrop = true;
            this.batchTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.batchTreeView.ContextMenuStrip = this.batchTreeViewContextMenuStrip;
            this.batchTreeView.DataMember = null;
            this.batchTreeView.DataSource = null;
            this.batchTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.batchTreeView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.batchTreeView.ImageIndex = 0;
            this.batchTreeView.ImageList = this.batchImageList;
            this.batchTreeView.Location = new System.Drawing.Point(0, 0);
            this.batchTreeView.Margin = new System.Windows.Forms.Padding(1);
            this.batchTreeView.Name = "batchTreeView";
            this.batchTreeView.SelectedImageIndex = 0;
            this.batchTreeView.Size = new System.Drawing.Size(120, 46);
            this.batchTreeView.TabIndex = 6;
            this.batchTreeView.NodeDataBinding += new ScottLane.DataTidy.Client.Controls.NodeEventHandler(this.BatchTreeView_DataBinding);
            this.batchTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.BatchTreeView_AfterLabelEdit);
            this.batchTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.BatchTreeView_AfterCollapse);
            this.batchTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.BatchTreeView_AfterExpand);
            this.batchTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.BatchTreeView_ItemDrag);
            this.batchTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.BatchTreeView_AfterSelect);
            this.batchTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.BatchTreeView_NodeMouseClick);
            this.batchTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.BatchTreeView_DragDrop);
            this.batchTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.BatchTreeView_DragEnter);
            this.batchTreeView.Enter += new System.EventHandler(this.BatchTreeView_Enter);
            this.batchTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BatchTreeView_KeyUp);
            // 
            // customEndToolStripSeparator
            // 
            this.customEndToolStripSeparator.Name = "customEndToolStripSeparator";
            this.customEndToolStripSeparator.Size = new System.Drawing.Size(179, 6);
            // 
            // ProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 46);
            this.Controls.Add(this.batchTreeView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "ProjectForm";
            this.Text = "New Project";
            this.Activated += new System.EventHandler(this.ProjectForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProjectForm_FormClosed);
            this.batchTreeViewContextMenuStrip.ResumeLayout(false);
            this.operationContextMenuStrip.ResumeLayout(false);
            this.batchContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip operationContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem executeOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator customStartToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem duplicateOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeOperationToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip batchTreeViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addBatchToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip batchContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem executeBatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem duplicateBatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem addOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeBatchToolStripMenuItem;
        private System.Windows.Forms.ImageList batchImageList;
        private System.Windows.Forms.SaveFileDialog saveProjectDialog;
        private ScottLane.DataTidy.Client.Controls.BindableTreeView batchTreeView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem enableOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator customEndToolStripSeparator;
    }
}