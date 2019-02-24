using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Creates a JSON file.
    /// </summary>
    [Operation(typeof(JsonFileCreateOperation), "ScottLane.DataTidy.File.Resources.JsonFileCreateOperation.png")]
    public class JsonFileCreateOperation : FileOperation
    {
        #region Properties

        /// <summary>
        /// The output file path.
        /// </summary>
        [GlobalisedCategory(typeof(JsonFileCreateOperation), nameof(OutputPath)), GlobalisedDisplayName(typeof(JsonFileCreateOperation), nameof(OutputPath)), GlobalisedDecription(typeof(JsonFileCreateOperation), nameof(OutputPath)), Editor(typeof(JsonSaveFileNameEditor), typeof(UITypeEditor))]
        public override string OutputPath
        {
            get { return base.OutputPath; }
            set { base.OutputPath = value; }
        }

        /// <summary>
        /// The data source.
        /// </summary>
        [GlobalisedCategory(typeof(JsonFileCreateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(JsonFileCreateOperation), nameof(DataSource)), GlobalisedDecription(typeof(JsonFileCreateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter))]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }

        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the JsonFileCreateOperation class with the specified parent Batch.
        /// </summary>
        /// <param name="parentBatch">The parent Batch.</param>
        public JsonFileCreateOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Creates a JSON file.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.JsonFileCreateOperationExecute, Name)));

            DataTable dataTable = default(DataTable);

            try
            {
                dataTable = DataSource.GetDataTable(cancel, progress);
                progress?.Report(new ExecutionProgress(ExecutionStage.Transform, dataTable.Rows.Count, dataTable.Rows.Count));
                cancel.ThrowIfCancellationRequested();

                progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.JsonFileCreateOperationExecuteCreating, OutputPath)));

                StreamWriter streamWriter = new StreamWriter(OutputPath);

                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    serializer.Serialize(jsonWriter, dataTable);
                }

                cancel.ThrowIfCancellationRequested();
            }
            finally
            {
                progress?.Report(new ExecutionProgress(ExecutionStage.Load, dataTable?.Rows.Count ?? 0, dataTable?.Rows.Count ?? 0));
            }

            OnExecuted(new ExecutableEventArgs(this));
            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.JsonFileCreateOperationExecuteSuccessful, OutputPath)));
        }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.JsonFileCreateOperationFriendlyName, OutputPath ?? Properties.Resources.JsonFileCreateOperationFriendlyNamePath, DataSource?.Name ?? Properties.Resources.JsonFileCreateOperationFriendlyNameDataSource);
        }
    }
}
