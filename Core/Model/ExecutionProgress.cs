namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provides reporting of execution progress.
    /// </summary>
    public class ExecutionProgress
    {
        /// <summary>
        /// Gets or sets the type of progress that is being reported.
        /// </summary>
        public ProgressType ProgressType { get; set; }

        /// <summary>
        /// Gets or sets the execution stage.
        /// </summary>
        public ExecutionStage ExecutionStage { get; set; }

        /// <summary>
        /// Gets or sets the total number of items that have been executed for the specified execution stage.
        /// </summary>
        public int ExecutedItemCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of items to execute.
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of items to execute for the specified execution stage.
        /// </summary>
        public int OperationIndex { get; set; }

        /// <summary>
        /// Gets or sets the type of notification.
        /// </summary>
        public NotificationType NotificationType { get; set; }

        /// <summary>
        /// Gets or sets the notification message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initialises a new instance of the ExecutionProgress class for reporting execution progress messages.
        /// </summary>
        /// <param name="notificationType">The notification type.</param>
        /// <param name="message">The progress message.</param>
        public ExecutionProgress(NotificationType notificationType, string message)
        {
            NotificationType = notificationType;
            Message = message;
            ProgressType = ProgressType.Notification;
        }

        /// <summary>
        /// Initialises a new instance of the ExecutionProgress class for numeric execution progress. 
        /// </summary>
        /// <param name="executionStage">The operation type.</param>
        /// <param name="executedItemCount">The total number of items that have been executed for the specified execution stage.</param>
        /// <param name="totalItemCount">The total number of items to execute for the specified execution stage.</param>
        public ExecutionProgress(ExecutionStage executionStage, int executedItemCount, int totalItemCount)
        {
            ExecutionStage = executionStage;
            ExecutedItemCount = executedItemCount;
            TotalItemCount = totalItemCount;
            ProgressType = ProgressType.ItemProgress;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationIndex"></param>
        public ExecutionProgress(int operationIndex)
        {
            OperationIndex = operationIndex;
            ProgressType = ProgressType.OperationProgress;
        }
    }

    /// <summary>
    /// Defines the types of progress that can be reported.
    /// </summary>
    public enum ProgressType
    {
        Notification,
        ItemProgress,
        OperationProgress
    }

    /// <summary>
    /// Defines the execution stages that can be reported.
    /// </summary>
    public enum ExecutionStage
    {
        Extract,
        Transform,
        Load
    }
}
