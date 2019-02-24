using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    [DataSource(typeof(FilterDataSource), "ScottLane.DataTidy.Core.Resources.FilterDataSource.png", typeof(UtilityConnection))]
    public class FilterDataSource : DataSource, IDataSourcesProvider, IDataSourceFieldsProvider, ILookupCriteriaCreator, ILookupSourceFieldsProvider
    {
        private IDataSource dataSource;

        /// <summary>
        /// Gets or sets the data source to compare.
        /// </summary>
        [GlobalisedCategory(typeof(FilterDataSource), nameof(DataSource)), GlobalisedDisplayName(typeof(FilterDataSource), nameof(DataSource)), GlobalisedDecription(typeof(FilterDataSource), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(IsReference = true)]
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

        private BindingList<LookupCriteria> filters = new BindingList<LookupCriteria>();

        /// <summary>
        /// Gets or sets mappings between data source fields to compare.
        /// </summary>
        [GlobalisedCategory(typeof(FilterDataSource), nameof(Filters)), GlobalisedDisplayName(typeof(FilterDataSource), nameof(Filters)), GlobalisedDecription(typeof(FilterDataSource), nameof(Filters)), Editor(typeof(LookupCriteriaCollectionEditor), typeof(UITypeEditor))]
        public BindingList<LookupCriteria> Filters
        {
            get { return filters; }
            set
            {
                if (filters != value)
                {
                    filters = value;
                    //filters.ListChanged += FieldMappings_ListChanged;
                    OnPropertyChanged(nameof(Filters));
                }
            }
        }

        public FilterDataSource(IConnection parentConnection) : base(parentConnection)
        { }

        public override DataColumnCollection GetDataColumns()
        {
            return DataSource?.GetDataColumns();
        }

        public override DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return DataSource?.GetDataTable(cancel, progress);
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(DataSource == default(IDataSource), "Please select a Data Source", nameof(DataSource));
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

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
            return DataTableField.GetDataTableFields(DataSource?.GetDataColumns());
        }

        public LookupCriteria CreateLookupCriteria(Type type)
        {
            return (LookupCriteria)Activator.CreateInstance(type, new object[] { this });
        }

        public List<DataTableField> GetLookupSourceFields()
        {
            return DataTableField.GetDataTableFields(DataSource?.GetDataColumns());
        }

    }
}
