using ScottLane.DataTidy.Client.Model;
using ScottLane.DataTidy.Core;
using ScottLane.DataTidy.Dynamics365;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ScottLane.DataTidy.Client.Forms
{
    public partial class MainForm : Form
    {
        private ProjectForm activeProjectForm;
        private List<ChildForm> childForms;

        public MainForm()
        {
            InitializeComponent();

            openProjectDialog.Filter = string.Format(Properties.Resources.ProjectFileFilter, Core.Properties.Resources.ProjectFileExtension);
            LoadPluginAssemblies();
            CreateChildForms();
            EnableControls();

            ApplicationState.Default.ExecutionRequested += ApplicationState_ExecutionRequested;
            ApplicationState.Default.AsyncProcessStarted += ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.AsyncProcessStopped += ApplicationState_AsyncProcessStopped;
        }

        public MainForm(bool loadSampleProject) : this()
        {
            Project project = CreateProjectTemplate();
            OpenProject(project);
        }

        public Project CreateProjectTemplate()
        {
            Project project = new Project();
            project.Connections.Add(new Dynamics365Connection(project) { Name = "Dynamics 365" });
            project.Batches.Add(new Batch(project));

            return project;
        }

        public MainForm(string path) : this()
        {
            OpenProject(path);
        }

        private void ApplicationState_ExecutionRequested(object sender, IExecuteEventArgs e)
        {
            ExecuteItemAsync(e.Item);
        }

        private void ApplicationState_AsyncProcessStarted(object sender, AsyncEventArgs e)
        {
            EnableControls();
        }

        private void ApplicationState_AsyncProcessStopped(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void EnableControls()
        {
            saveProjectToolStripMenuItem.Enabled = !ApplicationState.Default.AsyncProcessRunning && activeProjectForm != default(ProjectForm);
            saveProjectAsToolStripMenuItem.Enabled = !ApplicationState.Default.AsyncProcessRunning && activeProjectForm != default(ProjectForm);
            newProjectToolStripMenuItem.Enabled = !ApplicationState.Default.AsyncProcessRunning;
            openProjectToolStripMenuItem.Enabled = !ApplicationState.Default.AsyncProcessRunning;
            closeProjectToolStripMenuItem.Enabled = activeProjectForm != default(ProjectForm);
            executeToolStripButton.Enabled = !ApplicationState.Default.AsyncProcessRunning && activeProjectForm != default(ProjectForm);
            stopToolStripButton.Enabled = ApplicationState.Default.AsyncProcessRunning;
            recentProjectsToolStripMenuItem.Enabled = !ApplicationState.Default.AsyncProcessRunning;
        }

        private static void LoadPluginAssemblies()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // todo - this should not be required - need to use plugin directory only and ILMerge all DLL's into the plugins
            if (Directory.Exists(assemblyPath))
            {
                List<string> assemblies = Directory.GetFiles(assemblyPath, "*.dll").ToList();

                foreach (string assembly in assemblies)
                {
                    if (!assembly.Contains("ScottLane.DataTidy.Core.dll"))
                    {
                        Assembly.LoadFrom(assembly);
                    }
                }
            }

            string pluginPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins");

            if (Directory.Exists(pluginPath))
            {
                List<string> pluginAssemblies = Directory.GetFiles(pluginPath, "*.dll").ToList();

                foreach (string pluginAssembly in pluginAssemblies)
                {
                    Assembly.LoadFrom(pluginAssembly);
                }
            }
        }

        #region Child Form Management

        /// <summary>
        /// Creates all child forms and brings the notifications form to the front.
        /// </summary>
        private void CreateChildForms()
        {
            childForms = new List<ChildForm>();
            CreateChildForm(typeof(NotificationsForm), DockState.DockBottom);
            CreateChildForm(typeof(PropertiesForm), DockState.DockRight);
            CreateChildForm(typeof(DataSourceForm), DockState.DockLeft);
            CreateChildForm(typeof(ProgressForm), DockState.DockBottom);
            CreateChildForm(typeof(PerformanceForm), DockState.DockBottom);
            CreateChildForm(typeof(NotificationsForm), DockState.DockBottom);
        }

        /// <summary>
        /// Creates a new child form of the specified type in the specified dock state and sets focus to the form.
        /// If the form already exists, sets focus to the existing child form.
        /// If the specified type is ProjectForm a new form will always be created.
        /// </summary>
        /// <param name="type">The child form type.</param>
        /// <param name="dockState">The form dock state.</param>
        private ChildForm CreateChildForm(Type type, DockState dockState)
        {
            ChildForm childForm = childForms.Find(c => c.GetType() == type);

            if (childForm != default(ChildForm) && type != typeof(ProjectForm))
            {
                childForm.Show();
            }
            else
            {
                childForm = (ChildForm)Activator.CreateInstance(type);
                childForm.Activated += ChildForm_Activated;
                childForm.FormClosed += ChildForm_FormClosed;
                childForm.Show(mainDockPanel, dockState);
                childForms.Add(childForm);
            }

            return childForm;
        }

        private void ChildForm_Activated(object sender, EventArgs e)
        {
            if (sender is ProjectForm)
            {
                activeProjectForm = (ProjectForm)sender;
            }

            EnableControls();
        }

        private void ChildForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is ProjectForm)
            {
                activeProjectForm = default(ProjectForm);
            }

            childForms.RemoveAll(childForm => ReferenceEquals(childForm, sender));
            EnableControls();
        }

        #endregion

        #region Menu Events

        private void ProjectToolStripButton_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.RecentProjects != default(StringCollection))
                {
                    recentProjectsToolStripMenuItem.Enabled = true;
                    recentProjectsToolStripMenuItem.DropDownItems.Clear();

                    foreach (string path in Properties.Settings.Default.RecentProjects)
                    {
                        if (!string.IsNullOrEmpty(path))
                        {
                            ToolStripButton recentProjectToolStripButton = new ToolStripButton()
                            {
                                Text = ClientUtility.ShortenPath(path, 40),
                                Tag = path
                            };
                            recentProjectToolStripButton.Click += RecentProjectToolStripButton_Click;
                            recentProjectsToolStripMenuItem.DropDownItems.Add(recentProjectToolStripButton);
                        }
                    }
                }
                else
                {
                    recentProjectsToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format("An error occurred reading recent projects: {0}", ex.Message), ex));
            }
        }

        private void AddRecentlyOpenedProject(string path)
        {
            if (Properties.Settings.Default.RecentProjects == default(StringCollection))
            {
                Properties.Settings.Default.RecentProjects = new StringCollection();
            }

            if (Properties.Settings.Default.RecentProjects.Contains(path))
            {
                Properties.Settings.Default.RecentProjects.Remove(path);
            }

            Properties.Settings.Default.RecentProjects.Insert(0, path);
            Properties.Settings.Default.Save();
        }

        private void RecentProjectToolStripButton_Click(object sender, EventArgs e)
        {
            string path = ((ToolStripButton)sender).Tag.ToString();

            if (!System.IO.File.Exists(path))
            {
                if (MessageBox.Show(Properties.Resources.MainFormRecentProjectRemovePrompt, Properties.Resources.MainFormRecentProjectRemoveTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Properties.Settings.Default.RecentProjects.Remove(path);
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                OpenProject(path);
            }
        }

        private void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project project = CreateProjectTemplate();
            OpenProject(project);
        }

        private void OpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openProjectDialog.ShowDialog() == DialogResult.OK)
            {
                OpenProject(openProjectDialog.FileName);
            }
        }

        private void CloseProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                activeProjectForm.Close();
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.ProjectCloseFailed, ex.Message), ex));
            }
        }

        private void OpenProject(Project project)
        {
            try
            {
                ProjectForm projectForm = (ProjectForm)CreateChildForm(typeof(ProjectForm), DockState.Document);
                try
                {
                    projectForm.LoadProject(project);
                }
                catch
                {
                    projectForm.Close();
                    throw;
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.ProjectOpenFailed, ex.Message), ex));
            }
        }

        private void OpenProject(string path)
        {
            try
            {
                bool projectAlreadyOpen = childForms.Exists(childForm => childForm is ProjectForm && ((ProjectForm)childForm).Path == path);

                if (projectAlreadyOpen)
                {
                    MessageBox.Show(Properties.Resources.ProjectOpenAlreadyText, Properties.Resources.ProjectOpenAlreadyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ProjectForm projectForm = (ProjectForm)CreateChildForm(typeof(ProjectForm), DockState.Document);
                    try
                    {
                        projectForm.LoadProject(path);
                        AddRecentlyOpenedProject(path);
                    }
                    catch
                    {
                        projectForm.Close();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.ProjectOpenFailed, ex.Message), ex));
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                activeProjectForm.SaveProject();
                AddRecentlyOpenedProject(activeProjectForm.Path);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.MainFormProjectSaveFailed, ex.Message), ex));
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                activeProjectForm.SaveProjectAs();
                AddRecentlyOpenedProject(activeProjectForm.Path);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.MainFormProjectSaveFailed, ex.Message), ex));
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExecuteToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteItemAsync(ApplicationState.Default.ActiveProject);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, string.Format(Properties.Resources.MainFormExecuteFailed, ex.Message), ex));
            }
        }

        private async void ExecuteItemAsync(IExecutable item)
        {
            if (MessageBox.Show(string.Format(Properties.Resources.MainFormExecutePrompt, item.Name), Properties.Resources.MainFormExecuteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DateTime startedOn = DateTime.Now;

                try
                {
                    ValidationResult validationResult = item.Validate();

                    if (validationResult.IsValid)
                    {
                        CancellationTokenSource cancel = new CancellationTokenSource();
                        Progress<ExecutionProgress> progress = new Progress<ExecutionProgress>();
                        ApplicationState.Default.NotifyAsyncProcessStarted(cancel, progress, item.ItemCount);
                        await Task.Run(() => item.Execute(cancel.Token, progress));
                        ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.MainFormExecuteSucceeded, item.Name, ClientUtility.GetDurationString(DateTime.Now - startedOn))));
                        ApplicationState.Default.NotifyAsyncProgressStopped(true);
                    }
                    else
                    {
                        validationResult.Errors.ForEach(error => ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, error.Message)));
                    }
                }
                catch (OperationCanceledException)
                {
                    ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Warning, string.Format(Properties.Resources.MainFormExecuteStopped, item.Name, ClientUtility.GetDurationString(DateTime.Now - startedOn))));
                    ApplicationState.Default.NotifyAsyncProgressStopped(false);
                }
                catch (Exception ex)
                {
                    ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Error, ex.Message, ex));
                    ApplicationState.Default.NotifyAsyncProgressStopped(false);
                }
            }
        }

        private void StopToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationState.Default.Cancel.Cancel();
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, Properties.Resources.MainFormExecuteStopping));
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.MainFormExecuteStopFailed, ex.Message), ex));
            }
        }

        private void ViewNotificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CreateChildForm(typeof(NotificationsForm), DockState.DockBottom);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.WindowOpenFailed, ex.Message), ex));
            }
        }

        private void ViewPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CreateChildForm(typeof(PropertiesForm), DockState.DockRight);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.WindowOpenFailed, ex.Message), ex));
            }
        }

        private void ViewProgressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CreateChildForm(typeof(ProgressForm), DockState.DockBottom);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.WindowOpenFailed, ex.Message), ex));
            }
        }

        private void ViewPerformanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CreateChildForm(typeof(PerformanceForm), DockState.DockBottom);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.WindowOpenFailed, ex.Message), ex));
            }
        }

        private void DataSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CreateChildForm(typeof(DataSourceForm), DockState.DockLeft);
            }
            catch (Exception ex)
            {
                ApplicationState.Default.RaiseNotification(new NotificationEventArgs(NotificationType.Information, string.Format(Properties.Resources.WindowOpenFailed, ex.Message), ex));
            }
        }

        private void RemoveRecentProjectToolStripMenuItem_Click(object sender, EventArgs e)
        { }

        #endregion

        #region Form Events

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            OpenProject(filePath);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = e.Cancel || (ApplicationState.Default.AsyncProcessRunning && MessageBox.Show(Properties.Resources.MainFormCloseAsyncRunningPrompt, Properties.Resources.MainFormCloseAsyncRunningTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No);
            }
            catch { }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ApplicationState.Default.AsyncProcessStopped -= ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.AsyncProcessStarted -= ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.ExecutionRequested -= ApplicationState_ExecutionRequested;
        }

        #endregion

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void DocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/scottelane/data-tidy/wiki");
        }
    }
}
