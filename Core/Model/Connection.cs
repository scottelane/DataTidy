using System;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Implementation of IConnection that can be used as a base class by IConnection implementors.
    /// </summary>
    public abstract class Connection : IConnection, INotifyPropertyChanged
    {
        [JsonProperty]
        protected bool useGeneratedName = true;

        /// <summary>
        /// Event that is fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the unique connection identifier.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("ID"), GlobalisedDecription("The unique connection identifier."), Browsable(true), ReadOnly(true)]
        public virtual Guid ID { get; set; } = Guid.NewGuid();

        private string name;

        /// <summary>
        /// Gets or sets the connection name.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Name"), GlobalisedDecription("The connection name."), Browsable(true)]
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
                    if (string.IsNullOrEmpty(value))
                    {
                        useGeneratedName = true;
                        name = GenerateFriendlyName();
                    }
                    else
                    {
                        useGeneratedName = false;
                        name = value;
                    }

                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private BindingList<IDataSource> dataSources;

        /// <summary>
        /// Gets a list of data sources associated with the connection.
        /// </summary>
        [Browsable(false)]
        public BindingList<IDataSource> DataSources
        {
            get { return dataSources; }
            set
            {
                if (dataSources != value)
                {
                    dataSources = value;
                    dataSources.ListChanged += DataSources_ListChanged;
                }
            }
        }

        private void DataSources_ListChanged(object sender, ListChangedEventArgs e)
        {
            Parent.IsModified = true;
        }

        /// <summary>
        /// Gets or sets the parent project.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public Project Parent { get; set; }

        /// <summary>
        /// Initialises a new instance of the Connection class.
        /// </summary>
        public Connection(Project project)
        {
            Parent = project;
            DataSources = new BindingList<IDataSource>();
        }

        /// <summary>
        /// Generates a friendly name for the connection.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected virtual string GenerateFriendlyName()
        {
            return name;
        }

        /// <summary>
        /// Overrides the ToString method and returns the connection name.
        /// </summary>
        /// <returns>The connection name.</returns>
        public override string ToString()
        {
            return "Connection";
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
        /// Validates the Connection.
        /// </summary>
        /// <returns>The ValidationResult.</returns>
        public virtual ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(Parent == default(Project), Properties.Resources.ConnectionValidateParentProject, nameof(Parent));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Updates all parent references for the connection and child data sources. Used for serialisation purposes to maintain the parent hierarchy required for propertygrid type converters.
        /// </summary>
        /// <param name="parentProject">The parent Project.</param>
        public virtual void UpdateParentReferences(Project parentProject)
        {
            Parent = parentProject;

            foreach (IDataSource dataSource in DataSources)
            {
                if (dataSource != default(IDataSource))  // todo - why is this happening?
                {
                    dataSource.UpdateParentReferences(this);
                }
            }
        }

        /// <summary>
        /// Creates a clone of the connection.
        /// </summary>
        /// <returns>The cloned connection.</returns>
        public virtual IConnection Clone()
        {
            IConnection clone = (IConnection)MemberwiseClone();
            clone.ID = Guid.NewGuid();
            clone.Name = string.Concat(Name, " (Copy)");
            clone.DataSources = new BindingList<IDataSource>(DataSources.Select(dataSource => dataSource.Clone(false)).ToList());
            clone.UpdateParentReferences(Parent);

            return clone;
        }

        /// <summary>
        /// Clears the connection cache.
        /// </summary>
        protected virtual void ClearConnectionCache()
        {
            ConnectionCache cache = new ConnectionCache(this);
            cache.Clear();
        }
    }
}
