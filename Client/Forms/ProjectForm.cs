using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottLane.DataTidy.Core;
using ScottLane.DataTidy.Client.Controls;
using ScottLane.DataTidy.Client.Model;

namespace ScottLane.DataTidy.Client.Forms
{
    /// <summary>
    /// The main window for a project that displays batches and operations that can be executed.
    /// </summary>
    public partial class ProjectForm : ChildForm
    {
        private Project project;
        public string Path { get; private set; }

        public ProjectForm()
        {
            InitializeComponent();
            batchImageList.Images.Add(typeof(Batch).FullName, Properties.Resources.ProjectFormBatch);
            saveProjectDialog.Filter = string.Format(Properties.Resources.ProjectFileFilter, Core.Properties.Resources.ProjectFileExtension);
            ApplicationState.Default.AsyncProcessStarted += ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.AsyncProcessStopped += ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.SelectedItemPropertyChanged += ApplicationState_SelectedItemPropertyChanged;

            project = new Project();
            BindControlsToProject();
            // todo - if 'Project' name is already taken by a project window, choose Project (2), etc
        }

        private void BindControlsToProject()
        {
            ApplicationState.Default.ActiveProject = project;
            batchTreeView.DataMember = nameof(Project.Batches);
            batchTreeView.DataSource = project;
            batchTreeView.SetExpandedState(project.OperationState);
            batchTreeViewContextMenuStrip.Enabled = true;
            project.PropertyChanged += Project_PropertyChanged;
            RefreshFormName();
        }

        private void RefreshFormName()
        {
            Text = string.Format("{0}{1}", project.Name, project.IsModified ? "*" : string.Empty);
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshFormName();
        }

        private void ApplicationState_SelectedItemPropertyChanged(object sender, ObjectEventArgs e)
        {
            // todo - remove after treeview inotifypropertychanged event bug fixed and internal project comms to notify changes is complete
            project.IsModified = true;
            batchTreeView.Refresh();
        }

        private void ApplicationState_AsyncProcessStarted(object sender, AsyncEventArgs e)
        {
            batchTreeView.Enabled = false;
        }

        private void ApplicationState_AsyncProcessStopped(object sender, EventArgs e)
        {
            batchTreeView.Enabled = true;
        }

        #region Project Events

        public void LoadProject(string fileName)
        {
            project = Project.Load(fileName);
            BindControlsToProject();
            Path = fileName;
            saveProjectDialog.FileName = fileName;
            ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, Properties.Resources.ProjectOpenSucceeded));
        }

        public void LoadProject(Project project)
        {
            this.project = project;
            BindControlsToProject();
        }

        public void SaveProject()
        {
            SaveProject(false);
        }

        public void SaveProjectAs()
        {
            SaveProject(true);
        }

        private void SaveProject(bool forceDialog)
        {
            bool doSave = true;

            if (string.IsNullOrEmpty(Path) || forceDialog)
            {
                if (saveProjectDialog.ShowDialog() == DialogResult.OK)
                {
                    Path = saveProjectDialog.FileName;
                }
                else
                {
                    doSave = false;
                }
            }

            if (doSave)
            {
                project.Save(Path);

                //ApplicationState.Current.ActiveProject.IsModifie
                //OnProjectSaved(new ProjectEventArgs(project));
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.MainFormProjectSaveSucceeded)));
            }

        }

        #endregion

        #region TreeView Events

        private void BatchTreeView_DataBinding(object sender, Controls.NodeEventArgs e)
        {
            try
            {
                if (e.Node.DataSource is Batch batch)
                {
                    e.Node.Enabled = true;
                    e.Node.DataMember = nameof(batch.Operations);
                    e.Node.DisplayMember = nameof(batch.Name);
                    e.Node.ContextMenuStrip = batchContextMenuStrip;
                    e.Node.ImageKey = batch.GetType().FullName;
                    e.Node.SelectedImageKey = e.Node.ImageKey;
                    e.Node.ID = batch.ID;
                }
                else if (e.Node.DataSource is IOperation operation)
                {
                    e.Node.Enabled = operation.Enabled;
                    e.Node.DisplayMember = nameof(operation.Name);
                    e.Node.ContextMenuStrip = operationContextMenuStrip;
                    e.Node.ImageKey = operation.GetType().FullName;
                    e.Node.SelectedImageKey = e.Node.ImageKey;
                    e.Node.ID = operation.ID;
                    AddOperationImage(operation.GetType());
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddOperationImage(Type type)
        {
            OperationAttribute attribute = (OperationAttribute)Attribute.GetCustomAttribute(type, typeof(OperationAttribute));

            if (attribute?.IconResourceName != null && !batchImageList.Images.ContainsKey(type.FullName))
            {
                batchImageList.Images.Add(type.FullName, attribute.GetIcon(type.Assembly));
            }
        }

        private void BatchTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    batchTreeView.SelectedNode = e.Node;
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void BatchTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (batchTreeView.SelectedNode != default(BindableTreeNode))
                {
                    ApplicationState.Default.SelectedItem = ((BindableTreeNode)batchTreeView.SelectedNode).DataSource;
                    //OnItemSelected(new ItemEventArgs(((BindableTreeNode)batchTreeView.SelectedNode).DataSource));
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void BatchTreeView_Enter(object sender, EventArgs e)
        {
            try
            {
                if (batchTreeView.SelectedNode != default(BindableTreeNode))
                {
                    ApplicationState.Default.SelectedItem = ((BindableTreeNode)batchTreeView.SelectedNode).DataSource;
                    //OnItemSelected(new ItemEventArgs(((BindableTreeNode)batchTreeView.SelectedNode).DataSource));
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void BatchTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (batchTreeView.SelectedNode != default(TreeNode))
                {
                    object selectedNodeDataSource = ((BindableTreeNode)batchTreeView.SelectedNode).DataSource;

                    if (e.KeyCode == Keys.Delete && selectedNodeDataSource is Batch)
                    {
                        RemoveBatch(batchTreeView.SelectedNode.Index, true);
                    }
                    else if (e.KeyCode == Keys.Delete && selectedNodeDataSource is IOperation)
                    {
                        RemoveOperation((BindableTreeNode)batchTreeView.SelectedNode.Parent, batchTreeView.SelectedNode.Index, true);
                    }
                    else if (e.KeyCode == Keys.F2)
                    {
                        batchTreeView.SelectedNode.BeginEdit();
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D && selectedNodeDataSource is Batch)
                    {
                        DuplicateBatch((BindableTreeNode)batchTreeView.SelectedNode);
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D && selectedNodeDataSource is IOperation)
                    {
                        DuplicateOperation((BindableTreeNode)batchTreeView.SelectedNode);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void BatchTreeView_ItemDrag(object sender, ItemDragEventArgs e)
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

        private void BatchTreeView_DragEnter(object sender, DragEventArgs e)
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

        private void BatchTreeView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    BindableTreeNode draggedNode = (BindableTreeNode)e.Data.GetData(typeof(BindableTreeNode));
                    BindableTreeNode droppedNode = (BindableTreeNode)batchTreeView.HitTest(batchTreeView.PointToClient(new Point(e.X, e.Y)))?.Node;

                    if (draggedNode.DataSource is Batch && droppedNode.DataSource is Batch && draggedNode.ID != droppedNode.ID)
                    {
                        // rearrange batches
                        bool expanded = droppedNode.IsExpanded;
                        int droppedIndex = droppedNode.Index;
                        RemoveBatch(draggedNode.Index, false);
                        AddBatch((Batch)draggedNode.DataSource, droppedIndex);

                        if (expanded)
                        {
                            batchTreeView.SelectedNode.Expand();   // todo - this flashes a bit
                        }
                    }
                    else if (draggedNode.DataSource is IOperation && droppedNode.DataSource is IOperation && draggedNode.ID != droppedNode.ID)
                    {
                        // rearrange operations
                        int droppedIndex = droppedNode.Index;
                        RemoveOperation((BindableTreeNode)draggedNode.Parent, draggedNode.Index, false);
                        AddOperation((IOperation)draggedNode.DataSource, (BindableTreeNode)droppedNode.Parent, droppedIndex);
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
        private void BatchTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            try
            {
                project.OperationState = batchTreeView.GetExpandedState();
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        /// <summary>
        /// Stores the expanded state so the application is in the same state next time the project is opened.
        /// </summary>
        private void BatchTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                project.OperationState = batchTreeView.GetExpandedState();
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void BatchTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //if (e.Label != null)
            //{
            //    if (e.Node.Tag is Batch)
            //    {
            //        ((Batch)e.Node.Tag).Name = e.Label; // todo - this is wrong
            //        OnItemChanged(new ItemEventArgs(e.Node.Tag));
            //    }
            //    else if (e.Node.Tag is Operation)
            //    {
            //        ((IOperation)e.Node.Tag).Name = e.Label;
            //        OnItemChanged(new ItemEventArgs(e.Node.Tag));
            //    }
            //}
        }

        #endregion

        #region Batch Commands

        private void BatchContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                addOperationToolStripMenuItem.DropDownItems.Clear();
                BindableTreeNode batchNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                Type batchType = batchNode.DataSource.GetType();
                List<Type> operationTypes = CoreUtility.GetInterfaceImplementorsWithAttribute(typeof(IOperation), typeof(OperationAttribute));

                foreach (Type operationType in operationTypes)
                {
                    OperationAttribute operationAttribute = (OperationAttribute)Attribute.GetCustomAttribute(operationType, typeof(OperationAttribute));
                    ToolStripMenuItem addOperationToolStripMenuItem = new ToolStripMenuItem()
                    {
                        ImageScaling = ToolStripItemImageScaling.None,
                        Tag = operationType
                    };
                    addOperationToolStripMenuItem.Click += AddOperationToolStripMenuItem_Click;
                    addOperationToolStripMenuItem.Text = operationAttribute.DisplayName;
                    addOperationToolStripMenuItem.ToolTipText = operationAttribute.Description;
                    addOperationToolStripMenuItem.Image = operationAttribute.GetIcon(operationType.Assembly);
                    this.addOperationToolStripMenuItem.DropDownItems.Add(addOperationToolStripMenuItem);
                }

                // sort by name
                ArrayList menuItems = new ArrayList(this.addOperationToolStripMenuItem.DropDownItems);
                menuItems.Sort(new ToolStripItemComparer());
                addOperationToolStripMenuItem.DropDownItems.Clear();

                foreach (ToolStripItem menuItem in menuItems)
                {
                    addOperationToolStripMenuItem.DropDownItems.Add(menuItem);
                }

                addOperationToolStripMenuItem.Enabled = addOperationToolStripMenuItem.DropDownItems.Count > 0;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Batch batch = new Batch(project);
                AddBatch(batch, project.Batches.Count);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void AddBatch(Batch batch, int batchIndex)
        {
            project.Batches.Insert(batchIndex, batch);
            batchTreeView.SelectedNode = batchTreeView.Find(batch.ID);
            //OnItemChanged(new ItemEventArgs(batch));
        }

        private void DuplicateBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode batchNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                DuplicateBatch(batchNode);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DuplicateBatch(BindableTreeNode batchNode)
        {
            Batch batch = (Batch)batchNode.DataSource;
            Batch batchClone = batch.Clone();
            AddBatch(batchClone, batchNode.Index + 1);

            if (batchNode.IsExpanded)
            {
                batchTreeView.SelectedNode.Expand();
            }
        }

        private void RemoveBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode batchNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                RemoveBatch(batchNode.Index, true);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void RemoveBatch(int batchIndex, bool displayPrompt)
        {
            Batch batch = project.Batches[batchIndex];
            DialogResult result = DialogResult.Yes;

            if (displayPrompt)
            {
                result = MessageBox.Show(string.Format(Properties.Resources.MainFormBatchRemovePrompt, batch.Name), Properties.Resources.MainFormBatchRemoveTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                project.Batches.RemoveAt(batchIndex);
                //OnItemChanged(new ItemEventArgs(project));
                ApplicationState.Default.SelectedItem = default(object);
                //OnItemSelected(new ItemEventArgs(null));    // todo - another event that can be hooked for this?
            }
        }

        private void ExecuteBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Batch batch = (Batch)ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender).DataSource;
            ApplicationState.Default.RequestExecution(batch);
        }

        #endregion

        #region Operation Commands

        private void AddOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode batchNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                Type operationType = (Type)((ToolStripMenuItem)sender).Tag;
                CreateAndAddOperation(operationType, batchNode, ((Batch)batchNode.DataSource).Operations.Count);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void CreateAndAddOperation(Type operationType, BindableTreeNode batchNode, int operationIndex)
        {
            Batch batch = (Batch)batchNode.DataSource;
            IOperation operation = (IOperation)Activator.CreateInstance(operationType, new object[] { batch });
            AddOperation(operation, batchNode, operationIndex);
        }

        private void AddOperation(IOperation operation, BindableTreeNode batchNode, int operationIndex)
        {
            Batch batch = (Batch)batchNode.DataSource;
            batch.Operations.Insert(operationIndex, operation);

            if (!batchNode.IsExpanded)
            {
                batchNode.Expand();
            }

            batchTreeView.SelectedNode = batchTreeView.Find(operation.ID);
            //OnItemChanged(new ItemEventArgs(batch));
        }

        private void EnableOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode operationNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                IOperation operation = (IOperation)operationNode.DataSource;
                operation.Enabled = true;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DisableOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode operationNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                IOperation operation = (IOperation)operationNode.DataSource;
                operation.Enabled = false;
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DuplicateOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode operationNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                DuplicateOperation(operationNode);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void DuplicateOperation(BindableTreeNode operationNode)
        {
            IOperation operation = (IOperation)operationNode.DataSource;
            IOperation operationClone = operation.Clone(true);
            AddOperation(operationClone, (BindableTreeNode)operationNode.Parent, operationNode.Index + 1);
            batchTreeView.SelectedNode = batchTreeView.Find(operationClone.ID);
        }

        private void RemoveOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BindableTreeNode operationNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                RemoveOperation((BindableTreeNode)operationNode.Parent, operationNode.Index, true);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void RemoveOperation(BindableTreeNode batchNode, int operationIndex, bool displayDialog)
        {
            Batch batch = (Batch)batchNode.DataSource;
            IOperation operation = batch.Operations[operationIndex];
            DialogResult result = DialogResult.Yes;

            if (displayDialog)
            {
                result = MessageBox.Show(string.Format(Properties.Resources.MainFormOperationRemovePrompt, operation.Name), Properties.Resources.MainFormOperationRemoveTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                batch.Operations.RemoveAt(operationIndex);
                //OnItemChanged(new ItemEventArgs(batch));
                ApplicationState.Default.SelectedItem = null;
            }
        }

        private void ExecuteOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IExecutable operation = (IExecutable)ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender).DataSource;
            ApplicationState.Default.RequestExecution(operation);
        }

        private void OperationContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                BindableTreeNode operationNode = ClientUtility.GetContextMenuTreeviewNode(batchTreeView, sender);
                executeOperationToolStripMenuItem.Enabled = operationNode.DataSource is IExecutable;

                if (operationNode.DataSource is IOperation operation)
                {
                    executeOperationToolStripMenuItem.Enabled = operationNode.DataSource is IExecutable && operation.Enabled;
                    enableOperationToolStripMenuItem.Visible = !operation.Enabled;
                    disableOperationToolStripMenuItem.Visible = operation.Enabled;
                }

                int menuItemCount = operationContextMenuStrip.Items.IndexOf(customEndToolStripSeparator) - operationContextMenuStrip.Items.IndexOf(customStartToolStripSeparator) - 1;

                // hide any custom menu items
                for (int menuItemsRemoved = 0; menuItemsRemoved < menuItemCount; menuItemsRemoved++)
                {
                    operationContextMenuStrip.Items.RemoveAt(operationContextMenuStrip.Items.IndexOf(customStartToolStripSeparator) + 1);
                }

                if (operationNode.DataSource is ICustomMenuItemProvider)
                {
                    customEndToolStripSeparator.Visible = true;
                    List<CustomMenuItem> menuItems = ((ICustomMenuItemProvider)operationNode.DataSource).GetCustomMenuItems();

                    foreach (CustomMenuItem menuItem in menuItems)
                    {
                        ToolStripMenuItem customToolStripMenuItem = new ToolStripMenuItem()
                        {
                            Text = menuItem.Text,
                            ToolTipText = menuItem.ToolTip,
                            Image = menuItem.Icon,
                            ImageScaling = ToolStripItemImageScaling.None,
                            Size = new Size(182, 38),
                            Tag = menuItem,
                            Enabled = menuItem.Enabled
                        };

                        customToolStripMenuItem.Click += CustomToolStripMenuItem_Click;
                        operationContextMenuStrip.Items.Insert(operationContextMenuStrip.Items.IndexOf(customEndToolStripSeparator), customToolStripMenuItem);
                    }
                }
                else
                {
                    customEndToolStripSeparator.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private void CustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CustomMenuItem menuItem = (CustomMenuItem)(((ToolStripMenuItem)sender).Tag);

                if (menuItem.SynchronousEventHandler != null)
                {
                    menuItem.SynchronousEventHandler?.Invoke();
                }
                else
                {
                    ExecuteMenuItemAsynchronously(menuItem);
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }
        }

        private async void ExecuteMenuItemAsynchronously(CustomMenuItem menuItem)
        {
            bool completedSuccessfully = false;

            try
            {
                CancellationTokenSource cancel = new CancellationTokenSource();
                Progress<ExecutionProgress> progress = new Progress<ExecutionProgress>();
                ApplicationState.Default.NotifyAsyncProcessStarted(cancel, progress, 1);

                await Task.Run(() => menuItem.AsynchronousEventHandler.Invoke(cancel.Token, progress));

                if (!cancel.IsCancellationRequested)
                {
                    completedSuccessfully = true;
                }
                else
                {
                    ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.MainFormExecuteStopped, menuItem.Text)));
                }
            }
            catch (OperationCanceledException)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.MainFormExecuteStopped, menuItem.Text)));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
            }

            ApplicationState.Default.NotifyAsyncProgressStopped(completedSuccessfully);
        }

        #endregion

        #region Form Events

        private void ProjectForm_Activated(object sender, EventArgs e)
        {
            ApplicationState.Default.ActiveProject = project;
        }

        private void ProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (project.IsModified && !e.Cancel)
            {
                DialogResult result = MessageBox.Show("The project has not been saved. Do you want to save before closing?", "Save Project", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveProject();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ProjectForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ApplicationState.Default.SelectedItemPropertyChanged -= ApplicationState_SelectedItemPropertyChanged;
            ApplicationState.Default.AsyncProcessStopped -= ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.AsyncProcessStarted -= ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.ActiveProject = default(Project);
        }

        #endregion
    }
}