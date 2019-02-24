using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Design;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Allows the selection criteria for a data source lookup to be captured.
    /// </summary>
    public class LookupValueCriteria : LookupCriteria, IDataSourcesProvider, ILookupCriteriaCreator, IFieldValueCreator
    {
        private const CachingBehaviour DEFAULT_CACHING = CachingBehaviour.DataSource;
        private const DuplicateBehaviour DEFAULT_DUPLICATE_BEHAVIOUR = DuplicateBehaviour.Error;
        private const NotFoundBehaviour DEFAULT_NOT_FOUND_BEHAVIOUR = NotFoundBehaviour.Error;

        protected DataTable dataTable;

        protected IDataSource dataSource;

        /// <summary>
        /// The data source to look up.
        /// </summary>
        [DisplayName("Data Source"), GlobalisedDecription("The data source to look up a value in."), TypeConverter(typeof(DataSourceConverter))]
        public virtual IDataSource DataSource
        {
            get { return dataSource; }
            set
            {
                if (value != dataSource)    // todo - comparer
                {
                    dataSource = value;
                    ValueField = default(string);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source value field to look up.
        /// </summary>
        [DisplayName("Value Field"), GlobalisedDecription("The data source field to look up the value from."), TypeConverter(typeof(DataSourceFieldConverter))]
        public virtual string ValueField { get; set; }

        [DisplayName("Selection Criteria"), GlobalisedDecription("The selection criteria to find the data source record to look up."), Editor(typeof(LookupCriteriaCollectionEditor), typeof(UITypeEditor))]
        public virtual List<LookupCriteria> LookupCriteria { get; set; } = new List<LookupCriteria>();

        // todo - need a way to override this in derived classes
        [DisplayName("Caching"), GlobalisedDecription("Indicates whether matched records will be cached to improve performance. Set to 'Cache' if lookup records are repeated across multiple records, 'CacheEntityLocal' to cache the entire entity locally or 'DoNotCache' to not cache."), DefaultValue(DEFAULT_CACHING)]
        public virtual CachingBehaviour CachingBehaviour { get; set; } = DEFAULT_CACHING;

        [DisplayName("Duplicate Behaviour"), GlobalisedDecription("Indicates whether finding a duplicate reference should error, match the last modified record or skip."), DefaultValue(DEFAULT_DUPLICATE_BEHAVIOUR)]
        public virtual DuplicateBehaviour DuplicateBehaviour { get; set; } = DEFAULT_DUPLICATE_BEHAVIOUR;

        // todo - subclass this into class that only uses it for field mappings?
        [DisplayName("Not Found Behaviour"), GlobalisedDecription("Indicates whether missing references raise an error or are ignored."), DefaultValue(DEFAULT_NOT_FOUND_BEHAVIOUR)]
        public virtual NotFoundBehaviour NotFoundBehaviour { get; set; } = DEFAULT_NOT_FOUND_BEHAVIOUR;

        public override string FilterPreview
        {
            get { return string.Format("{0} {1} '{2}'", SourceField != default(DataTableField) ? SourceField.DisplayName : "<Field>", ComparisonOperator == ComparisonOperator.Equals ? "=" : "<>", ValueField ?? "<Value Field>"); }
        }

        [JsonConstructor]
        public LookupValueCriteria(object parent) : base(parent)
        { }

        public override object GetValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            return null;    //todo - helper class
        }

        public BindingList<IDataSource> GetDataSources()
        {
            return ((IDataSourcesProvider)Parent).GetDataSources();
        }

        public LookupCriteria CreateLookupCriteria(Type type)
        {
            return (LookupCriteria)Activator.CreateInstance(type, new object[] { this });
        }

        public FieldValue CreateFieldValue(Type type)
        {
            throw new NotImplementedException();
        }
    }
}