using System;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Base class for all operations that perform data loading or miscellaneous actions.
    /// </summary>
    public abstract class Operation : IOperation, IExecutable, IDataSourcesProvider, INotifyPropertyChanged
    {
        private const bool DEFAULT_CLEAR_CACHE = true;
        private const bool DEFAULT_CONTINUE_ON_ERROR = false;
        private const bool DEFAULT_ENABLED = true;

        #region Properties

        [JsonProperty]
        protected bool useGeneratedName = true;

        /// <summary>
        /// Gets or sets the unique operation identifier.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("ID"), GlobalisedDecription("The unique operation identifier."), Browsable(true), ReadOnly(true)]
        public Guid ID { get; set; } = Guid.NewGuid();

        private string name;

        /// <summary>
        /// Gets or sets the operation name.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Name"), GlobalisedDecription("The operation name."), Browsable(true)]
        public virtual string Name
        {
            get
            {
                if (useGeneratedName)
                {
                    name = GenerateFriendlyName();
                }

                return name;
            }
            set
            {
                if (name != value)
                {
                    string friendlyName = GenerateFriendlyName();

                    if (string.IsNullOrEmpty(value) || value == friendlyName)
                    {
                        useGeneratedName = true;
                        name = friendlyName;
                    }
                    else
                    {
                        if (name != default(string))
                        {
                            useGeneratedName = false;
                        }

                        name = value;
                    }

                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool clearCache = DEFAULT_CLEAR_CACHE;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Clear Cache"), GlobalisedDecription("If True, clears all cached data after the operation has executed."), Browsable(true), DefaultValue(DEFAULT_CLEAR_CACHE)]
        public bool ClearCache
        {
            get { return clearCache; }
            set
            {
                if (clearCache != value)
                {
                    clearCache = value;
                }

                OnPropertyChanged(nameof(ClearCache));
            }
        }

        private bool enabled = DEFAULT_ENABLED;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Enabled"), GlobalisedDecription("If True, allows the operation to be executed individually or in a batch."), Browsable(true), DefaultValue(DEFAULT_ENABLED)]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                }

                OnPropertyChanged(nameof(Enabled));
            }
        }

        protected Batch parentBatch;

        /// <summary>
        /// Gets or sets the parent Batch.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public Batch ParentBatch
        {
            get { return parentBatch; }
            set { parentBatch = value; }
        }

        /// <summary>
        /// Gets the number of operations run when executed.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public int ItemCount
        {
            get { return 1; }
        }

        #endregion

        #region Events

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
        /// Raises the Executed event and clears the operation cache.
        /// </summary>
        /// <param name="e">The ExecutableEventArgs.</param>
        protected virtual void OnExecuted(ExecutableEventArgs e)
        {
            if (clearCache && e.Executable is IOperation operation)
            {
                OperationCache cache = new OperationCache(operation);
                cache.Clear();
            }

            Executed?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Operation class with the specified parent Batch.
        /// </summary>
        /// <param name="parentBatch">The parent Batch.</param>
        [JsonConstructor]
        public Operation(Batch parentBatch)
        {
            this.parentBatch = parentBatch;
        }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected virtual string GenerateFriendlyName()
        {
            return "Operation";
        }

        /// <summary>
        /// Refreshes the Name based on the current state.
        /// </summary>
        protected virtual void RefreshName()
        {
            if (useGeneratedName)
            {
                Name = GenerateFriendlyName();
            }
        }

        /// <summary>
        /// Updates all parent references for the operation. Used for serialisation purposes to maintain the parent hierarchy required for propertygrid type converters.
        /// </summary>
        /// <param name="parentBatch">The batch that the operation belongs to.</param>
        public virtual void UpdateParentReferences(Batch parentBatch)
        {
            ParentBatch = parentBatch;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="addSuffix"></param>
        /// <returns></returns>
        public virtual IOperation Clone(bool addSuffix)
        {
            IOperation clone = (IOperation)MemberwiseClone();
            clone.ID = Guid.NewGuid();
            clone.Name = string.Concat(Name, addSuffix ? " (Copy)" : string.Empty);

            return clone;
        }

        /// <summary>
        /// Overrides the ToString method and returns the operation name.
        /// </summary>
        /// <returns>The operation name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets a list of all data sources.
        /// </summary>
        /// <returns>A list of DataSource items.</returns>
        public BindingList<IDataSource> GetDataSources()
        {
            return ((IDataSourcesProvider)ParentBatch.ParentProject).GetDataSources();
        }

        public abstract ValidationResult Validate();

        public abstract void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress);
    }
}
