namespace ScottLane.DataTidy.Client.Forms
{
    partial class SampleDataViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SampleDataViewer));
            this.dataTableGridView = new System.Windows.Forms.DataGridView();
            this.gridContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyWithHeadingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataTableGridView)).BeginInit();
            this.gridContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataTableGridView
            // 
            this.dataTableGridView.AllowUserToAddRows = false;
            this.dataTableGridView.AllowUserToDeleteRows = false;
            this.dataTableGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataTableGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataTableGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataTableGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataTableGridView.ContextMenuStrip = this.gridContextMenuStrip;
            this.dataTableGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTableGridView.Location = new System.Drawing.Point(0, 0);
            this.dataTableGridView.Name = "dataTableGridView";
            this.dataTableGridView.ReadOnly = true;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataTableGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataTableGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataTableGridView.Size = new System.Drawing.Size(1066, 799);
            this.dataTableGridView.TabIndex = 0;
            this.dataTableGridView.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataTableGridView_ColumnAdded);
            this.dataTableGridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DataTableGridView_KeyUp);
            // 
            // gridContextMenuStrip
            // 
            this.gridContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.copyWithHeadingsToolStripMenuItem});
            this.gridContextMenuStrip.Name = "gridContextMenuStrip";
            this.gridContextMenuStrip.Size = new System.Drawing.Size(184, 48);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // copyWithHeadingsToolStripMenuItem
            // 
            this.copyWithHeadingsToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.copyWithHeadingsToolStripMenuItem.Name = "copyWithHeadingsToolStripMenuItem";
            this.copyWithHeadingsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.copyWithHeadingsToolStripMenuItem.Text = "Copy With Headings";
            this.copyWithHeadingsToolStripMenuItem.Click += new System.EventHandler(this.CopyWithHeadingsToolStripMenuItem_Click);
            // 
            // SampleDataViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1066, 799);
            this.Controls.Add(this.dataTableGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SampleDataViewer";
            this.Text = "Sample Data Viewer";
            this.Load += new System.EventHandler(this.DataTableViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataTableGridView)).EndInit();
            this.gridContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataTableGridView;
        private System.Windows.Forms.ContextMenuStrip gridContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyWithHeadingsToolStripMenuItem;
    }
}