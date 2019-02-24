using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottLane.DataTidy.Core;
using ScottLane.DataTidy.Client.Controls;
using ScottLane.DataTidy.Client.Model;

namespace ScottLane.DataTidy.Client.Forms
{
    public partial class DataSourceForm : ChildForm
    {
        public DataSourceForm()
        {
            InitializeComponent();

            ApplicationState.Default.ActiveProjectChanged += ApplicationState_ActiveProjectChanged;
            ApplicationState.Default.AsyncProcessStarted += ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.AsyncProcessStopped += ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.SelectedItemChanged += ApplicationState_SelectedItemChanged;
            ApplicationState.Default.SelectedItemPropertyChanged += ApplicationState_SelectedItemPropertyChanged;
        }

        private void ApplicationState_SelectedItemPropertyChanged(object sender, ObjectEventArgs e)
        {
            // todo - remove after treeview inotifypropertychanged event bug fixed
            dataSourceTreeView.Refresh();
        }

        private void ApplicationState_SelectedItemChanged(object sender, ObjectEventArgs e)
        {
            if (Visible && e.Item is INotifyPropertyChanged item)
            {
                item.PropertyChanged += SelectedItem_PropertyChanged;
            }
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void ApplicationState_ActiveProjectChanged(object sender, ProjectEventArgs e)
        {
            SetActiveProject(e.Project);
        }

        private void SetActiveProject(Project project)
        {
            dataSourceTreeView.DataSource = project;
            dataSourceTreeView.DataMember = nameof(Project.Connections);
            dataSourceTreeView.SetExpandedState(project?.DataSourceState);
            dataSourceTreeView.ContextMenuStrip = project != default(Project) ? dataSourceTreeViewContextMenuStrip : default(ContextMenuStrip);
            dataSourceTreeViewContextMenuStrip.Enabled = project != default(Project);
        }

        private void ApplicationState_AsyncProcessStarted(object sender, AsyncEventArgs e)
        {
            dataSourceTreeView.Enabled = false;
            dataSourceTreeView.ContextMenuStrip.Enabled = false;
        }

        private void ApplicationState_AsyncProcessStopped(object sender, EventArgs e)
        {
            dataSourceTreeView.Enabled = true;
            dataSourceTreeView.ContextMenuStrip.Enabled = true;
        }

        private void DataSourceForm_Load(object sender, EventArgs e)
        {
            SetActiveProject(ApplicationState.Default.ActiveProject);
        }

        #region TreeView Events

        private void DataSourceTreeView_DataBinding(object sender, NodeEventArgs e)
        {
            try
            {
                // set the data source and image of each node
                if (e.Node.DataSource is IConnection connection)
                {
                    e.Node.DataMember = nameof(connection.DataSources);
                    e.Node.DisplayMember = nameof(connection.Name);
                    e.Node.ContextMenuStrip = connectionContextMenuStrip;
                    e.Node.ImageKey = connection.GetType().FullName;
                    e.Node.SelectedImageKey = e.Node.ImageKey;
                    e.Node.ID = connection.ID;
                    AddConnectionImage(connection.GetType());
                }
                else if (e.Node.DataSource is IDataSource dataSource)
                {
                    e.Node.DisplayMember = nameof(dataSource.Name);
                    e.Node.ContextMenuStrip = dataSourceContextMenuStrip;
                    e.Node.ImageKey = dataSource.GetType().FullName;
                    e.Node.SelectedImageKey = e.Node.ImageKey;
                    e.Node.ID = dataSource.ID;
                    AddDataSourceImage(dataSource.GetType());
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddConnectionImage(Type type)
        {
            // find the image corresponding with the connection so it can be displayed in the TreeView
            ConnectionAttribute attribute = (ConnectionAttribute)Attribute.GetCustomAttribute(type, typeof(ConnectionAttribute));

            if (attribute?.IconResourceName != null && !connectionImageList.Images.ContainsKey(type.FullName))
            {
                connectionImageList.Images.Add(type.FullName, attribute.GetIcon(type.Assembly));
            }
        }

        private void AddDataSourceImage(Type type)
        {
            DataSourceAttribute attribute = (DataSourceAttribute)Attribute.GetCustomAttribute(type, typeof(DataSourceAttribute));

            if (attribute?.IconResourceName != null && !connectionImageList.Images.ContainsKey(type.FullName))
            {
                connectionImageList.Images.Add(type.FullName, attribute.GetIcon(type.Assembly));
            }
        }

        /// <summary>
        /// Focuses a node selected with a mouse button.
        /// </summary>
        private void DataSourceTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    dataSourceTreeView.SelectedNode = e.Node;
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Notifies the application that a node has been selected.
        /// </summary>
        private void DataSourceTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                ApplicationState.Default.SelectedItem = ((BindableTreeNode)e.Node).DataSource;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Notifies the application that a node has been selected.
        /// </summary>
        private void DataSourceTreeView_Enter(object sender, EventArgs e)
        {
            try
            {
                if (dataSourceTreeView.SelectedNode != default(BindableTreeNode))
                {
                    ApplicationState.Default.SelectedItem = ((BindableTreeNode)dataSourceTreeView.SelectedNode).DataSource;
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Actions keyboard shortcuts.
        /// </summary>
        private void DataSourceTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (dataSourceTreeView.SelectedNode != default(TreeNode))
                {
                    object selectedNodeDataSource = ((BindableTreeNode)dataSourceTreeView.SelectedNode).DataSource;

                    if (e.KeyCode == Keys.Delete && selectedNodeDataSource is IConnection)
                    {
                        RemoveConnection(dataSourceTreeView.SelectedNode.Index, true);
                    }
                    else if (e.KeyCode == Keys.Delete && selectedNodeDataSource is IDataSource)
                    {
                        RemoveDataSource((BindableTreeNode)dataSourceTreeView.SelectedNode.Parent, dataSourceTreeView.SelectedNode.Index, true);
                    }
                    else if (e.KeyCode == Keys.F2)
                    {
                        dataSourceTreeView.SelectedNode.BeginEdit();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D && selectedNodeDataSource is IConnection)
                    {
                        DuplicateConnection((BindableTreeNode)dataSourceTreeView.SelectedNode);
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D && selectedNodeDataSource is IDataSource)
                    {
                        DuplicateDataSource((BindableTreeNode)dataSourceTreeView.SelectedNode);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DataSourceTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                DoDragDrop(e.Item, DragDropEffects.Copy | DragDropEffects.Move);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DataSourceTreeView_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effect = DragDropEffects.Move;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Performs drag and drop operations to re-arrange or duplicate connections or data sources.
        /// </summary>
        private void DataSourceTreeView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    BindableTreeNode draggedNode = (BindableTreeNode)e.Data.GetData(typeof(BindableTreeNode));
                    BindableTreeNode droppedNode = (BindableTreeNode)dataSourceTreeView.HitTest(dataSourceTreeView.PointToClient(new Point(e.X, e.Y)))?.Node;

                    if (draggedNode.DataSource is IConnection && droppedNode.DataSource is IConnection && draggedNode.ID != droppedNode.ID)
                    {
                        // rearrange connections
                        bool expanded = droppedNode.IsExpanded;
                        int droppedIndex = droppedNode.Index;
                        RemoveConnection(draggedNode.Index, false);
                        AddConnection((IConnection)draggedNode.DataSource, droppedIndex);

                        if (expanded)
                        {
                            dataSourceTreeView.SelectedNode.Expand();
                        }
                    }
                    else if (draggedNode.DataSource is IDataSource && droppedNode.DataSource is IDataSource && draggedNode.Parent == droppedNode.Parent && draggedNode.ID != droppedNode.ID)
                    {
                        // rearrange data sources
                        int droppedIndex = droppedNode.Index;
                        RemoveDataSource((BindableTreeNode)draggedNode.Parent, draggedNode.Index, false);
                        AddDataSource((IDataSource)draggedNode.DataSource, (BindableTreeNode)droppedNode.Parent, droppedIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Stores the expanded state so the application is in the same state next time the project is opened.
        /// </summary>
        private void DataSourceTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            try
            {
                ApplicationState.Default.ActiveProject.DataSourceState = dataSourceTreeView.GetExpandedState();
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Stores the expanded state so the application is in the same state next time the project is opened.
        /// </summary>
        private void DataSourceTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                ApplicationState.Default.ActiveProject.DataSourceState = dataSourceTreeView.GetExpandedState();
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        #endregion

        #region Connection Commands

        private void ConnectionContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                BindableTreeNode connectionNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                Type connectionType = connectionNode.DataSource.GetType();
                ConnectionAttribute connectionAttribute = (ConnectionAttribute)Attribute.GetCustomAttribute(connectionType, typeof(ConnectionAttribute));
                IConnection connection = (IConnection)connectionNode.DataSource;
                testConnectionToolStripMenuItem.Visible = connection is IAvailableProvider;
                openConnectionInBrowserToolStripMenuItem.Visible = connection is IUrlAddressable;
                openConnectionInBrowserToolStripMenuItem.Enabled = connection is IUrlAddressable && ((IUrlAddressable)connection).Url != default(Uri);
                testOpenToolStripSeparator.Visible = connection is IAvailableProvider || connection is IUrlAddressable;
                ConnectionCache cache = new ConnectionCache((IConnection)connectionNode.DataSource);
                clearCacheToolStripMenuItem.Enabled = cache.Count > 0;

                addDataSourceToolStripMenuItem.DropDownItems.Clear();
                List<Type> dataSourceTypes = CoreUtility.GetInterfaceImplementorsWithAttribute(typeof(IDataSource), typeof(DataSourceAttribute));

                foreach (Type dataSourceType in dataSourceTypes)
                {
                    DataSourceAttribute dataSourceAttribute = (DataSourceAttribute)Attribute.GetCustomAttribute(dataSourceType, typeof(DataSourceAttribute));

                    if (connectionType == dataSourceAttribute.ConnectionType || connectionType.IsSubclassOf(dataSourceAttribute.ConnectionType))
                    {
                        ToolStripMenuItem dataSourceToolStripMenuItem = new ToolStripMenuItem()
                        {
                            ImageScaling = ToolStripItemImageScaling.None,
                            Tag = dataSourceType
                        };
                        dataSourceToolStripMenuItem.Click += AddDataSourceToolStripMenuItem_Click;
                        dataSourceToolStripMenuItem.Text = dataSourceAttribute.DisplayName;
                        dataSourceToolStripMenuItem.ToolTipText = dataSourceAttribute.Description;
                        dataSourceToolStripMenuItem.Image = dataSourceAttribute.GetIcon(dataSourceType.Assembly);
                        addDataSourceToolStripMenuItem.DropDownItems.Add(dataSourceToolStripMenuItem);
                    }
                }

                addDataSourceToolStripMenuItem.Enabled = addDataSourceToolStripMenuItem.DropDownItems.Count > 0;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddConnectionTreeViewContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // dynamically create a drop down list of all connections available in plugins
                addConnectionToolStripMenuItem.DropDownItems.Clear();
                List<Type> connectionTypes = CoreUtility.GetInterfaceImplementorsWithAttribute(typeof(IConnection), typeof(ConnectionAttribute));

                foreach (Type connectionType in connectionTypes)
                {
                    ConnectionAttribute connectionAttribute = (ConnectionAttribute)Attribute.GetCustomAttribute(connectionType, typeof(ConnectionAttribute));
                    ToolStripMenuItem addConnectionToolStripMenuItem = new ToolStripMenuItem(connectionType.Name)
                    {
                        ImageScaling = ToolStripItemImageScaling.None,
                        Tag = connectionType
                    };
                    addConnectionToolStripMenuItem.Click += AddConnectionToolStripMenuItem_Click;
                    addConnectionToolStripMenuItem.Text = connectionAttribute.DisplayName;
                    addConnectionToolStripMenuItem.ToolTipText = connectionAttribute.Description;
                    addConnectionToolStripMenuItem.Image = connectionAttribute.GetIcon(connectionType.Assembly);

                    this.addConnectionToolStripMenuItem.DropDownItems.Add(addConnectionToolStripMenuItem);
                }

                addConnectionToolStripMenuItem.Enabled = addConnectionToolStripMenuItem.DropDownItems.Count > 0;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                Type connectionType = (Type)menuItem.Tag;
                IConnection connection = (IConnection)Activator.CreateInstance(connectionType, new object[] { ApplicationState.Default.ActiveProject });
                ConnectionAttribute attribute = (ConnectionAttribute)Attribute.GetCustomAttribute(connectionType, typeof(ConnectionAttribute));
                connection.Name = attribute.DisplayName;
                AddConnection(connection, ApplicationState.Default.ActiveProject.Connections.Count);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddConnection(IConnection connection, int connectionIndex)
        {
            ApplicationState.Default.ActiveProject.Connections.Insert(connectionIndex, connection);
            dataSourceTreeView.SelectedNode = dataSourceTreeView.Find(connection.ID);
        }

        private void DuplicateConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode connectionNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                DuplicateConnection(connectionNode);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DuplicateConnection(BindableTreeNode connectionNode)
        {
            IConnection connection = (IConnection)connectionNode.DataSource;
            IConnection connectionClone = connection.Clone();
            AddConnection(connectionClone, connectionNode.Index + 1);

            if (connectionNode.IsExpanded)
            {
                dataSourceTreeView.SelectedNode.Expand();
            }
        }

        private void TestConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IConnection connection = (IConnection)ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender).DataSource;
                TestConnectivityAsync(connection);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private async void TestConnectivityAsync(IConnection connection)
        {
            bool completedSuccessfully = false;

            try
            {
                ValidationResult validationResult = connection.Validate();

                if (validationResult.IsValid)
                {
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    Progress<ExecutionProgress> progress = new Progress<ExecutionProgress>();
                    ApplicationState.Default.NotifyAsyncProcessStarted(cancel, progress, 1);
                    ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.DataSourceFormConnectionTest, connection.Name)));

                    ConnectivityResult result = await Task.Run(() => ((IAvailableProvider)connection).IsAvailable(cancel.Token, progress));

                    if (cancel.IsCancellationRequested)
                    {
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, string.Format(Properties.Resources.DataSourceFormConnectionTestStopped, connection.Name)));
                    }
                    else if (result.IsAvailable)
                    {
                        completedSuccessfully = true;
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.DataSourceFormConnectionTestAvailable, connection.Name)));
                    }
                    else
                    {
                        completedSuccessfully = true;
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.DataSourceFormConnectionTestNotAvailable, connection.Name, result.ErrorMessage)));
                    }
                }
                else
                {
                    validationResult.Errors.ForEach(error => ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, error.Message)));
                }
            }
            catch (OperationCanceledException)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, string.Format(Properties.Resources.DataSourceFormConnectionTestStopped, connection.Name)));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.DataSourceFormConnectionTestNotAvailable, connection.Name, ex.Message), ex));
            }
            finally
            {
                ApplicationState.Default.NotifyAsyncProgressStopped(completedSuccessfully);
            }
        }

        private void ClearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode connectionNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                IConnection connection = ApplicationState.Default.ActiveProject.Connections[connectionNode.Index];
                ConnectionCache cache = new ConnectionCache(connection);
                cache.Clear();
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.DataSourceFormCacheClearSucceeded, connection.Name)));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void RemoveConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode connectionNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                RemoveConnection(connectionNode.Index, true);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void RemoveConnection(int connectionIndex, bool displayPrompt)
        {
            IConnection connection = ApplicationState.Default.ActiveProject.Connections[connectionIndex];
            DialogResult result = DialogResult.Yes;

            if (displayPrompt)
            {
                result = MessageBox.Show(string.Format(Properties.Resources.DataSourceFormConnectionRemovePrompt, connection.Name), Properties.Resources.DataSourceFormConnectionRemoveTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                ApplicationState.Default.ActiveProject.Connections.RemoveAt(connectionIndex);
                ApplicationState.Default.SelectedItem = default(object);
            }
        }

        private void OpenConnectionInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode connectionNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                IUrlAddressable connection = (IUrlAddressable)connectionNode.DataSource;
                System.Diagnostics.Process.Start(connection.Url.ToString());
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        #endregion

        #region Data Source Commands

        private void DataSourceContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                BindableTreeNode dataSourceNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                object dataSource = dataSourceNode.DataSource;

                viewDataSourceSampleDataToolStripMenuItem.Visible = dataSource is ISampleDataProvider;
                getRecordCountToolStripMenuItem.Visible = dataSource is IRecordCountProvider;
                openDataSourceInBrowserToolStripMenuItem.Visible = dataSource is IUrlAddressable;
                duplicateToolStripSeparator.Visible = dataSource is IRecordCountProvider || dataSource is IUrlAddressable;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode connectionNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                Type dataSourceType = (Type)((ToolStripMenuItem)sender).Tag;
                CreateAndAddDataSource(dataSourceType, connectionNode, ((IConnection)connectionNode.DataSource).DataSources.Count);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void CreateAndAddDataSource(Type dataSourceType, BindableTreeNode connectionNode, int dataSourceIndex)
        {
            IConnection connection = (IConnection)connectionNode.DataSource;
            IDataSource dataSource = (IDataSource)Activator.CreateInstance(dataSourceType, new object[] { connection });
            AddDataSource(dataSource, connectionNode, dataSourceIndex);
        }

        private void AddDataSource(IDataSource dataSource, BindableTreeNode connectionNode, int dataSourceIndex)
        {
            IConnection connection = (IConnection)connectionNode.DataSource;
            connection.DataSources.Insert(dataSourceIndex, dataSource);

            if (!connectionNode.IsExpanded)
            {
                connectionNode.Expand();
            }

            dataSourceTreeView.SelectedNode = dataSourceTreeView.Find(dataSource.ID);
        }

        private void DuplicateDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode dataSourceNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                DuplicateDataSource(dataSourceNode);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DuplicateDataSource(BindableTreeNode dataSourceNode)
        {
            IDataSource dataSource = (IDataSource)dataSourceNode.DataSource;
            IDataSource dataSourceClone = dataSource.Clone(true);
            AddDataSource(dataSourceClone, (BindableTreeNode)dataSourceNode.Parent, dataSourceNode.Index + 1);
            dataSourceTreeView.SelectedNode = dataSourceTreeView.Find(dataSourceClone.ID);
        }

        private void ViewDataSourceSampleDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode stepNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                IDataSource dataSource = (IDataSource)stepNode.DataSource;
                ViewSampleDataAsync(dataSource);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private async void ViewSampleDataAsync(IDataSource dataSource)
        {
            const int SAMPLE_DATA_RECORD_LIMIT = 1000;

            bool completedSuccessfully = false;

            try
            {
                ValidationResult validationResult = dataSource.Validate();

                if (validationResult.IsValid)
                {
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    Progress<ExecutionProgress> progress = new Progress<ExecutionProgress>();
                    ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format("Loading sample data from {0}", dataSource.Name)));
                    ApplicationState.Default.NotifyAsyncProcessStarted(cancel, progress, 1);

                    DataTable dataTable = await Task.Run(() => ((ISampleDataProvider)dataSource).GetSampleData(cancel.Token, progress, SAMPLE_DATA_RECORD_LIMIT));

                    if (!cancel.IsCancellationRequested)
                    {
                        completedSuccessfully = true;
                        SampleDataViewer viewer = new SampleDataViewer() { DataTable = dataTable };
                        viewer.Show();
                    }
                    else
                    {
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, string.Format("Stopped")));
                    }
                }
                else
                {
                    validationResult.Errors.ForEach(error => ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, error.Message)));
                }
            }
            catch (OperationCanceledException)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, string.Format("Stopped")));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
            finally
            {
                ApplicationState.Default.NotifyAsyncProgressStopped(completedSuccessfully);
            }
        }

        private void GetRecordCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode stepNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                IDataSource dataSource = (IDataSource)stepNode.DataSource;
                GetRecordCountAsync(dataSource);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private async void GetRecordCountAsync(IDataSource dataSource)
        {
            bool completedSuccessfully = false;

            try
            {
                ValidationResult validationResult = dataSource.Validate();

                if (validationResult.IsValid)
                {
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    Progress<ExecutionProgress> progress = new Progress<ExecutionProgress>();
                    ApplicationState.Default.NotifyAsyncProcessStarted(cancel, progress, 1);
                    ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format("Counting records in {0}", dataSource.Name)));

                    int recordCount = await Task.Run(() => ((IRecordCountProvider)dataSource).GetRecordCount(cancel.Token, progress));

                    if (!cancel.IsCancellationRequested)
                    {
                        completedSuccessfully = true;
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format("{0} contains {1:n0} record(s)", dataSource.Name, recordCount)));
                    }
                    else
                    {
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, "Record count stopped"));
                    }
                }
                else
                {
                    validationResult.Errors.ForEach(error => ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, error.Message)));
                }
            }
            catch (OperationCanceledException)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, "Record count stopped"));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
            finally
            {
                ApplicationState.Default.NotifyAsyncProgressStopped(completedSuccessfully);
            }
        }

        private void RemoveDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode dataSourceNode = (BindableTreeNode)ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                RemoveDataSource((BindableTreeNode)dataSourceNode.Parent, dataSourceNode.Index, true);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void RemoveDataSource(BindableTreeNode connectionNode, int dataSourceIndex, bool displayDialog)
        {
            IConnection connection = (IConnection)connectionNode.DataSource;
            IDataSource dataSource = connection.DataSources[dataSourceIndex];
            DialogResult result = DialogResult.Yes;

            if (displayDialog)
            {
                result = MessageBox.Show(string.Format("Are you sure you want to remove {0}?", dataSource.Name), "Remove Data Source Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                connection.DataSources.RemoveAt(dataSourceIndex);
                ApplicationState.Default.SelectedItem = default(object);
            }
        }

        private void OpenDataSourceInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode dataSourceNode = ClientUtility.GetContextMenuTreeviewNode(dataSourceTreeView, sender);
                IUrlAddressable dataSource = (IUrlAddressable)dataSourceNode.DataSource;
                System.Diagnostics.Process.Start(dataSource.Url.ToString());
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        #endregion

        private void DataSourceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ApplicationState.Default.SelectedItemPropertyChanged -= ApplicationState_SelectedItemPropertyChanged;
            ApplicationState.Default.ActiveProjectChanged -= ApplicationState_ActiveProjectChanged;
            ApplicationState.Default.AsyncProcessStarted -= ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.AsyncProcessStopped -= ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.SelectedItemChanged -= ApplicationState_SelectedItemChanged;
        }
    }
}
