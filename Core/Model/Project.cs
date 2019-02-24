using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A collection of connections, data sources, batches and operations that deliver the applications core functionality.
    /// </summary>
    public class Project : IDataSourcesProvider, IExecutable, INotifyPropertyChanged
    {
        private const bool DEFAULT_CONTINUE_ON_ERROR = false;

        #region Properties

        private bool isModified = false;

        public bool IsModified
        {
            get { return isModified; }
            set
            {
                if (isModified != value)
                {
                    isModified = value;
                    OnPropertyChanged(nameof(IsModified));
                }
            }
        }

        private string path;

        public string Path
        {
            get { return path; }
            private set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Name"), GlobalisedDecription("The project name."), Browsable(true)]
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    return System.IO.Path.GetFileNameWithoutExtension(Path);
                }

                return Properties.Resources.ProjectDefaultName;
            }
        }

        //project.Name = System.IO.Path.GetFileNameWithoutExtension(Path);


        /// <summary>
        /// Gets or sets a value indicating whether subsequent batches are processed if an error is encountered when the project is executed.
        /// </summary>
        [GlobalisedCategory("Processing"), GlobalisedDisplayName("Continue On Error"), GlobalisedDecription("Determines whether subsequent batches are processed if an error is encountered."), Browsable(true), DefaultValue(DEFAULT_CONTINUE_ON_ERROR)]
        public bool ContinuteOnError { get; set; } = DEFAULT_CONTINUE_ON_ERROR;

        private BindingList<Batch> batches;

        /// <summary>
        /// Gets or sets a list of associated Batch objects.
        /// </summary>
        [Browsable(false)]
        public BindingList<Batch> Batches
        {
            get { return batches; }
            set
            {
                if (batches != value)
                {
                    batches = value;
                    batches.ListChanged += Batches_ListChanged; // todo - switch to onpropertychanged?
                }
            }
        }

        private void Batches_ListChanged(object sender, ListChangedEventArgs e)
        {
            IsModified = true;
        }

        private BindingList<IConnection> connections;

        /// <summary>
        /// Gets or sets a list of associated Connection objects.
        /// </summary>
        [Browsable(false)]
        public BindingList<IConnection> Connections
        {
            get { return connections; }
            set
            {
                if (connections != value)
                {
                    connections = value;
                    connections.ListChanged += Connections_ListChanged;
                }
            }
        }

        private void Connections_ListChanged(object sender, ListChangedEventArgs e)
        {
            IsModified = true;
        }

        /// <summary>
        /// Gets the total number of operations in all associated batches.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Operation Count"), GlobalisedDecription("The total number of operations in all associated batches."), Browsable(true), JsonIgnore]
        public int ItemCount
        {
            get { return Batches.Sum(batch => batch.ItemCount); }
        }

        /// <summary>
        /// Gets or sets the expanded state of data sources in the user interface.
        /// </summary>
        public Dictionary<Guid, bool> DataSourceState { get; set; } = new Dictionary<Guid, bool>();

        /// <summary>
        /// Gets or sets the expanded state of operations in the user interface.
        /// </summary>
        public Dictionary<Guid, bool> OperationState { get; set; } = new Dictionary<Guid, bool>();

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
        /// Raises the Executed event.
        /// </summary>
        /// <param name="e">The ExecutableEventArgs.</param>
        protected virtual void OnExecuted(ExecutableEventArgs e)
        {
            Executed?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Project class.
        /// </summary>
        public Project()
        {
            Batches = new BindingList<Batch>();
            Connections = new BindingList<IConnection>();
        }

        /// <summary>
        /// Saves the Project to a file in JSON format.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void Save(string path)
        {
            string backupFileName = string.Concat(path, ".save.bak");

            if (File.Exists(path))
            {
                File.Copy(path, backupFileName, true);
                File.SetAttributes(backupFileName, File.GetAttributes(backupFileName) | FileAttributes.Hidden);
            }

            try
            {
                StreamWriter streamWriter = new StreamWriter(path);

                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    JsonSerializerSettings settings = new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Objects
                    };
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    serializer.Serialize(jsonWriter, this);
                }

                IsModified = false;
                Path = path;
                File.Delete(backupFileName);
            }
            catch
            {
                File.Copy(backupFileName, path, true);
                throw;
            }
        }

        /// <summary>
        /// Loads a Project from a JSON file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The Project.</returns>
        public static Project Load(string path)
        {
            try
            {
                StreamReader streamReader = new StreamReader(path);

                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Objects
                    };
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    Project project = (Project)serializer.Deserialize(jsonReader, typeof(Project));
                    project.UpdateParentReferences();
                    project.IsModified = false;
                    project.Path = path;

                    return project;
                }
            }
            catch
            {
                throw;
            }
            finally
            { }
        }

        // http://codeblog.larsholm.net/2011/06/embed-dlls-easily-in-a-net-assembly/
        // todo - merge SDK assemblies into plugin and remove from debug folder
        private class TypeBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    if (assembly.GetName().Name == assemblyName)
                    {
                        return assembly.GetType(typeName);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Updates the parent references for all child batches. Used for serialisation purposes to maintain the parent hierarchy required for propertygrid type converters.
        /// </summary>
        internal void UpdateParentReferences()
        {
            Batches.ToList().ForEach(batch => batch.UpdateParentReferences(this));
            Connections.ToList().ForEach(connection => connection.UpdateParentReferences(this));
        }

        /// <summary>
        /// Validates the project and all child Batch objects.
        /// </summary>
        /// <returns>The ValidationResult.</returns>
        public ValidationResult Validate()
        {
            ValidationResult projectResult = new ValidationResult();
            projectResult.AddErrorIf(Batches.Count == 0, "There are no batches in the project");

            foreach (Batch batch in Batches)
            {
                ValidationResult batchResult = batch.Validate();
                projectResult.Errors.AddRange(batchResult.Errors);
            }

            return projectResult;
        }

        public IExecutable FindExecutableItem(Guid id)
        {
            IExecutable item = default(IExecutable);

            foreach (Batch batch in Batches)
            {
                if (batch.ID == id)
                {
                    return batch;
                }
                else
                {
                    foreach (IOperation operation in batch.Operations)
                    {
                        if (operation.ID == id && operation is IExecutable executableOperation)
                        {
                            return executableOperation;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Executes all associated Batch objects.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        public void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format("Executing {0}", Name)));

            for (int batchIndex = 0; batchIndex < Batches.Count; batchIndex++)
            {
                try
                {
                    Batch batch = Batches[batchIndex];
                    batch.Execute(cancel, progress);
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

            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format("Successfully executed {0}", Name)));
            OnExecuted(new ExecutableEventArgs(this));
        }

        /// <summary>
        /// Overrides the ToString method and returns the Project name.
        /// </summary>
        /// <returns>The Project name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets a list of all DataSource objects in the Project.
        /// </summary>
        /// <returns>The list of DataSource objects.</returns>
        public BindingList<IDataSource> GetDataSources()
        {
            BindingList<IDataSource> dataSources = new BindingList<IDataSource>();

            foreach (IConnection connection in Connections)
            {
                foreach (DataSource dataSource in connection.DataSources)
                {
                    dataSources.Add(dataSource);
                }
            }

            return dataSources;
        }
    }
}
