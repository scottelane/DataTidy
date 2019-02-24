using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A group of operations that can be executed in sequence.
    /// </summary>
    public class Batch : IExecutable, INotifyPropertyChanged
    {
        private const bool DEFAULT_CONTINUE_ON_ERROR = false;
        private const string DEFAULT_NAME = "New Batch";

        #region Properties

        /// <summary>
        /// Gets or sets the unique batch identifier.
        /// </summary>
        [GlobalisedCategory(typeof(Batch), nameof(ID)), GlobalisedDisplayName(typeof(Batch), nameof(ID)), GlobalisedDecription(typeof(Batch), nameof(ID)), Browsable(true), ReadOnly(true)]
        public Guid ID { get; set; } = Guid.NewGuid();

        private string name = DEFAULT_NAME;

        /// <summary>
        /// Gets or sets the batch name.
        /// </summary>
        [GlobalisedCategory(typeof(Batch), nameof(Name)), GlobalisedDisplayName(typeof(Batch), nameof(Name)), GlobalisedDecription(typeof(Batch), nameof(Name)), DefaultValue(DEFAULT_NAME), Browsable(true)]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool continuteOnError = DEFAULT_CONTINUE_ON_ERROR;

        /// <summary>
        /// Gets or sets a value that determines whether subsequent operations are processed if an error occurs.
        /// </summary>
        [GlobalisedCategory(typeof(Batch), nameof(ContinuteOnError)), GlobalisedDisplayName(typeof(Batch), nameof(ContinuteOnError)), GlobalisedDecription(typeof(Batch), nameof(ContinuteOnError)), DefaultValue(DEFAULT_CONTINUE_ON_ERROR)]
        public bool ContinuteOnError
        {
            get { return continuteOnError; }
            set
            {
                if (continuteOnError != value)
                {
                    continuteOnError = value;
                    OnPropertyChanged(nameof(ContinuteOnError));
                }
            }
        }

        private BindingList<IOperation> operations;

        /// <summary>
        /// Gets or sets a list of associated Operation objects.
        /// </summary>
        [Browsable(false)]
        public BindingList<IOperation> Operations
        {
            get { return operations; }
            set
            {
                if (operations != value)
                {
                    operations = value;
                    operations.ListChanged += Operations_ListChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent Project.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public Project ParentProject { get; private set; }

        /// <summary>
        /// Gets the number of steps in the batch.
        /// </summary>
        [GlobalisedCategory(typeof(Batch), nameof(ItemCount)), GlobalisedDisplayName(typeof(Batch), nameof(ItemCount)), GlobalisedDecription(typeof(Batch), nameof(ItemCount)), JsonIgnore]
        public int ItemCount
        {
            get { return Operations.Where(operation => operation.Enabled).ToList().Count; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Batch class with the specified parent Project.
        /// </summary>
        /// <param name="parentProject">The parent Project.</param>
        public Batch(Project parentProject)
        {
            ParentProject = parentProject;
            Operations = new BindingList<IOperation>();
        }

        private void Operations_ListChanged(object sender, ListChangedEventArgs e)
        {
            ParentProject.IsModified = true;
        }

        /// <summary>
        /// Validates the Batch.
        /// </summary>
        /// <returns>True if the Batch is valid, false otherwise.</returns>
        public ValidationResult Validate()
        {
            ValidationResult batchResult = new ValidationResult();
            batchResult.AddErrorIf(Operations.Count == 0, Properties.Resources.BatchValidateNoOperations);

            foreach (IOperation operation in Operations)
            {
                ValidationResult operationResult = operation.Validate();
                batchResult.Errors.AddRange(operationResult.Errors);
            }

            return batchResult;
        }

        /// <summary>
        /// Executes all Operations in the Batch.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        public void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.BatchExecuting, Name)));
            List<IOperation> enabledOperations = Operations.Where(operation => operation.Enabled).ToList();

            for (int operationIndex = 0; operationIndex < enabledOperations.Count(); operationIndex++)
            {
                try
                {
                    if (operationIndex > 0 && operationIndex < Operations.Count)
                    {
                        progress?.Report(new ExecutionProgress(operationIndex));
                    }

                    if (enabledOperations[operationIndex] is IExecutable operation)
                    {
                        operation.Execute(cancel, progress);
                    }

                    cancel.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (ContinuteOnError)
                    {
                        progress?.Report(new ExecutionProgress(NotificationType.Warning, ex.Message));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.BatchExecuted, Name)));
            OnExecuted(new ExecutableEventArgs(this));
        }

        /// <summary>
        /// Updates all parent references for the Batch. Used for serialisation purposes to maintain the parent hierarchy required for propertygrid type converters.
        /// </summary>
        /// <param name="project">The project that the Batch belongs to.</param>
        public void UpdateParentReferences(Project project)
        {
            ParentProject = project;
            Operations.ToList().ForEach(operation => operation.UpdateParentReferences(this));
        }

        /// <summary>
        /// Clones the Batch.
        /// </summary>
        /// <returns>The cloned Batch.</returns>
        public Batch Clone()
        {
            Batch clone = (Batch)MemberwiseClone();
            clone.ID = Guid.NewGuid();
            clone.Name = string.Concat(Name, Properties.Resources.BatchCopy);
            clone.Operations = new BindingList<IOperation>(Operations.Select(operation => operation.Clone(false)).ToList());
            clone.UpdateParentReferences(ParentProject);

            return clone;
        }

        /// <summary>
        /// Gets the Batch name.
        /// </summary>
        /// <returns>The Batch name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Event that is raised when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Event that is raised when the item starts executing.
        /// </summary>
        public event ExecuteHandler Executing;

        /// <summary>
        /// Raises the Executing event.
        /// </summary>
        /// <param name="e">The ExecutableEventArgs.</param>
        protected virtual void OnExecuting(ExecutableEventArgs e)
        {
            Executing?.Invoke(this, e);
        }

        /// <summary>
        /// Event that is raised when the item has finished executing.
        /// </summary>
        public event ExecuteHandler Executed;

        /// <summary>
        /// Raises the Executed event.
        /// </summary>
        /// <param name="e">The ExecutableEventArgs.</param>
        protected virtual void OnExecuted(ExecutableEventArgs e)
        {
            Executed?.Invoke(this, e);
        }
    }
}
