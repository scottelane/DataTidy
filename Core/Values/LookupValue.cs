using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Gets a value by looking up the value from a data source with a selection criteria.
    /// </summary>
    public class LookupValue : FieldValue, IDataSourcesProvider, ILookupCriteriaCreator, IFieldValueCreator, IDataSourceFieldsProvider, ILookupSourceFieldsProvider
    {
        private const CachingBehaviour DEFAULT_CACHING_BEHAVIOUR = CachingBehaviour.DataSource;
        private const DuplicateBehaviour DEFAULT_DUPLICATE_BEHAVIOUR = DuplicateBehaviour.Error;
        private const NotFoundBehaviour DEFAULT_NOT_FOUND_BEHAVIOUR = NotFoundBehaviour.Error;
        private const ColumnOrder DEFAULT_COLUMN_ORDER = ColumnOrder.Ascending;

        #region Properties

        protected IDataSource dataSource;  // todo - should not be local to entire class - what if params change?

        [GlobalisedCategory("Lookup"), GlobalisedDisplayName("Data Source"), GlobalisedDecription("The data source to look up the value from."), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 1)]
        public virtual IDataSource DataSource
        {
            get { return dataSource; }
            set
            {
                if (value != dataSource)    // todo - comparer
                {
                    dataSource = value;
                    RefreshBrowsableFields();
                    ValueField = default(DataTableField);  // todo - more general events for property changes?
                }
            }
        }

        protected DataTableField valueField;

        [GlobalisedCategory("Lookup"), GlobalisedDisplayName("Value Field"), GlobalisedDecription("The field in the data source that contains the value to look up."), TypeConverter(typeof(LookupSourceFieldConverter)), JsonProperty(Order = 2)]
        public virtual DataTableField ValueField
        {
            get { return valueField; }
            set
            {
                if (valueField != value)
                {
                    valueField = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(ValueField));
                }
            }
        }

        protected BindingList<LookupCriteria> lookupCriteria = new BindingList<LookupCriteria>();

        [GlobalisedCategory("Lookup"), GlobalisedDisplayName("Selection Criteria"), GlobalisedDecription("The selection criteria to find the data source record to look up."), Editor(typeof(LookupCriteriaCollectionEditor), typeof(UITypeEditor))]
        public virtual BindingList<LookupCriteria> LookupCriteria
        {
            get { return lookupCriteria; }
            set
            {
                if (lookupCriteria != value)
                {
                    lookupCriteria = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(LookupCriteria));
                }
            }
        }

        protected DataTableField orderByField;

        [GlobalisedCategory("Ordering"), GlobalisedDisplayName("Order By"), GlobalisedDecription("The field to order results by if multiple matching records are returned."), TypeConverter(typeof(LookupSourceFieldConverter))]
        public virtual DataTableField OrderByField
        {
            get { return orderByField; }
            set
            {
                if (orderByField != value)
                {
                    orderByField = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(OrderByField));
                }
            }
        }

        protected ColumnOrder columnOrder = DEFAULT_COLUMN_ORDER;

        [GlobalisedCategory("Ordering"), GlobalisedDisplayName("Column Order"), GlobalisedDecription("The column order."), DefaultValue(DEFAULT_COLUMN_ORDER)]
        public virtual ColumnOrder ColumnOrder
        {
            get { return columnOrder; }
            set
            {
                if (columnOrder != value)
                {
                    columnOrder = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(ColumnOrder));
                }
            }
        }

        protected CachingBehaviour cachingBehaviour = DEFAULT_CACHING_BEHAVIOUR;

        // todo - need a way to override this in derived classes
        [GlobalisedCategory("Behaviour"), GlobalisedDisplayName("Caching"), GlobalisedDecription("Indicates whether the entire data source is cached or no caching is performed."), DefaultValue(DEFAULT_CACHING_BEHAVIOUR)]
        public virtual CachingBehaviour CachingBehaviour
        {
            get { return cachingBehaviour; }
            set
            {
                if (cachingBehaviour != value)
                {
                    cachingBehaviour = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(CachingBehaviour));
                }
            }
        }

        protected DuplicateBehaviour duplicateBehaviour = DEFAULT_DUPLICATE_BEHAVIOUR;

        [GlobalisedCategory("Behaviour"), GlobalisedDisplayName("Duplicates"), GlobalisedDecription("Indicates whether finding multiple lookup values should error, match the first record as determined by Order By or ignore all found values."), DefaultValue(DEFAULT_DUPLICATE_BEHAVIOUR)]
        public virtual DuplicateBehaviour DuplicateBehaviour
        {
            get { return duplicateBehaviour; }
            set
            {
                if (duplicateBehaviour != value)
                {
                    duplicateBehaviour = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(DuplicateBehaviour));
                }
            }
        }

        protected NotFoundBehaviour notFoundBehaviour = DEFAULT_NOT_FOUND_BEHAVIOUR;

        // todo - subclass this into class that only uses it for field mappings?
        [GlobalisedCategory("Behaviour"), GlobalisedDisplayName("Not Found"), GlobalisedDecription("Indicates whether missing lookup values should raise an error or be ignored."), DefaultValue(DEFAULT_NOT_FOUND_BEHAVIOUR)]
        public virtual NotFoundBehaviour NotFoundBehaviour
        {
            get { return notFoundBehaviour; }
            set
            {
                if (notFoundBehaviour != value)
                {
                    notFoundBehaviour = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(NotFoundBehaviour));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the LookupValue class with the specified parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        [JsonConstructor]
        public LookupValue(object parent) : base(parent)
        { }

        /// <summary>
        /// Gets a value by looking up the value from a data source with a selection criteria.
        /// </summary>
        /// <param name="record">The data source row.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The value.</returns>
        public override object GetValue(DataRow record, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            object value = default(object);

            if (CachingBehaviour == CachingBehaviour.DataSource)
            {
                value = GetCachedDataSourceValue(record, cancel, default(IProgress<ExecutionProgress>));
            }
            else if (CachingBehaviour == CachingBehaviour.None)
            {
                value = GetDataSourceValue(record, cancel, default(IProgress<ExecutionProgress>));
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="cancel"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        /// <remarks>http://www.csharp-examples.net/dataview-rowfilter/</remarks>
        protected virtual object GetCachedDataSourceValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            DataTable dataTable = default(DataTable);

            if (Parent is IOperation operation)
            {
                OperationCache cache = new OperationCache(operation);
                string cacheKey = string.Format("LookupValue:{0}", dataSource.ID);
                dataTable = (DataTable)cache[cacheKey];

                if (dataTable == default(DataTable))
                {
                    dataTable = DataSource.GetDataTable(cancel, progress);
                    cache[cacheKey] = dataTable;
                }
            }

            return GetDataSourceValue(dataTable, row, cancel, progress);
        }

        protected virtual object GetDataSourceValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return GetDataSourceValue(DataSource.GetDataTable(cancel, progress), row, cancel, progress);
        }

        protected virtual object GetDataSourceValue(DataTable table, DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            string filterExpression = string.Empty;

            for (int criteriaIndex = 0; criteriaIndex < LookupCriteria.Count; criteriaIndex++)
            {
                LookupCriteria lookupCriteria = LookupCriteria[criteriaIndex];
                object filterValue = lookupCriteria.GetValue(row, cancel, progress);

                if (filterValue == null)
                {
                    return null;    // todo -  we can't look up a null value as part of a lookup at this stage
                }

                if (filterValue is string)
                {
                    filterValue = ((string)filterValue).Replace("'", "''");
                }

                if (filterValue is string || filterValue is Guid || filterValue is DateTime)
                {
                    filterValue = string.Concat("'", filterValue, "'");
                }

                filterExpression = string.Concat(filterExpression, string.Format("{0}{1}{2}{3}", criteriaIndex == 0 ? string.Empty : " AND ", lookupCriteria.SourceField.ColumnName, lookupCriteria.ComparisonOperator == ComparisonOperator.DoesNotEqual ? "<>" : "=", filterValue));
            }

            string queryDescription = string.Format("{0} records with {1}", DataSource.Name, filterExpression);
            string sort = string.Empty;

            if (OrderByField != default(DataTableField))
            {
                sort = string.Format("{0} {1}", OrderByField.ColumnName, ColumnOrder == ColumnOrder.Ascending ? "ASC" : "DESC");
            }

            object value = default(object);
            List<DataRow> rows = table.Select(filterExpression, sort).ToList(); // todo - could cause error if data table is null

            if (rows.Count == 1 || (rows.Count > 1 && DuplicateBehaviour == DuplicateBehaviour.First))
            {
                value = Convert(rows[0][ValueField.ColumnName]);
            }
            else if (rows.Count > 1 && DuplicateBehaviour == DuplicateBehaviour.Error)
            {
                throw new DuplicateLookupValueException(string.Format("Matched {0} {1}", rows.Count, queryDescription), this);
            }
            else if (rows.Count == 0 && NotFoundBehaviour == NotFoundBehaviour.Error)
            {
                throw new MissingLookupValueException(string.Format("No {0} could be found", queryDescription), this);
            }

            return value;
        }

        public override void UpdateParentReferences(object parent)
        {
            base.UpdateParentReferences(parent);

            foreach (LookupCriteria criteria in LookupCriteria)
            {
                criteria.UpdateParentReferences(this);
            }
        }

        public override FieldValue Clone()
        {
            LookupValue clone = (LookupValue)base.Clone();
            clone.LookupCriteria = new BindingList<Core.LookupCriteria>();

            foreach (LookupCriteria criteria in LookupCriteria)
            {
                clone.LookupCriteria.Add(criteria.Clone());
            }

            return clone;
        }

        protected virtual object Convert(object value)
        {
            return value;
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            result.AddErrorIf(DataSource == default(IDataSource), "Please select a Data Source", nameof(DataSource));
            result.AddErrorIf(ValueField == default(DataTableField), "Please select a Value Field", nameof(ValueField));
            result.AddErrorIf(LookupCriteria == default(BindingList<LookupCriteria>) || LookupCriteria.Count == 0, "Please add at least one Selection Criteria", nameof(LookupCriteria));
            return result;
        }

        #region Interface Methods

        public BindingList<IDataSource> GetDataSources()
        {
            return ((IDataSourcesProvider)Parent).GetDataSources();
        }

        public override List<DataTableField> GetDataSourceFields()
        {
            return ((IDataSourceFieldsProvider)Parent).GetDataSourceFields();
        }

        public List<DataTableField> GetLookupSourceFields()
        {
            return DataTableField.GetDataTableFields(DataSource?.GetDataColumns());
        }

        public FieldValue CreateFieldValue(Type type)
        {
            return (FieldValue)Activator.CreateInstance(type, new object[] { this });
        }

        public LookupCriteria CreateLookupCriteria(Type type)
        {
            return (LookupCriteria)Activator.CreateInstance(type, new object[] { this });
        }

        #endregion
    }

    public enum CachingBehaviour
    {
        DataSource,
        None
    }

    public enum DuplicateBehaviour
    {
        Error,
        First,
        Ignore
    }

    public enum NotFoundBehaviour
    {
        Error,
        Ignore
    }

    public enum ColumnOrder
    {
        Ascending,
        Descending
    }
}