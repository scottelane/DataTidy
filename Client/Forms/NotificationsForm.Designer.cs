namespace ScottLane.DataTidy.Client.Forms
{
    partial class NotificationsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.messagesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.warningsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.errorsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.viewLogsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.viewErrorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNotificationLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NotificationDataGridView = new System.Windows.Forms.DataGridView();
            this.NotificationType = new System.Windows.Forms.DataGridViewImageColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeStamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NotificationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NotificationDataGridView)).BeginInit();
            this.NotificationContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messagesToolStripButton,
            this.toolStripSeparator2,
            this.warningsToolStripButton,
            this.toolStripSeparator3,
            this.errorsToolStripButton,
            this.toolStripSeparator1,
            this.clearAllToolStripButton,
            this.toolStripSeparator4,
            this.viewLogsToolStripDropDownButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolStrip1.Size = new System.Drawing.Size(1031, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // messagesToolStripButton
            // 
            this.messagesToolStripButton.Checked = true;
            this.messagesToolStripButton.CheckOnClick = true;
            this.messagesToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.messagesToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Information;
            this.messagesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messagesToolStripButton.Name = "messagesToolStripButton";
            this.messagesToolStripButton.Size = new System.Drawing.Size(95, 20);
            this.messagesToolStripButton.Text = "Messages (0)";
            this.messagesToolStripButton.CheckedChanged += new System.EventHandler(this.MessagesToolStripButton_CheckedChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // warningsToolStripButton
            // 
            this.warningsToolStripButton.Checked = true;
            this.warningsToolStripButton.CheckOnClick = true;
            this.warningsToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.warningsToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsFormWarning;
            this.warningsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.warningsToolStripButton.Name = "warningsToolStripButton";
            this.warningsToolStripButton.Size = new System.Drawing.Size(94, 20);
            this.warningsToolStripButton.Text = "Warnings (0)";
            this.warningsToolStripButton.CheckedChanged += new System.EventHandler(this.WarningsToolStripButton_CheckedChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // errorsToolStripButton
            // 
            this.errorsToolStripButton.Checked = true;
            this.errorsToolStripButton.CheckOnClick = true;
            this.errorsToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.errorsToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsFormError;
            this.errorsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.errorsToolStripButton.Name = "errorsToolStripButton";
            this.errorsToolStripButton.Size = new System.Drawing.Size(74, 20);
            this.errorsToolStripButton.Text = "Errors (0)";
            this.errorsToolStripButton.CheckedChanged += new System.EventHandler(this.ErrorsToolStripButton_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // clearAllToolStripButton
            // 
            this.clearAllToolStripButton.Image = global::ScottLane.DataTidy.Client.Properties.Resources.ClearNotifications;
            this.clearAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearAllToolStripButton.Name = "clearAllToolStripButton";
            this.clearAllToolStripButton.Size = new System.Drawing.Size(71, 20);
            this.clearAllToolStripButton.Text = "Clear All";
            this.clearAllToolStripButton.Click += new System.EventHandler(this.ClearAllToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // viewLogsToolStripDropDownButton
            // 
            this.viewLogsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewLogsToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewErrorLogToolStripMenuItem,
            this.viewNotificationLogToolStripMenuItem,
            this.openLogFolderToolStripMenuItem,
            this.clearLogsToolStripMenuItem});
            this.viewLogsToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("viewLogsToolStripDropDownButton.Image")));
            this.viewLogsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewLogsToolStripDropDownButton.Name = "viewLogsToolStripDropDownButton";
            this.viewLogsToolStripDropDownButton.Size = new System.Drawing.Size(73, 20);
            this.viewLogsToolStripDropDownButton.Text = "View Logs";
            this.viewLogsToolStripDropDownButton.DropDownOpening += new System.EventHandler(this.viewLogsToolStripDropDownButton_DropDownOpening);
            // 
            // viewErrorLogToolStripMenuItem
            // 
            this.viewErrorLogToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsFormViewErrorLog;
            this.viewErrorLogToolStripMenuItem.Name = "viewErrorLogToolStripMenuItem";
            this.viewErrorLogToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.viewErrorLogToolStripMenuItem.Text = "View Error Log";
            this.viewErrorLogToolStripMenuItem.Click += new System.EventHandler(this.viewErrorLogToolStripMenuItem_Click);
            // 
            // viewNotificationLogToolStripMenuItem
            // 
            this.viewNotificationLogToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsFormViewNotificationLog;
            this.viewNotificationLogToolStripMenuItem.Name = "viewNotificationLogToolStripMenuItem";
            this.viewNotificationLogToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.viewNotificationLogToolStripMenuItem.Text = "View Notification Log";
            this.viewNotificationLogToolStripMenuItem.Click += new System.EventHandler(this.viewNotificationLogToolStripMenuItem_Click);
            // 
            // openLogFolderToolStripMenuItem
            // 
            this.openLogFolderToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsFormOpenLogFolder;
            this.openLogFolderToolStripMenuItem.Name = "openLogFolderToolStripMenuItem";
            this.openLogFolderToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openLogFolderToolStripMenuItem.Text = "Open Log Folder";
            this.openLogFolderToolStripMenuItem.Click += new System.EventHandler(this.openLogFolderToolStripMenuItem_Click);
            // 
            // clearLogsToolStripMenuItem
            // 
            this.clearLogsToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.NotificationsFormClearLogs;
            this.clearLogsToolStripMenuItem.Name = "clearLogsToolStripMenuItem";
            this.clearLogsToolStripMenuItem.ShowShortcutKeys = false;
            this.clearLogsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.clearLogsToolStripMenuItem.Text = "Clear Logs";
            this.clearLogsToolStripMenuItem.Click += new System.EventHandler(this.clearLogsToolStripMenuItem_Click);
            // 
            // NotificationDataGridView
            // 
            this.NotificationDataGridView.AllowUserToAddRows = false;
            this.NotificationDataGridView.AllowUserToDeleteRows = false;
            this.NotificationDataGridView.AllowUserToResizeRows = false;
            this.NotificationDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.NotificationDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NotificationDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.NotificationDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.NotificationDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.NotificationDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NotificationType,
            this.Message,
            this.TimeStamp});
            this.NotificationDataGridView.ContextMenuStrip = this.NotificationContextMenuStrip;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.NotificationDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.NotificationDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NotificationDataGridView.Location = new System.Drawing.Point(0, 27);
            this.NotificationDataGridView.MultiSelect = false;
            this.NotificationDataGridView.Name = "NotificationDataGridView";
            this.NotificationDataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.NotificationDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.NotificationDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotificationDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.NotificationDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.NotificationDataGridView.Size = new System.Drawing.Size(1031, 281);
            this.NotificationDataGridView.TabIndex = 1;
            this.NotificationDataGridView.TabStop = false;
            this.NotificationDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.NotificationDataGridView_CellDoubleClick);
            this.NotificationDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.NotificationDataGridView_CellFormatting);
            this.NotificationDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.NotificationDataGridView_ColumnHeaderMouseClick);
            this.NotificationDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.NotificationDataGridView_DataBindingComplete);
            this.NotificationDataGridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NotificationDataGridView_KeyUp);
            this.NotificationDataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NotificationDataGridView_MouseDown);
            // 
            // NotificationType
            // 
            this.NotificationType.HeaderText = "";
            this.NotificationType.Name = "NotificationType";
            this.NotificationType.ReadOnly = true;
            this.NotificationType.Width = 32;
            // 
            // Message
            // 
            this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TimeStamp
            // 
            this.TimeStamp.HeaderText = "Timestamp";
            this.TimeStamp.Name = "TimeStamp";
            this.TimeStamp.ReadOnly = true;
            this.TimeStamp.Width = 150;
            // 
            // NotificationContextMenuStrip
            // 
            this.NotificationContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.NotificationContextMenuStrip.Name = "notificationContextMenuStrip";
            this.NotificationContextMenuStrip.Size = new System.Drawing.Size(145, 26);
            this.NotificationContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.NotificationContextMenuStrip_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::ScottLane.DataTidy.Client.Properties.Resources.Copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // bindingSource
            // 
            this.bindingSource.Sort = "TimeStamp DESC";
            // 
            // NotificationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1031, 308);
            this.Controls.Add(this.NotificationDataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NotificationsForm";
            this.Text = "Notifications";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NotificationsForm_FormClosed);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NotificationDataGridView)).EndInit();
            this.NotificationContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton warningsToolStripButton;
        private System.Windows.Forms.ToolStripButton errorsToolStripButton;
        private System.Windows.Forms.DataGridView NotificationDataGridView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton clearAllToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton messagesToolStripButton;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeStamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewImageColumn NotificationType;
        private System.Windows.Forms.ContextMenuStrip NotificationContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton viewLogsToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem viewErrorLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewNotificationLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearLogsToolStripMenuItem;
    }
}