using ScottLane.DataTidy.Core;
using System;
using System.Threading;

namespace ScottLane.DataTidy.Client.Model
{
    /// <summary>
    /// Manages application state and triggers methods that operate across forms.
    /// </summary>
    public sealed class ApplicationState
    {
        /// <summary>
        /// Initialises a static instance of the ApplicationState class.
        /// </summary>
        static ApplicationState()
        { }

        /// <summary>
        /// Initialises a new instance of the ApplicationState class.
        /// </summary>
        private ApplicationState()
        { }

        /// <summary>
        /// Gets the default ApplicationState instance.
        /// </summary>
        public static ApplicationState Default { get; } = new ApplicationState();

        /// <summary>
        /// Event that is raised when the active Project changes.
        /// </summary>
        public event ProjectEventHandler ActiveProjectChanged;

        private Project activeProject;

        /// <summary>
        /// Gets the active project.
        /// </summary>
        public Project ActiveProject
        {
            get { return activeProject; }
            set
            {
                if (activeProject != value)
                {
                    activeProject = value;
                    OnActiveProjectChanged(new ProjectEventArgs(activeProject));
                }
            }
        }

        /// <summary>
        /// Raises the ActiveProjectChanged event.
        /// </summary>
        private void OnActiveProjectChanged(ProjectEventArgs e)
        {
            ActiveProjectChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Event that is raised when the selected item changes.
        /// </summary>
        public event ObjectEventHandler SelectedItemChanged;

        public event ObjectEventHandler SelectedItemPropertyChanged;

        private object selectedItem;

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnSelectedItemChanged(new ObjectEventArgs(selectedItem));
                }
            }
        }

        /// <summary>
        /// Raises the SelectedItemChanged event.
        /// </summary>
        private void OnSelectedItemChanged(ObjectEventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }

        /// <summary>
        /// todo - remove after treeview inotifypropertychanged event bug fixed
        /// </summary>
        /// <param name="e"></param>
        public void OnSelectedItemPropertyChanged(ObjectEventArgs e)
        {
            SelectedItemPropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Gets or sets a value indicating whether an asynchronous process is running.
        /// </summary>
        public bool AsyncProcessRunning { get; private set; }

        public CancellationTokenSource Cancel { get; set; }

        /// <summary>
        /// Event that is raised when an asynchronous process execution is requested.
        /// </summary>
        public event IExecuteEventHandler ExecutionRequested;

        public void NotifyAsyncProcessStarted(CancellationTokenSource cancel, Progress<ExecutionProgress> progress, int operationCount)
        {
            Cancel = cancel;
            AsyncProcessRunning = true;
            progress.ProgressChanged += Progress_ProgressChanged;
            OnAsyncProcessStarted(new AsyncEventArgs(operationCount));
        }

        public void NotifyAsyncProgressStopped(bool completedSuccessfully)
        {
            Cancel = default(CancellationTokenSource);
            AsyncProcessRunning = false;
            OnAsyncProcessStopped(new AsyncStoppedEventArgs(completedSuccessfully));
        }

        /// <summary>
        /// Calls a method that raises the AsyncProgressChanged event when the async progress changes, and raises a notification if the progress was reporting a notification.
        /// </summary>
        private void Progress_ProgressChanged(object sender, ExecutionProgress e)
        {
            OnAsyncProgressChanged(new ProgressEventArgs(e));

            if (e.ProgressType == ProgressType.Notification)
            {
                RaiseNotification(new NotificationEventArgs(e.NotificationType, e.Message));
            }
        }

        /// <summary>
        /// Event that is raised when the asynchronous processing progress changes.
        /// </summary>
        public event ProgressEventHandler AsyncProgressChanged;

        /// <summary>
        /// Raises the AsyncProgressChanged event.
        /// </summary>
        /// <param name="e"></param>
        private void OnAsyncProgressChanged(ProgressEventArgs e)
        {
            AsyncProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Event that is fired after an asyncronous process has started.
        /// </summary>
        public event AsyncEventHandler AsyncProcessStarted;

        /// <summary>
        /// Event that is fired after an asynchronous process has stopped.
        /// </summary>
        public event AsyncStoppedEventHandler AsyncProcessStopped;

        /// <summary>
        /// Requests the execution of an asynchronous process.
        /// </summary>
        /// <param name="item"></param>
        public void RequestExecution(IExecutable item)
        {
            OnExecutionRequested(new IExecuteEventArgs(item));
        }

        /// <summary>
        /// Event that is raised when the execution of an asynchronous process is requested.
        /// </summary>
        private void OnExecutionRequested(IExecuteEventArgs e)
        {
            ExecutionRequested?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the AsyncProcessStarted event.
        /// </summary>
        private void OnAsyncProcessStarted(AsyncEventArgs e)
        {
            AsyncProcessStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the AsyncProcessStopped event.
        /// </summary>
        private void OnAsyncProcessStopped(AsyncStoppedEventArgs e)
        {
            AsyncProcessStopped(this, e);
        }

        /// <summary>
        /// Event that is raised when an error is selected.
        /// </summary>
        public event NotificationEventHandler SelectedErrorChanged;

        private NotificationEventArgs selectedError;

        /// <summary>
        /// Gets or sets the selected error.
        /// </summary>
        public NotificationEventArgs SelectedError
        {
            get { return selectedError; }
            set
            {
                if (selectedError != value)
                {
                    selectedError = value;
                    OnSelectedErrorChanged(selectedError);
                }
            }
        }

        /// <summary>
        /// Raises the SelectedErrorChanged event.
        /// </summary>
        private void OnSelectedErrorChanged(NotificationEventArgs e)
        {
            SelectedErrorChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Event that is raised when a notification is raised.
        /// </summary>
        public event NotificationEventHandler NotificationRaised;

        public void RaiseNotification(NotificationEventArgs e)  // todo - encompass details in a class with error type
        {
            OnNotificationRaised(e);
        }

        private void OnNotificationRaised(NotificationEventArgs e)
        {
            NotificationRaised?.Invoke(this, e);
        }
    }

    public class IExecuteEventArgs : EventArgs
    {
        public IExecutable Item { get; set; }

        public IExecuteEventArgs(IExecutable item)
        {
            Item = item;
        }
    }

    public class AsyncEventArgs : EventArgs
    {
        public int OperationCount { get; set; }

        public AsyncEventArgs(int operationCount)
        {
            OperationCount = operationCount;
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public ExecutionProgress Progress { get; set; }

        public ProgressEventArgs(ExecutionProgress progress)
        {
            Progress = progress;
        }
    }

    public class ProjectEventArgs : EventArgs
    {
        public Project Project { get; set; }

        public ProjectEventArgs(Project project)
        {
            Project = project;
        }
    }

    public class ObjectEventArgs : EventArgs
    {
        public object Item { get; set; }

        public ObjectEventArgs(object item) : base()
        {
            Item = item;
        }
    }

    public class AsyncStoppedEventArgs : EventArgs
    {
        public bool CompletedSuccessfully { get; set; }

        public AsyncStoppedEventArgs(bool completedSuccessfully)
        {
            CompletedSuccessfully = completedSuccessfully;
        }
    }

    public class NotificationEventArgs : EventArgs
    {
        public string Message { get; set; }

        public NotificationType NotificationType { get; set; }

        public string PropertyName { get; set; }

        public Exception Exception { get; set; }

        public DateTime TimeStamp { get; }

        public NotificationEventArgs(NotificationType notificationType, string message, Exception ex)
        {
            NotificationType = notificationType;
            Message = message;
            Exception = ex;
            TimeStamp = DateTime.Now;
        }

        public NotificationEventArgs(NotificationType notificationType, string message, string propertyName) : this(notificationType, message)
        {
            PropertyName = propertyName;
        }

        public NotificationEventArgs(NotificationType notificationType, string message) : this(notificationType, message, default(Exception))
        { }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AsyncEventHandler(object sender, AsyncEventArgs e);

    /// <summary>
    /// Delegate for Project events.
    /// </summary>
    public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);

    /// <summary>
    /// Delegate for IExecutable events.
    /// </summary>
    public delegate void IExecuteEventHandler(object sender, IExecuteEventArgs e);

    /// <summary>
    /// Delegate for Notification events.
    /// </summary>
    public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);

    /// <summary>
    /// Delegate for general Object events.
    /// </summary>
    public delegate void ObjectEventHandler(object sender, ObjectEventArgs e);

    /// <summary>
    /// Delegate for async operation stopped event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AsyncStoppedEventHandler(object sender, AsyncStoppedEventArgs e);

    /// <summary>
    /// Delegate for Progress events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
}