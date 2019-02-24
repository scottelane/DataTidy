using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Threading;
using CsvHelper;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Creates a CSV file.
    /// </summary>
    [Operation(typeof(CsvFileCreateOperation), "ScottLane.DataTidy.File.Resources.CsvFileCreateOperation.png")]
    public class CsvFileCreateOperation : FileOperation, IExecutable
    {
        private const string DEFAULT_DELIMITER = ",";
        private const bool DEFAULT_EXCEL_LEADING_ZEROES = false;
        private const HeadingText DEFAULT_HEADING_TEXT = HeadingText.DisplayName;
        private const int PROGRESS_REPORTING_INTERVAL = 100;

        #region Properties

        private string delimeter = DEFAULT_DELIMITER;

        /// <summary>
        /// Gets or sets the field delimiter.
        /// </summary>
        [GlobalisedCategory(typeof(CsvFileCreateOperation), nameof(Delimiter)), GlobalisedDisplayName(typeof(CsvFileCreateOperation), nameof(Delimiter)), GlobalisedDecription(typeof(CsvFileCreateOperation), nameof(Delimiter)), DefaultValue(DEFAULT_DELIMITER)]
        public string Delimiter
        {
            get { return delimeter; }
            set
            {
                if (delimeter != value)
                {
                    delimeter = value;
                    OnPropertyChanged(nameof(Delimiter));
                }
            }
        }

        private bool excelLeadingZeroes = DEFAULT_EXCEL_LEADING_ZEROES;

        [GlobalisedCategory(typeof(CsvFileCreateOperation), nameof(ExcelLeadingZeroes)), GlobalisedDisplayName(typeof(CsvFileCreateOperation), nameof(ExcelLeadingZeroes)), GlobalisedDecription(typeof(CsvFileCreateOperation), nameof(ExcelLeadingZeroes)), DefaultValue(DEFAULT_EXCEL_LEADING_ZEROES)]
        public bool ExcelLeadingZeroes
        {
            get { return excelLeadingZeroes; }
            set
            {
                if (excelLeadingZeroes != value)
                {
                    excelLeadingZeroes = value;
                    OnPropertyChanged(nameof(ExcelLeadingZeroes));
                }
            }
        }

        private HeadingText headingText = DEFAULT_HEADING_TEXT;

        [GlobalisedCategory("CSV File"), GlobalisedDisplayName("Heading Text"), GlobalisedDecription("Determines what text is used for each field heading."), DefaultValue(DEFAULT_HEADING_TEXT), Browsable(true)]
        public HeadingText HeadingText
        {
            get { return headingText; }
            set
            {
                if (headingText != value)
                {
                    headingText = value;
                    OnPropertyChanged(nameof(HeadingText));
                }
            }
        }

        /// <summary>
        /// The output file path.
        /// </summary>
        [GlobalisedCategory(typeof(CsvFileCreateOperation), nameof(OutputPath)), GlobalisedDisplayName(typeof(CsvFileCreateOperation), nameof(OutputPath)), GlobalisedDecription(typeof(CsvFileCreateOperation), nameof(OutputPath)), Editor(typeof(CsvSaveFileNameEditor), typeof(UITypeEditor))]
        public override string OutputPath
        {
            get { return base.OutputPath; }
            set { base.OutputPath = value; }
        }

        /// <summary>
        /// The data source.
        /// </summary>
        [GlobalisedCategory(typeof(CsvFileCreateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(CsvFileCreateOperation), nameof(DataSource)), GlobalisedDecription(typeof(CsvFileCreateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter))]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the CsvFileCreateOperation class with the specified batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public CsvFileCreateOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Validates the CsvFileCreateOperation settings.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(Delimiter == default(string), Properties.Resources.CsvFileCreateOperationValidateDelimiter, nameof(Delimiter));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.CsvFileCreateOperationFriendlyName, Path.GetFileName(OutputPath) ?? Properties.Resources.CsvFileCreateOperationFriendlyNamePath, DataSource?.Name ?? Properties.Resources.CsvFileCreateOperationFriendlyNameDataSource);
        }

        /// <summary>
        /// Creates a CSV file.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        public override void Execute(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            OnExecuting(new ExecutableEventArgs(this));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.CsvFileCreateOperationExecute, Name)));

            int rowIndex = 0;
            int lastReportRowIndex = rowIndex;
            DataTable dataTable = default(DataTable);

            try
            {
                dataTable = DataSource.GetDataTable(cancel, progress);
                progress?.Report(new ExecutionProgress(ExecutionStage.Transform, dataTable.Rows.Count, dataTable.Rows.Count));
                cancel.ThrowIfCancellationRequested();

                progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.CsvFileCreateOperationExecuteCreating, OutputPath)));

                using (StreamWriter fileWriter = new StreamWriter(OutputPath))
                {
                    CsvWriter csvWriter = new CsvWriter(fileWriter);
                    csvWriter.Configuration.Delimiter = Delimiter;
                    csvWriter.Configuration.UseExcelLeadingZerosFormatForNumerics = ExcelLeadingZeroes;

                    // write headers
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        csvWriter.WriteField(headingText == HeadingText.DisplayName ? column.Caption : column.ColumnName);
                    }

                    csvWriter.NextRecord();

                    // write records
                    while (rowIndex < dataTable.Rows.Count)
                    {
                        for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
                        {
                            csvWriter.WriteField(dataTable.Rows[rowIndex][columnIndex]);
                        }

                        csvWriter.NextRecord();
                        rowIndex++;

                        if (rowIndex - lastReportRowIndex >= PROGRESS_REPORTING_INTERVAL)
                        {
                            progress?.Report(new ExecutionProgress(ExecutionStage.Load, rowIndex, dataTable.Rows.Count));
                            lastReportRowIndex = rowIndex;
                        }

                        cancel.ThrowIfCancellationRequested();
                    }
                }
            }
            finally
            {
                progress?.Report(new ExecutionProgress(ExecutionStage.Load, rowIndex, dataTable?.Rows.Count ?? 0));
            }

            //progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.CsvFileCreateOperationExecuteSuccessful, OutputPath)));

            OnExecuted(new ExecutableEventArgs(this));
        }
    }

    /// <summary>
    /// Describes what text is displayed in the header row.
    /// </summary>
    public enum HeadingText
    {
        DisplayName,
        Identifier,
    }
}
