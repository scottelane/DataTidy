using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ScottLane.DataTidy.Client.Model;
using Equin.ApplicationFramework;

namespace ScottLane.DataTidy.Client.Forms
{
    /// <summary>
    /// http://blw.sourceforge.net/
    /// </summary>
    public partial class NotificationsForm : ChildForm
    {
        readonly TimeSpan THROTTLE_INTERVAL = TimeSpan.FromSeconds(1);
        const int MAXIMUM_NOTIFICATIONS_PER_INTERVAL = 10;
        const int MAXIMUM_MESSAGES_TO_DISPLAY = 500;

        int notificationsSinceLastInterval;
        int informationCount;
        int warningCount;
        int errorCount;
        DateTime lastIntervalOn;
        BindingList<NotificationEventArgs> notifications;
        BindingListView<NotificationEventArgs> notificationsView;

        // https://msdn.microsoft.com/en-us/library/ms993236.aspx
        public NotificationsForm()
        {
            InitializeComponent();

            ClientUtility.SetDoubleBuffered(NotificationDataGridView);

            notifications = new BindingList<NotificationEventArgs>();
            notificationsView = new BindingListView<NotificationEventArgs>(notifications);
            notificationsView.ListChanged += Notifications_ListChanged;

            NotificationDataGridView.AutoGenerateColumns = false;
            NotificationDataGridView.Columns[1].DataPropertyName = "Message";
            NotificationDataGridView.Columns[2].DataPropertyName = "TimeStamp";
            NotificationDataGridView.DataSource = notificationsView;
            SortDataGridView(NotificationDataGridView.Columns[2]);

            ApplicationState.Default.NotificationRaised += Default_NotificationRaised;
            ApplicationState.Default.AsyncProcessStopped += Default_AsyncProcessStopped;

            ResetCounts();
            UpdateNotificationFilter();
            UpdateFilterButtonText();
        }

        private void ResetCounts()
        {
            informationCount = 0;
            warningCount = 0;
            errorCount = 0;
        }

        private void Notifications_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateFilterButtonText();
        }

        private void Default_AsyncProcessStopped(object sender, AsyncStoppedEventArgs e)
        {
            UpdateFilterButtonText();
        }

        private void UpdateFilterButtonText()
        {
            messagesToolStripButton.Text = string.Format(Properties.Resources.NotificationFormMessagesText, informationCount);
            warningsToolStripButton.Text = string.Format(Properties.Resources.NotificationFormWarningsText, warningCount);
            errorsToolStripButton.Text = string.Format(Properties.Resources.NotificationFormErrorsText, errorCount);
        }

        private void Default_NotificationRaised(object sender, NotificationEventArgs e)
        {
            AddThrottledNotification(e);
        }

        private void AddThrottledNotification(NotificationEventArgs e)
        {
            if (lastIntervalOn == default(DateTime))
            {
                lastIntervalOn = DateTime.Now;
                notificationsSinceLastInterval = 0;
            }

            notificationsSinceLastInterval++;

            switch (e.NotificationType)
            {
                case Core.NotificationType.Information:
                    informationCount++;
                    break;
                case Core.NotificationType.Warning:
                    warningCount++;
                    break;
                case Core.NotificationType.Error:
                    errorCount++;
                    break;
            }

            if (notificationsSinceLastInterval <= MAXIMUM_NOTIFICATIONS_PER_INTERVAL)
            {
                notifications.Add(e);

                if (notifications.Count > MAXIMUM_MESSAGES_TO_DISPLAY)
                {
                    notifications.RemoveAt(0);
                }
            }

            if (DateTime.Now - lastIntervalOn > THROTTLE_INTERVAL)
            {
                lastIntervalOn = DateTime.Now;
                notificationsSinceLastInterval = 0;
            }
        }

        private void MessagesToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNotificationFilter();
        }

        private void WarningsToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNotificationFilter();
        }

        private void ErrorsToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNotificationFilter();
        }

        private void UpdateNotificationFilter()
        {
            notificationsView.ApplyFilter(n => n.NotificationType == Core.NotificationType.Information && messagesToolStripButton.Checked ||
                                                n.NotificationType == Core.NotificationType.Warning && warningsToolStripButton.Checked ||
                                                n.NotificationType == Core.NotificationType.Error && errorsToolStripButton.Checked);
        }

        private void ClearAllToolStripButton_Click(object sender, EventArgs e)
        {
            ResetCounts();
            notifications.Clear();
        }

        private void NotificationDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            NotificationEventArgs notification = ((ObjectView<NotificationEventArgs>)NotificationDataGridView.Rows[e.RowIndex].DataBoundItem).Object;

            if (e.ColumnIndex == 0)
            {
                if (notification.NotificationType == Core.NotificationType.Error)
                {
                    e.Value = Properties.Resources.NotificationsFormError;
                }
                else if (notification.NotificationType == Core.NotificationType.Information)
                {
                    e.Value = Properties.Resources.Information;
                }
                else if (notification.NotificationType == Core.NotificationType.Warning)
                {
                    e.Value = Properties.Resources.NotificationsFormWarning;
                }
            }
        }

        private void NotificationDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            NotificationDataGridView.ClearSelection();
        }

        private void NotificationDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn column = NotificationDataGridView.Columns[e.ColumnIndex];
            SortDataGridView(column);
        }

        private void SortDataGridView(DataGridViewColumn column)
        {
            if (column.SortMode == DataGridViewColumnSortMode.Automatic)
            {
                notificationsView.ApplySort(string.Format("{0} {1}", column.DataPropertyName, column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "ASC" : "DESC"));
            }
        }

        private void NotificationDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NotificationEventArgs notification = ((ObjectView<NotificationEventArgs>)NotificationDataGridView.Rows[e.RowIndex].DataBoundItem).Object;
            ApplicationState.Default.SelectedError = notification;
        }

        private void NotificationDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // todo - is this doing anything? remove?
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTest = NotificationDataGridView.HitTest(e.X, e.Y);
            }
        }

        private void NotificationDataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
                {
                    object data = NotificationDataGridView.GetClipboardContent();

                    if (data != default(object))
                    {
                        Clipboard.SetDataObject(data);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                object data = NotificationDataGridView.GetClipboardContent();

                if (data != default(object))
                {
                    Clipboard.SetDataObject(data);
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }

        private void NotificationsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ApplicationState.Default.NotificationRaised -= Default_NotificationRaised;
        }

        private void NotificationContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = ApplicationState.Default.AsyncProcessRunning;
        }

        private void viewErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(ClientUtility.ErrorLogPath))
                {
                    Process.Start(ClientUtility.ErrorLogPath);
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }

        private void viewNotificationLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(ClientUtility.NotificationLogPath))
                {
                    Process.Start(ClientUtility.NotificationLogPath);
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }

        private void openLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(ClientUtility.LogFolder))
                {
                    Process.Start(ClientUtility.LogFolder);
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }

        private void clearLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(ClientUtility.NotificationLogPath))
                {
                    System.IO.File.Delete(ClientUtility.NotificationLogPath);
                }

                if (System.IO.File.Exists(ClientUtility.ErrorLogPath))
                {
                    System.IO.File.Delete(ClientUtility.ErrorLogPath);
                }

                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Information, "All logs were cleared successfully"));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }

        private void viewLogsToolStripDropDownButton_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                viewNotificationLogToolStripMenuItem.Enabled = System.IO.File.Exists(ClientUtility.NotificationLogPath);
                viewErrorLogToolStripMenuItem.Enabled = System.IO.File.Exists(ClientUtility.ErrorLogPath);
                openLogFolderToolStripMenuItem.Enabled = Directory.Exists(ClientUtility.LogFolder);
                clearLogsToolStripMenuItem.Enabled = System.IO.File.Exists(ClientUtility.NotificationLogPath) || System.IO.File.Exists(ClientUtility.ErrorLogPath);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(Core.NotificationType.Error, ex.Message, ex));
            }
        }
    }
}
