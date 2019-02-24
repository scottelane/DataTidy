using System;
using System.Threading;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Interface for all objects that can be executed by the application.
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Gets the item name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the total number of items that will be executed.
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// Validates the item before execution.
        /// </summary>
        /// <returns>The validation result.</returns>
        ValidationResult Validate();

        /// <summary>
        /// Executes the item and any child items.
        /// </summary>
        /// <param name="cancel"></param>
        /// <param name="progress"></param>
        void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress);

        /// <summary>
        /// Event that is raised when the item starts executing.
        /// </summary>
        event ExecuteHandler Executing;

        /// <summary>
        /// Event that is raised when the item has finished executing.
        /// </summary>
        event ExecuteHandler Executed;
    }

    /// <summary>
    /// Delegate for execution-related events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The ExecutableEventArgs.</param>
    public delegate void ExecuteHandler(object sender, ExecutableEventArgs e);

    /// <summary>
    /// Encapsulates data for execution-related events.
    /// </summary>
    public class ExecutableEventArgs
    {
        /// <summary>
        /// Gets or sets the IExecutable that is running.
        /// </summary>
        public IExecutable Executable { get; set; }

        /// <summary>
        /// Initialises a new instance of the ExecutableEventArgs with the specified IExecutable.
        /// </summary>
        /// <param name="executable">The IExecutable.</param>
        public ExecutableEventArgs(IExecutable executable)
        {
            Executable = executable;
        }
    }
}