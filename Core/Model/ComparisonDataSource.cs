using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A data source that outputs the results of a comparison between two data sources.
    /// </summary>
    [DataSource(typeof(ComparisonDataSource), "ScottLane.DataTidy.Core.Resources.ComparisonDataSource.png", typeof(UtilityConnection))]
    public class ComparisonDataSource : DataSource, IDataSourcesProvider, IDataSourceFieldsProvider, ISecondaryDataSourceFieldsProvider, IFieldMappingCreator
    {
        private const ComparisonOutput DEFAULT_OUTPUT = ComparisonOutput.All;
        private readonly string SOURCE_COLUMN_NAME = Properties.Resources.ComparisonDataSourceColumnNameSource;
        private readonly string COMPARISON_COLUMN_NAME = Properties.Resources.ComparisonDataSourceColumnNameComparison;
        private readonly string RESULT_COLUMN_NAME = Properties.Resources.ComparisonDataSourceColumnNameResult;
        private readonly string DIFFERENCE_FIELDS_COLUMN_NAME = Properties.Resources.ComparisonDataSourceColumnNameDifferences;

        #region Properties

        private IDataSource sourceData;

        /// <summary>
        /// Gets or sets the data source to compare.
        /// </summary>
        [GlobalisedCategory(typeof(ComparisonDataSource), nameof(SourceData)), GlobalisedDisplayName(typeof(ComparisonDataSource), nameof(SourceData)), GlobalisedDecription(typeof(ComparisonDataSource), nameof(SourceData)), TypeConverter(typeof(DataSourceConverter)), Browsable(true), JsonProperty(IsReference = true)]
        public IDataSource SourceData
        {
            get { return sourceData; }
            set
            {
                if (sourceData != value)
                {
                    sourceData = value;
                    OnPropertyChanged(nameof(SourceData));
                }
            }
        }

        private IDataSource comparisonData;

        /// <summary>
        /// Gets or sets the data source to compare against.
        /// </summary>
        [GlobalisedCategory(typeof(ComparisonDataSource), nameof(ComparisonData)), GlobalisedDisplayName(typeof(ComparisonDataSource), nameof(ComparisonData)), GlobalisedDecription(typeof(ComparisonDataSource), nameof(ComparisonData)), TypeConverter(typeof(DataSourceConverter)), Browsable(true), JsonProperty(IsReference = true)]
        public IDataSource ComparisonData
        {
            get { return comparisonData; }
            set
            {
                if (comparisonData != value)
                {
                    comparisonData = value;
                    OnPropertyChanged(nameof(ComparisonData));
                }
            }
        }

        private BindingList<FieldMapping> keyMappings = new BindingList<FieldMapping>();

        /// <summary>
        /// Gets or sets mappings between data source primary keys.
        /// </summary>
        [GlobalisedCategory(typeof(ComparisonDataSource), nameof(KeyMappings)), GlobalisedDisplayName(typeof(ComparisonDataSource), nameof(KeyMappings)), GlobalisedDecription(typeof(ComparisonDataSource), nameof(KeyMappings)), Editor(typeof(ColumnMappingCollectionEditor), typeof(UITypeEditor)), Browsable(true)]
        public BindingList<FieldMapping> KeyMappings
        {
            get { return keyMappings; }
            set
            {
                if (keyMappings != value)
                {
                    keyMappings = value;
                    keyMappings.ListChanged += KeyMappings_ListChanged;
                    OnPropertyChanged(nameof(KeyMappings));
                }
            }
        }

        private BindingList<FieldMapping> fieldMappings = new BindingList<FieldMapping>();

        /// <summary>
        /// Gets or sets mappings between data source fields to compare.
        /// </summary>
        [GlobalisedCategory(typeof(ComparisonDataSource), nameof(FieldMappings)), GlobalisedDisplayName(typeof(ComparisonDataSource), nameof(FieldMappings)), GlobalisedDecription(typeof(ComparisonDataSource), nameof(FieldMappings)), Editor(typeof(ColumnMappingCollectionEditor), typeof(UITypeEditor)), Browsable(true)]
        public BindingList<FieldMapping> FieldMappings
        {
            get { return fieldMappings; }
            set
            {
                if (fieldMappings != value)
                {
                    fieldMappings = value;
                    fieldMappings.ListChanged += FieldMappings_ListChanged;
                    OnPropertyChanged(nameof(FieldMappings));
                }
            }
        }

        /// <summary>
        /// Gets or sets the comparison results to output.
        /// </summary>
        [GlobalisedCategory(typeof(ComparisonDataSource), nameof(Output)), GlobalisedDisplayName(typeof(ComparisonDataSource), nameof(Output)), GlobalisedDecription(typeof(ComparisonDataSource), nameof(Output)), DefaultValue(DEFAULT_OUTPUT), Browsable(true)]
        public ComparisonOutput Output { get; set; } = DEFAULT_OUTPUT;

        #endregion

        /// <summary>
        /// Initialises a new instance of the ComparisonDataSource class with the specified parent connection.
        /// </summary>
        /// <param name="parent">The parent connection.</param>
        public ComparisonDataSource(IConnection parent) : base(parent)
        { }

        /// <summary>
        /// Bubbles KeyMappings collection changes.
        /// </summary>
        private void KeyMappings_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(KeyMappings));
        }

        /// <summary>
        /// Bubbles FieldMappings collection changes.
        /// </summary>
        private void FieldMappings_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FieldMappings));
        }

        /// <summary>
        /// Validates the data source.
        /// </summary>
        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(SourceData == default(IDataSource), Properties.Resources.ComparisonDataSourceValidateSourceData, nameof(SourceData));
                result.AddErrorIf(ComparisonData == default(IDataSource), Properties.Resources.ComparisonDataSourceValidateComparisonData);
                result.AddErrorIf(KeyMappings.Count == 0, Properties.Resources.ComparisonDataSourceValidateKeyMappings);
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Generates a friendly name for the data source.
        /// </summary>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.ComparisonDataSourceFriendlyName, SourceData?.Name ?? Properties.Resources.ComparisonDataSourceFriendlyNameSourceData, ComparisonData?.Name ?? Properties.Resources.ComparisonDataSourceFriendlyNameComparisonData);
        }

        /// <summary>
        /// Updates the parent references of the data source and all child objects.
        /// </summary>
        public override void UpdateParentReferences(IConnection parentConnection)
        {
            base.UpdateParentReferences(parentConnection);
            KeyMappings.ToList().ForEach(keyMapping => keyMapping.UpdateParentReferences(this));
            FieldMappings.ToList().ForEach(fieldMapping => fieldMapping.UpdateParentReferences(this));
        }

        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetDataTable(cancel, progress, int.MaxValue);
        }

        private DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit)
        {
            DataTable source = SourceData.GetDataTable(cancel, progress);
            DataTable comparison = ComparisonData.GetDataTable(cancel, progress);
            DataTable output = CreateOutputTable(source, comparison);

            progress?.Report(new ExecutionProgress(NotificationType.Information, Properties.Resources.ComparisonDataSourceComparingData));
            progress?.Report(new ExecutionProgress(ExecutionStage.Transform, output.Rows.Count, source.Rows.Count + comparison.Rows.Count));

            // combine all data tables
            output.Merge(source, false, MissingSchemaAction.Ignore);
            comparison.PrimaryKey = null;   // todo - unable to do this in child method?
            output.Merge(comparison, false, MissingSchemaAction.Ignore);

            foreach (DataRow row in output.Rows)
            {
                ComparisonResult comparisonResult = ComparisonResult.Equal;
                string differenceFields = string.Empty;

                foreach (FieldMapping mapping in FieldMappings)
                {
                    if (!Equals(row[mapping.SourceField.ColumnName], row[mapping.ComparisonField.ColumnName]))    // todo - column name clashes, etc
                    {
                        comparisonResult = ComparisonResult.Different;
                        differenceFields += differenceFields.Length > 0 ? string.Format(", {0}", mapping.SourceField.ColumnName) : mapping.SourceField.ColumnName;
                    }
                }

                row[RESULT_COLUMN_NAME] = comparisonResult.ToString("g");
                row[DIFFERENCE_FIELDS_COLUMN_NAME] = !string.IsNullOrEmpty(differenceFields) ? differenceFields : default(string);
            }

            progress?.Report(new ExecutionProgress(ExecutionStage.Transform, output.Rows.Count, output.Rows.Count));

            return GetFilteredOutputTable(output);
        }

        /// <summary>
        /// Creates the structure of the output table by combining the source and comparison table structures, fields to compare and key fields.
        /// Also updates the name of the source and comparison table fields to support the DataTable.Merge method.
        /// </summary>
        /// <param name="source">The source table.</param>
        /// <param name="comparison">The comparison table.</param>
        /// <returns>The output table.</returns>
        public DataTable CreateOutputTable(DataTable source, DataTable comparison)
        {
            DataTable output = new DataTable();
            DataColumn[] outputKeys = new DataColumn[KeyMappings.Count];
            DataColumn[] comparisonKeys = new DataColumn[KeyMappings.Count];
            int keyIndex = 0;

            foreach (FieldMapping keyMapping in KeyMappings)
            {
                // create primary key columns in table
                DataColumn keyColumn = new DataColumn(keyMapping.SourceField.ColumnName, keyMapping.SourceField.DataType)
                {
                    AllowDBNull = false,
                    Unique = KeyMappings.Count == 1
                };
                outputKeys[keyIndex] = keyColumn;
                output.Columns.Add(keyColumn);

                // adjust the comparison table structure in prepration for the merge
                comparison.Columns[keyMapping.ComparisonField.ColumnName].ColumnName = keyMapping.SourceField.ColumnName;
                comparisonKeys[keyIndex] = comparison.Columns[KeyMappings[keyIndex].SourceField.ColumnName];
            }

            output.PrimaryKey = outputKeys;
            comparison.PrimaryKey = comparisonKeys;

            foreach (FieldMapping fieldMapping in FieldMappings)
            {
                //string uniqueName = GetUniqueColumnName(output, fieldMapping.SourceField.ColumnName, 1);

                // create columns in output table for columns being compared
                output.Columns.Add(new DataColumn(fieldMapping.SourceField.ColumnName, fieldMapping.SourceField.DataType)
                {
                    AllowDBNull = true,
                    Unique = false
                });

                //source.Columns[fieldMapping.SourceField.ColumnName].ColumnName = uniqueName;
                string uniqueName = GetUniqueColumnName(output, fieldMapping.ComparisonField.ColumnName, 1);

                output.Columns.Add(new DataColumn(uniqueName, fieldMapping.ComparisonField.DataType)
                {
                    AllowDBNull = true,
                    Unique = false
                });

                comparison.Columns[fieldMapping.ComparisonField.ColumnName].ColumnName = uniqueName;
            }

            // create columns to store record origins
            source.Columns.Add(new DataColumn(SOURCE_COLUMN_NAME, typeof(bool), true.ToString()));
            output.Columns.Add(new DataColumn(SOURCE_COLUMN_NAME, typeof(bool)) { DefaultValue = false });
            comparison.Columns.Add(new DataColumn(COMPARISON_COLUMN_NAME, typeof(bool), true.ToString()));
            output.Columns.Add(new DataColumn(COMPARISON_COLUMN_NAME, typeof(bool)) { DefaultValue = false });

            // create columns to store comparison results
            output.Columns.Add(new DataColumn(RESULT_COLUMN_NAME, typeof(string)));
            output.Columns.Add(new DataColumn(DIFFERENCE_FIELDS_COLUMN_NAME, typeof(string)));

            return output;
        }

        private string GetUniqueColumnName(DataTable dataTable, string columnName, int attempt)
        {
            string candidateName = attempt == 1 ? columnName : string.Format("{0} ({1})", columnName, attempt);

            if (!dataTable.Columns.Contains(candidateName))
            {
                return candidateName;
            }

            return GetUniqueColumnName(dataTable, columnName, ++attempt);
        }

        private DataTable GetFilteredOutputTable(DataTable table)
        {
            DataView filteredView = new DataView(table);

            if (Output == ComparisonOutput.Differences)
            {
                filteredView.RowFilter = string.Format("{0}='{1}'", RESULT_COLUMN_NAME, ComparisonResult.Different.ToString("g"));
            }
            else if (Output == ComparisonOutput.Equal)
            {
                filteredView.RowFilter = string.Format("{0}='{1}'", RESULT_COLUMN_NAME, ComparisonResult.Equal.ToString("g"));
            }
            else if (Output == ComparisonOutput.SourceOnly)
            {
                filteredView.RowFilter = string.Format("{0}={1} AND {2}={3}", SOURCE_COLUMN_NAME, true.ToString(), COMPARISON_COLUMN_NAME, false.ToString());
            }
            else if (Output == ComparisonOutput.ComparisonOnly)
            {
                filteredView.RowFilter = string.Format("{0}={1} AND {2}={3}", COMPARISON_COLUMN_NAME, true.ToString(), SOURCE_COLUMN_NAME, false.ToString());
            }

            return filteredView.ToTable();
        }

        public override DataColumnCollection GetDataColumns()
        {
            // todo - this could be done if we use the data columns from other data sources
            throw new NotImplementedException();
        }

        #region Interfaces

        public List<DataTableField> GetDataSourceFields()
        {
            List<DataTableField> fields = new List<DataTableField>();

            try
            {
                fields.AddRange(DataTableField.GetDataTableFields(SourceData?.GetDataColumns()));
            }
            catch { }

            return fields;
        }

        public List<DataTableField> GetSecondaryDataSourceFields()
        {
            List<DataTableField> fields = new List<DataTableField>();

            try
            {
                fields.AddRange(DataTableField.GetDataTableFields(ComparisonData?.GetDataColumns()));
            }
            catch { }

            return fields;
        }

        public BindingList<IDataSource> GetDataSources()
        {
            BindingList<IDataSource> dataSources = ((IDataSourcesProvider)Parent.Parent).GetDataSources();

            foreach (IDataSource dataSource in dataSources)
            {
                if (dataSource.ID == ID)
                {
                    dataSources.Remove(dataSource);
                    return dataSources;
                }
            }

            return dataSources;
        }

        public FieldMapping CreateFieldMapping(Type type)
        {
            return (FieldMapping)Activator.CreateInstance(type, new object[] { this });
        }

        #endregion
    }

    /// <summary>
    /// Specifies the types of output that can be returned by the ComparisonDataSource.
    /// </summary>
    public enum ComparisonOutput
    {
        All,
        Equal,
        Differences,
        SourceOnly,
        ComparisonOnly
    }

    /// <summary>
    /// Specifies the outcomes of a row comparison.
    /// </summary>
    public enum ComparisonResult
    {
        Equal,
        Different
    }
}
