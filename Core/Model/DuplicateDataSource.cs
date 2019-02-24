using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    [DataSource(typeof(DuplicateDataSource), "ScottLane.DataTidy.Core.Resources.DuplicateDataSource.png", typeof(UtilityConnection))]
    public class DuplicateDataSource : DataSource, IDataSourcesProvider, IDataSourceFieldsProvider
    {
        #region Properties

        private IDataSource dataSource;

        /// <summary>
        /// Gets or sets the data source to find duplicates in.
        /// </summary>
        [GlobalisedCategory(typeof(DuplicateDataSource), nameof(DataSource)), GlobalisedDisplayName(typeof(DuplicateDataSource), nameof(DataSource)), GlobalisedDecription(typeof(DuplicateDataSource), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(IsReference = true)]
        public IDataSource DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    OnPropertyChanged(nameof(DataSource));
                }
            }
        }

        private BindingList<DataTableField> keyFields = new BindingList<DataTableField>();

        /// <summary>
        /// Gets or sets mappings between data source primary keys.
        /// </summary>
        [GlobalisedCategory(typeof(DuplicateDataSource), nameof(KeyFields)), GlobalisedDisplayName(typeof(DuplicateDataSource), nameof(KeyFields)), GlobalisedDecription(typeof(DuplicateDataSource), nameof(KeyFields)), TypeConverter(typeof(DataTableFieldFieldListConverter)), Editor(typeof(DataTableFieldCheckedListBoxEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public BindingList<DataTableField> KeyFields
        {
            get { return keyFields; }
            set
            {
                if (keyFields != value)
                {
                    keyFields = value;
                    OnPropertyChanged(nameof(KeyFields));

                    if (keyFields != null)
                    {
                        keyFields.ListChanged += KeyFields_ListChanged;
                    }
                }
            }
        }

        #endregion  

        public DuplicateDataSource(IConnection parent) : base(parent)
        { }

        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            // todo - this method needs progres reporting
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(DataSource.GetDataTable(cancel, progress).Copy());
            DataTable sourceCopy = dataSet.Tables[0].Copy();
            sourceCopy.TableName = string.Concat(sourceCopy.TableName, "-2");
            dataSet.Tables.Add(sourceCopy);

            DataTable output = sourceCopy.Copy();
            output.Rows.Clear();

            // create relations
            DataColumn[] sourceColumns = new DataColumn[KeyFields.Count];
            DataColumn[] sourceCopyColumns = new DataColumn[KeyFields.Count];

            for (int fieldIndex = 0; fieldIndex < KeyFields.Count; fieldIndex++)
            {
                sourceColumns[fieldIndex] = dataSet.Tables[0].Columns[KeyFields[fieldIndex].ColumnName];
                sourceCopyColumns[fieldIndex] = dataSet.Tables[1].Columns[KeyFields[fieldIndex].ColumnName];
            }

            DataRelation relation = new DataRelation("Relationship", sourceColumns, sourceCopyColumns, false);
            dataSet.Relations.Add(relation);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                if (row.GetChildRows(relation).Length > 1)
                {
                    output.ImportRow(row);
                }
            }

            return output;
        }

        /// <summary>
        /// Validates the data source.
        /// </summary>
        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(DataSource == default(IDataSource), Properties.Resources.DuplicateDataSourceValidateDataSource, nameof(DataSource));
                result.AddErrorIf(keyFields.Count == 0, Properties.Resources.DuplicateDataSourceValidateKeyFields, nameof(KeyFields));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.DuplicateDataSourceFriendlyName, DataSource?.Name ?? Properties.Resources.DuplicateDataSourceFriendlyNameDataSource);
        }

        public override IDataSource Clone(bool addSuffix)
        {
            DuplicateDataSource clone = (DuplicateDataSource)base.Clone(addSuffix);
            clone.KeyFields = new BindingList<DataTableField>(KeyFields.Select(keyField => keyField.Clone()).ToList());

            return clone;
        }

        public override DataColumnCollection GetDataColumns()
        {
            throw new NotImplementedException();
        }

        private void KeyFields_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(KeyFields));
        }

        #region Interface Methods

        // todo - why binding list here?
        public BindingList<IDataSource> GetDataSources()
        {
            BindingList<IDataSource> dataSources = new BindingList<IDataSource>();

            try
            {
                dataSources = ((IDataSourcesProvider)Parent.Parent).GetDataSources();

                foreach (IDataSource dataSource in dataSources)
                {
                    if (dataSource.ID == ID)
                    {
                        dataSources.Remove(dataSource);
                    }
                }
            }
            catch { }

            return dataSources;
        }

        public List<DataTableField> GetDataSourceFields()
        {
            List<DataTableField> fields = new List<DataTableField>();

            try
            {
                fields.AddRange(DataTableField.GetDataTableFields(DataSource?.GetDataColumns()));
            }
            catch { }

            return fields;
        }

#endregion
    }
}
