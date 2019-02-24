using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Threading;
using System.Windows.Forms.Design;
using ScottLane.DataTidy.Core;
using ExcelDataReader;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Provides data from a Microsoftr Excel workbook.
    /// </summary>
    [DataSource(typeof(ExcelDataSource), "ScottLane.DataTidy.File.Resources.ExcelDataSource.png", typeof(FileSystemConnection))]
    public class ExcelDataSource : FileDataSource, ISheetsProvider, IRecordCountProvider, IDataSourceFieldsProvider, ISampleDataProvider
    {
        private const bool DEFAULT_FIRST_ROW_HEADERS = true;
        private const int ALL_RECORDS_LIMIT = -1;
        private const string EXTENSION_BINARY = ".xls";
        private const string EXTENSION_OPEN_XML = ".xlsx";

        #region Properties

        private bool firstRowHeaders = DEFAULT_FIRST_ROW_HEADERS;

        /// <summary>
        /// Gets or sets a value that indicates whether the first row of the worksheet contains headers.
        /// </summary>
        [GlobalisedCategory(typeof(ExcelDataSource), nameof(FirstRowHeaders)), GlobalisedDisplayName(typeof(ExcelDataSource), nameof(FirstRowHeaders)), GlobalisedDecription(typeof(ExcelDataSource), nameof(FirstRowHeaders)), DefaultValue(DEFAULT_FIRST_ROW_HEADERS), Browsable(true)]
        public bool FirstRowHeaders
        {
            get { return firstRowHeaders; }
            set
            {
                if (firstRowHeaders != value)
                {
                    firstRowHeaders = value;
                    OnPropertyChanged(nameof(FirstRowHeaders));
                }
            }
        }

        private string sheetName;

        /// <summary>
        /// Gets or sets the name of the worksheet to read from.
        /// </summary>
        [GlobalisedCategory(typeof(ExcelDataSource), nameof(SheetName)), GlobalisedDisplayName(typeof(ExcelDataSource), nameof(SheetName)), GlobalisedDecription(typeof(ExcelDataSource), nameof(SheetName)), TypeConverter(typeof(SheetTypeConverter)), Browsable(true)]
        public string SheetName
        {
            get { return sheetName; }
            set
            {
                if (sheetName != value)
                {
                    sheetName = value;
                    OnPropertyChanged(nameof(SheetName));
                    RefreshName();
                }
            }
        }

        [GlobalisedCategory(typeof(ExcelDataSource), nameof(Path)), GlobalisedDisplayName(typeof(ExcelDataSource), nameof(Path)), GlobalisedDecription(typeof(ExcelDataSource), nameof(Path)), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), Browsable(true)]
        public override string Path
        {
            get { return base.Path; }
            set { base.Path = value; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the ExcelDataSource class with the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ExcelDataSource(FileSystemConnection connection) : base(connection)
        { }

        /// <summary>
        /// Generates a friendly name for the data source.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return Path != default(string) && SheetName != default(String) ? string.Format("{0} {1}", System.IO.Path.GetFileNameWithoutExtension(Path), SheetName) : Path != default(string) ? System.IO.Path.GetFileNameWithoutExtension(Path) : SheetName != default(string) ? SheetName : Properties.Resources.ExcelDataSourceFriendlyName;
        }

        /// <summary>
        /// Gets a DataTable containing records from the Excel worksheet.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The DataTable.</returns>
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
            return GetDataTable(cancel, progress, recordLimit, true);
        }

        private DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit, bool reportTextProgress)
        {
            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, 0, 0));

            if (reportTextProgress)
            {
                progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.ExcelDataSourceExtract, Path)));
            }

            DataSet dataSet = GetExcelDataSet(recordLimit);

            progress?.Report(new ExecutionProgress(ExecutionStage.Extract, dataSet.Tables[SheetName].Rows.Count, dataSet.Tables[SheetName].Rows.Count));

            return dataSet.Tables[SheetName];
        }

        private DataSet GetExcelDataSet(int recordLimit)
        {
            DataSet dataSet = new DataSet();
            FileInfo excelFile = new FileInfo(Path);
            IExcelDataReader excelReader = GetExcelDataReader(excelFile);

            while (excelReader.Read() && dataSet.Tables.Count == 0)
            {
                if (excelReader.Name == SheetName)
                {
                    dataSet = excelReader.AsDataSet();

                    if (firstRowHeaders && dataSet.Tables[SheetName].Rows.Count >= 1)
                    {
                        for (int columnIndex = 0; columnIndex < dataSet.Tables[SheetName].Columns.Count; columnIndex++)
                        {
                            string columnName = dataSet.Tables[SheetName].Rows[0][columnIndex].ToString();

                            if (!dataSet.Tables[SheetName].Columns.Contains(columnName))
                            {
                                dataSet.Tables[SheetName].Columns[columnIndex].ColumnName = columnName;
                                dataSet.Tables[SheetName].Columns[columnIndex].Caption = columnName;
                            }
                            else
                            {
                                throw new ApplicationException(string.Format("Please remove the duplicate '{0}' column from the {1} worksheet", columnName, sheetName));
                            }
                        }

                        dataSet.Tables[SheetName].Rows[0].Delete();
                        dataSet.AcceptChanges();
                    }

                    for (int rowIndex = 0; rowIndex < dataSet.Tables[SheetName].Rows.Count && recordLimit != ALL_RECORDS_LIMIT; rowIndex++)
                    {
                        if (rowIndex >= recordLimit)
                        {
                            dataSet.Tables[SheetName].Rows[rowIndex].Delete();
                            dataSet.AcceptChanges();
                        }
                    }
                }

                excelReader.NextResult();
            }

            excelReader.Dispose();

            return dataSet;
        }

        /// <summary>
        /// Validates the data source.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                if (Path != default(string))
                {
                    FileInfo excelFile = new FileInfo(Path);
                    result.AddErrorIf(excelFile.Extension.ToLower() != EXTENSION_BINARY && excelFile.Extension.ToLower() != EXTENSION_OPEN_XML, Properties.Resources.ExcelDataSourceValidateFileExtension);
                    result.AddErrorIf(string.IsNullOrEmpty(SheetName), Properties.Resources.ExcelDataSourceValidateSheetName, nameof(SheetName));
                }
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets a list of worksheets within the Excel workbook.
        /// </summary>
        /// <returns>A list of sheets name.</returns>
        public List<string> GetSheets()
        {
            List<string> sheetNames = new List<string>();

            try
            {
                FileInfo excelFile = new FileInfo(Path);
                IExcelDataReader excelReader = GetExcelDataReader(excelFile);

                while (excelReader.Read())
                {
                    if (!sheetNames.Contains(excelReader.Name))
                    {
                        sheetNames.Add(excelReader.Name);
                    }

                    excelReader.NextResult();
                }

                excelReader.Dispose();
            }
            catch
            { }

            return sheetNames;
        }

        /// <summary>
        /// Gets an IExcelDataReader for the specified excelFile.
        /// </summary>
        /// <param name="excelFile">The Microsoft Excel file.</param>
        /// <returns>The IExcelDataReader.</returns>
        private IExcelDataReader GetExcelDataReader(FileInfo excelFile)
        {
            FileStream fileStream = excelFile.OpenRead();
            IExcelDataReader excelReader;

            if (excelFile.Extension.ToLower() == EXTENSION_BINARY)
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            }
            else if (excelFile.Extension.ToLower() == EXTENSION_OPEN_XML)
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            }
            else
            {
                throw new ArgumentException(Properties.Resources.ExcelDataSourceValidateFileExtension);
            }

            return excelReader;
        }

        /// <summary>
        /// Counts the number of records in the spreadsheet.
        /// </summary>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The record count.</returns>
        public int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetDataTable(cancel, progress, ALL_RECORDS_LIMIT, false).Rows.Count;
        }

        public override DataColumnCollection GetDataColumns()
        {
            ConnectionCache cache = new ConnectionCache(Parent);
            string cacheKey = string.Format("GetDataColumns:{0}", ID.ToString());
            DataColumnCollection columns = (DataColumnCollection)cache[cacheKey];

            if (columns == default(DataColumnCollection))
            {
                DataSet dataSet = GetExcelDataSet(0);
                columns = dataSet.Tables[SheetName].Columns;
                cache[cacheKey] = columns;

            }

            // todo - this fails when rendering in comparison data source
            return columns;
        }

        public List<DataTableField> GetDataSourceFields()
        {
            List<DataTableField> fields = new List<DataTableField>();
            try
            {
                fields.AddRange(DataTableField.GetDataTableFields(GetDataColumns()));
            }
            catch { }

            return fields;
        }

        public DataTable GetSampleData(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            return GetDataTable(cancel, progress, recordLimit);
        }
    }
}
