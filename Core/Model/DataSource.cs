using System;
using System.Data;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Base implementation data sources that provide data to operations for processing.
    /// </summary>
    public abstract class DataSource : IDataSource, INotifyPropertyChanged
    {
        [JsonProperty]
        protected bool useGeneratedName = true;

        /// <summary>
        /// Gets or sets the unique data source identifier.
        /// </summary>
        [GlobalisedCategory(typeof(DataSource), nameof(ID)), GlobalisedDisplayName(typeof(DataSource), nameof(ID)), GlobalisedDecription(typeof(DataSource), nameof(ID)), Browsable(true), ReadOnly(true)]
        public Guid ID { get; set; } = Guid.NewGuid();

        private string name;

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        [GlobalisedCategory(typeof(DataSource), nameof(Name)), GlobalisedDisplayName(typeof(DataSource), nameof(Name)), GlobalisedDecription(typeof(DataSource), nameof(Name)), Browsable(true)]
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

        /// <summary>
        /// Gets the parent Connection.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public IConnection Parent { get; private set; }

        /// <summary>
        /// Event that is fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initialises a new instance of the DataSource class with the specified parent Connection.
        /// </summary>
        /// <param name="parentConnection">The parent Connection.</param>
        [JsonConstructor]
        public DataSource(IConnection parentConnection)
        {
            Parent = parentConnection;
        }

        /// <summary>
        /// Generates a friendly name for the data source.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected virtual string GenerateFriendlyName()
        {
            return Properties.Resources.DataSourceFriendlyName;
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
        /// Validates the DataSource.
        /// </summary>
        /// <returns>The ValidationResult.</returns>
        public virtual ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(Parent == default(IConnection), Properties.Resources.DataSourceValidateParentConnection, nameof(Parent));

                if (Parent != default(IConnection))
                {
                    result.Errors.AddRange(Parent.Validate().Errors);
                }
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Updates all parent references for the data source. Used for serialisation purposes to maintain the parent hierarchy required for propertygrid type converters.
        /// </summary>
        /// <param name="parentConnection">The parent Connection.</param>
        public virtual void UpdateParentReferences(IConnection parentConnection)
        {
            Parent = parentConnection;
        }

        /// <summary>
        /// Clones the data source and optionally adds a suffix to the name.
        /// </summary>
        /// <param name="addSuffix"></param>
        /// <returns></returns>
        public virtual IDataSource Clone(bool addSuffix)
        {
            IDataSource clone = (IDataSource)MemberwiseClone();
            clone.ID = Guid.NewGuid();
            clone.Name = string.Concat(Name, addSuffix ? Properties.Resources.DataSourceCloneSuffix : string.Empty);

            return clone;
        }

        /// <summary>
        /// Overrides the ToString method and returns the data source name.
        /// </summary>
        /// <returns>The data source name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != default(PropertyChangedEventHandler))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets the data source records as a DataTable and reports extraction progres.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The data table.</returns>
        public abstract DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress);

        /// <summary>
        /// Gets the data source records as a DataTable.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The data table.</returns>
        public virtual DataTable GetDataTable(CancellationToken cancel)
        {
            return GetDataTable(cancel, default(IProgress<ExecutionProgress>));
        }

        /// <summary>
        /// Gets the columns in the data source as a DataColumnCollection.
        /// </summary>
        /// <returns>The columns.</returns>
        public abstract DataColumnCollection GetDataColumns();
    }
}
