using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using System.IO;
using System.Threading;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Creates an XML file.
    /// </summary>
    [Operation(typeof(XmlFileCreateOperation), "ScottLane.DataTidy.File.Resources.XmlFileCreateOperation.png")]
    public class XmlFileCreateOperation : FileOperation, IExecutable
    {
        #region Properties

        /// <summary>
        /// The output file path.
        /// </summary>
        [GlobalisedCategory(typeof(XmlFileCreateOperation), nameof(OutputPath)), GlobalisedDisplayName(typeof(XmlFileCreateOperation), nameof(OutputPath)), GlobalisedDecription(typeof(XmlFileCreateOperation), nameof(OutputPath)), Editor(typeof(XmlSaveFileNameEditor), typeof(UITypeEditor))]
        public override string OutputPath
        {
            get { return base.OutputPath; }
            set { base.OutputPath = value; }
        }

        /// <summary>
        /// The data source.
        /// </summary>
        [GlobalisedCategory(typeof(XmlFileCreateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(XmlFileCreateOperation), nameof(DataSource)), GlobalisedDecription(typeof(XmlFileCreateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter))]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the XmlFileCreateOperation with the specified batch.
        /// </summary>
        /// <param name="batch">The parent batch.</param>
        public XmlFileCreateOperation(Batch batch) : base(batch)
        { }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.XmlFileCreateOperationFriendlyName, OutputPath ?? Properties.Resources.XmlFileCreateOperationFriendlyNamePath, DataSource?.Name ?? Properties.Resources.XmlFileCreateOperationFriendlyNameDataSource);
        }

        /// <summary>
        /// Creates an XML file.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.XmlFileCreateOperationExecute, Name)));

            DataTable dataTable = default(DataTable);

            try
            {
                dataTable = DataSource.GetDataTable(cancel, progress);
                progress?.Report(new ExecutionProgress(ExecutionStage.Transform, dataTable.Rows.Count, dataTable.Rows.Count));
                cancel.ThrowIfCancellationRequested();

                progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.XmlFileCreateOperationExecuteCreating, OutputPath)));

                using (FileStream stream = System.IO.File.Create(OutputPath))
                {
                    dataTable.WriteXml(stream);
                }

                cancel.ThrowIfCancellationRequested();
            }
            finally
            {
                progress?.Report(new ExecutionProgress(ExecutionStage.Load, dataTable?.Rows.Count ?? 0, dataTable?.Rows.Count ?? 0));
            }

            OnExecuted(new ExecutableEventArgs(this));
            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.XmlFileCreateOperationExecuteSuccessful, OutputPath)));
        }
    }
}
