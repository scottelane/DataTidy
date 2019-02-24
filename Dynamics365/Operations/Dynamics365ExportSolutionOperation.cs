using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    [Operation(typeof(Dynamics365ExportSolutionOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365ExportSolutionOperation.png")]
    public class Dynamics365ExportSolutionOperation : Dynamics365Operation, IDynamics365SolutionsProvider, ICustomMenuItemProvider
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Dynamics 365 organisation to delete records from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ExportSolutionOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365ExportSolutionOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365ExportSolutionOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        private Dynamics365Solution solution;

        /// <summary>
        /// Gets or sets the solution to export.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ExportSolutionOperation), nameof(Solution)), GlobalisedDisplayName(typeof(Dynamics365ExportSolutionOperation), nameof(Solution)), GlobalisedDecription(typeof(Dynamics365ExportSolutionOperation), nameof(Solution)), TypeConverter(typeof(Dynamics365SolutionConverter)), JsonProperty(Order = 2)]
        public Dynamics365Solution Solution
        {
            get { return solution; }
            set
            {
                if (solution != value)
                {
                    solution = value;
                    OnPropertyChanged(nameof(Solution));
                }
            }
        }

        public enum NamingConventionType
        {
            Custom,
            UniqueName,
            UniqueNameVersion,
            FriendlyName,
            FriendlyNameVersion
        }

        private NamingConventionType namingConvention;

        [GlobalisedCategory(typeof(Dynamics365ExportSolutionOperation), nameof(NamingConvention)), GlobalisedDisplayName(typeof(Dynamics365ExportSolutionOperation), nameof(NamingConvention)), GlobalisedDecription(typeof(Dynamics365ExportSolutionOperation), nameof(NamingConvention)), DefaultValue(NamingConventionType.Custom), JsonProperty(Order = 3)]
        public NamingConventionType NamingConvention
        {
            get { return namingConvention; }
            set
            {
                if (namingConvention != value)
                {
                    namingConvention = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(NamingConvention));
                }
            }
        }

        private string outputPath;

        /// <summary>
        /// The output file path if the 'Custom' naming convention is used.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ExportSolutionOperation), nameof(OutputPath)), GlobalisedDisplayName(typeof(Dynamics365ExportSolutionOperation), nameof(OutputPath)), GlobalisedDecription(typeof(Dynamics365ExportSolutionOperation), nameof(OutputPath)), EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(true), JsonProperty(Order = 4)]
        public virtual string OutputPath
        {
            get { return outputPath; }
            set
            {
                if (outputPath != value)
                {
                    outputPath = value;
                    OnPropertyChanged(nameof(OutputPath));
                }
            }
        }

        private string outputDirectory;

        /// <summary>
        /// The output file directory.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365ExportSolutionOperation), nameof(OutputDirectory)), GlobalisedDisplayName(typeof(Dynamics365ExportSolutionOperation), nameof(OutputDirectory)), GlobalisedDecription(typeof(Dynamics365ExportSolutionOperation), nameof(OutputDirectory)), EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor)), Browsable(false), JsonProperty(Order = 4)]
        public virtual string OutputDirectory
        {
            get { return outputDirectory; }
            set
            {
                if (outputDirectory != value)
                {
                    outputDirectory = value;
                    OnPropertyChanged(nameof(OutputDirectory));
                    RefreshName();
                }
            }
        }

        private bool isManaged;

        [GlobalisedCategory(typeof(Dynamics365ExportSolutionOperation), nameof(IsManaged)), GlobalisedDisplayName(typeof(Dynamics365ExportSolutionOperation), nameof(IsManaged)), GlobalisedDecription(typeof(Dynamics365ExportSolutionOperation), nameof(IsManaged)), DefaultValue(false)]
        public bool IsManaged
        {
            get { return isManaged; }
            set
            {
                if (isManaged != value)
                {
                    isManaged = value;
                    OnPropertyChanged(nameof(IsManaged));
                    RefreshName();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a instance of the Dynamics365PublishAllOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365ExportSolutionOperation(Batch parentBatch) : base(parentBatch)
        {
            RefreshBrowsableFields();
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(OutputPath), namingConvention == NamingConventionType.Custom);
            CoreUtility.SetBrowsable(this, nameof(OutputDirectory), namingConvention != NamingConventionType.Custom);
        }

        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            string exportPath = GetExportPath();
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365ExportSolutionOperationExecute, Solution.FriendlyName, exportPath)));

            using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
            {
                ExportSolutionRequest request = new ExportSolutionRequest()
                {
                    Managed = isManaged,
                    SolutionName = solution.UniqueName
                };

                ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)proxy.Execute(request);
                byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
                File.WriteAllBytes(exportPath, exportXml);
            }

            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365ExportSolutionOperationExecuteSuccessful, Solution.FriendlyName, exportPath)));
            OnExecuted(new ExecutableEventArgs(this));
        }

        private string GetExportPath()
        {
            string exportPath = default(string);

            switch (namingConvention)
            {
                case NamingConventionType.Custom:
                    exportPath = outputPath;
                    break;
                case NamingConventionType.FriendlyName:
                    exportPath = string.Format("{0}{1}{2}.zip", outputDirectory, Path.DirectorySeparatorChar, Solution.FriendlyName);
                    break;
                case NamingConventionType.FriendlyNameVersion:
                    exportPath = string.Format("{0}{1}{2} {3}.zip", outputDirectory, Path.DirectorySeparatorChar, Solution.FriendlyName, Solution.Version);
                    break;
                case NamingConventionType.UniqueName:
                    exportPath = string.Format("{0}{1}{2}.zip", outputDirectory, Path.DirectorySeparatorChar, Solution.UniqueName);
                    break;
                case NamingConventionType.UniqueNameVersion:
                    exportPath = string.Format("{0}{1}{2} {3}.zip", outputDirectory, Path.DirectorySeparatorChar, Solution.UniqueName, Solution.Version);
                    break;
            }

            return exportPath;
        }

        public override ScottLane.DataTidy.Core.ValidationResult Validate()
        {
            ScottLane.DataTidy.Core.ValidationResult result = base.Validate();

            result.AddErrorIf(Solution == default(Dynamics365Solution), "Please select a Solution", nameof(Solution));
            result.AddErrorIf(NamingConvention == default(NamingConventionType), "Please select a Naming Convention", nameof(NamingConvention));

            if (NamingConvention != default(NamingConventionType))
            {
                result.AddErrorIf(NamingConvention == NamingConventionType.Custom && string.IsNullOrEmpty(OutputPath), "Please specify the Output Path", nameof(OutputPath));
                result.AddErrorIf(NamingConvention != NamingConventionType.Custom && string.IsNullOrEmpty(OutputDirectory), "Please specify the Output Directory", nameof(OutputDirectory));
            }

            return result;
        }

        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365ExportSolutionOperationFriendlyName, Solution?.FriendlyName ?? Properties.Resources.Dynamics365ExportSolutionOperationFriendlyNameSolution, Connection?.Name ?? Properties.Resources.Dynamics365ExportSolutionOperationFriendlyNameConnection);
        }

        public List<Dynamics365Solution> GetSolutions()
        {
            List<Dynamics365Solution> solutions = new List<Dynamics365Solution>();

            try
            {
                solutions.AddRange(Dynamics365Solution.GetSolutions(Connection));
            }
            catch
            { }

            return solutions;
        }

        public List<CustomMenuItem> GetCustomMenuItems()
        {
            string exportPath = GetExportPath();

            List<CustomMenuItem> menuItems = new List<CustomMenuItem>
            {
                new CustomMenuItem()
                {
                    Text = Properties.Resources.Dynamics365ExportSolutionOperationOpenFolderText,
                    ToolTip = Properties.Resources.Dynamics365ExportSolutionOperationOpenFolderTooltip,
                    Icon = Properties.Resources.Dynamics365ExportSolutionOperationOpenFolder,
                    Item = this,
                    Enabled = System.IO.File.Exists(exportPath),
                    SynchronousEventHandler = OpenFolder
                }
            };

            return menuItems;
        }

        private void OpenFolder()
        {
            string exportPath = GetExportPath();

            if (System.IO.File.Exists(exportPath))
            {
                Process.Start(System.IO.Path.GetDirectoryName(exportPath));
            }
            else
            {
                throw new ApplicationException(string.Format(Properties.Resources.Dynamics365ExportSolutionOperationOpenFolderNotFound, exportPath));
            }
        }
    }
}
