using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Threading;
using System.Windows.Forms.Design;
using ScottLane.DataTidy.Core;
using CsvHelper;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Provides the ability to load records from a CSV file.
    /// </summary>
    [DataSource(typeof(CsvDataSource), "ScottLane.DataTidy.File.Resources.CsvDataSource.png", typeof(FileSystemConnection))]
    public class CsvDataSource : FileDataSource, IRecordCountProvider, ISampleDataProvider
    {
        private const string DEFAULT_DELIMITER = ",";
        private const MissingValueBehaviour DEFAULT_MISSING_VALUES = MissingValueBehaviour.Null;
        private const int PROGRESS_REPORTING_INTERVAL = 100;
        private const int ALL_RECORDS_LIMIT = -1;

        #region Properties

        private string delimiter = DEFAULT_DELIMITER;

        /// <summary>
        /// Gets or sets the field delimiter.
        /// </summary>
        [GlobalisedCategory(typeof(CsvDataSource), nameof(Delimiter)), GlobalisedDisplayName(typeof(CsvDataSource), nameof(Delimiter)), GlobalisedDecription(typeof(CsvDataSource), nameof(Delimiter))]
        public string Delimiter
        {
            get { return delimiter; }
            set
            {
                if (delimiter != value)
                {
                    delimiter = value;
                    OnPropertyChanged(nameof(Delimiter));
                }
            }
        }

        [GlobalisedCategory(typeof(CsvDataSource), nameof(Path)), GlobalisedDisplayName(typeof(CsvDataSource), nameof(Path)), GlobalisedDecription(typeof(CsvDataSource), nameof(Path)), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), Browsable(true)]
        public override string Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        private MissingValueBehaviour missingValues = DEFAULT_MISSING_VALUES;

        [GlobalisedCategory(typeof(CsvDataSource), nameof(MissingValues)), GlobalisedDisplayName(typeof(CsvDataSource), nameof(MissingValues)), GlobalisedDecription(typeof(CsvDataSource), nameof(MissingValues)), DefaultValue(DEFAULT_MISSING_VALUES)]
        public MissingValueBehaviour MissingValues
        {
            get { return missingValues; }
            set
            {
                if (missingValues != value)
                {
                    missingValues = value;
                    OnPropertyChanged(nameof(MissingValues));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the CsvDataSource class with the specified connection.
        /// </summary>
        /// <param name="connection">The file connection.</param>
        public CsvDataSource(FileSystemConnection connection) : base(connection)
        { }

        protected override string GenerateFriendlyName()
        {
            return Path != default(string) ? System.IO.Path.GetFileNameWithoutExtension(Path) : Properties.Resources.CsvDataSourceFriendlyName;
        }

        /// <summary>
        /// Gets the file record count.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            int recordCount = 0;
            int lastReportRecordCount = 0;

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));

            using (FileStream stream = ((FileSystemConnection)Parent).GetFileStream(Path))
            {
                using (StreamReader fileReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = GetCsvReader(fileReader))
                    {
                        while (csvReader.Read())
                        {
                            recordCount++;

                            if (recordCount - lastReportRecordCount >= PROGRESS_REPORTING_INTERVAL)
                            {
                                progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));
                                lastReportRecordCount = recordCount;
                            }

                            cancel.ThrowIfCancellationRequested();
                        }
                    }
                }
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, recordCount, recordCount));

            return recordCount;
        }

        /// <summary>
        /// Gets a data table from the file.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The data table.</returns>
        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetDataTable(cancel, progress, ALL_RECORDS_LIMIT);
        }

        /// <summary>
        /// Gets a limited number of data source records as a DataTable.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="recordLimit">The number of records to return.</param>
        /// <returns>The data table.</returns>
        private DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));
            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.CsvDataSourceExtract, (Path))));

            int rowIndex = 0;
            int lastReportRowIndex = rowIndex;
            DataTable dataTable = default(DataTable);

            dataTable = CreateEmptyDataTable();

            using (FileStream stream = ((FileSystemConnection)Parent).GetFileStream(Path))
            {
                using (StreamReader fileReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = GetCsvReader(fileReader))
                    {
                        while (csvReader.Read() && (rowIndex < recordLimit || recordLimit == ALL_RECORDS_LIMIT))
                        {
                            DataRow row = dataTable.NewRow();

                            for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
                            {
                                string value = csvReader.GetField(columnIndex);

                                if (value == string.Empty && MissingValues == MissingValueBehaviour.Null)
                                {
                                    value = null;
                                }

                                row[columnIndex] = value;
                            }

                            dataTable.Rows.Add(row);
                            rowIndex++;

                            if (rowIndex - lastReportRowIndex >= PROGRESS_REPORTING_INTERVAL)
                            {
                                progress?.Report(new ExecutionProgress(ExecutionStage.Extract, rowIndex, dataTable.Rows.Count));
                                lastReportRowIndex = rowIndex;
                            }

                            cancel.ThrowIfCancellationRequested();
                        }
                    }
                }
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, rowIndex, dataTable?.Rows.Count ?? 0));

            return dataTable;
        }

        /// <summary>
        /// Creates an empty data table with columns from the file header.
        /// </summary>
        /// <returns>The data table.</returns>
        private DataTable CreateEmptyDataTable()
        {
            DataTable dataTable = new DataTable();
            string[] fieldHeaders = null;

            using (FileStream stream = ((FileSystemConnection)Parent).GetFileStream(Path))
            {
                using (StreamReader fileReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = GetCsvReader(fileReader))
                    {
                        csvReader.ReadHeader();
                        fieldHeaders = csvReader.FieldHeaders;
                    }
                }
            }

            foreach (string fieldHeader in fieldHeaders)
            {
                DataColumn column = new DataColumn(fieldHeader, typeof(string));
                dataTable.Columns.Add(column);
            }

            return dataTable;
        }

        /// <summary>
        /// Validates the CsvDataSource settings.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(Delimiter == default(string), Properties.Resources.CsvDataSourceValidateDelimiter);
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets a CsvReader from a TextReader. 
        /// </summary>
        /// <param name="textReader">The text reader.</param>
        /// <returns>The CsvReader.</returns>
        private CsvReader GetCsvReader(TextReader textReader)
        {
            CsvReader csvReader = new CsvReader(textReader);
            csvReader.Configuration.Delimiter = Delimiter;
            csvReader.Configuration.DetectColumnCountChanges = true;
            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.AllowComments = false;

            return csvReader;
        }

        /// <summary>
        /// Gets the columns available in the file.
        /// </summary>
        /// <returns>The columns.</returns>
        public override DataColumnCollection GetDataColumns()
        {
            return CreateEmptyDataTable().Columns;
        }

        public DataTable GetSampleData(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            return GetDataTable(cancel, progress, recordLimit);
        }
    }

    public enum MissingValueBehaviour
    {
        EmptyString,
        Null
    }
}

