using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class DataSourceValueCriteria : LookupCriteria
    {
        protected DataTableField valueField;

        /// <summary>
        /// The user-provided value.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Value Field"), GlobalisedDecription("The user-provided value to use."), TypeConverter(typeof(DataSourceFieldConverter))]
        public DataTableField ValueField
        {
            get { return valueField; }
            set
            {
                if (valueField != value)
                {
                    valueField = value;
                    OnPropertyChanged(nameof(ValueField));
                }
            }
        }

        public override string FilterPreview
        {
            get { return string.Format("{0} {1} '{2}'", SourceField != default(DataTableField) ? SourceField.DisplayName : "<Source Field>", ComparisonOperator == ComparisonOperator.Equals ? "=" : "<>", ValueField != default(DataTableField) ? ValueField.DisplayName : "<Value Field>"); }
        }

        [JsonConstructor]
        public DataSourceValueCriteria(object parent) : base(parent)
        { }

        ///// <summary>
        ///// Derives a mapping value from a custom value.
        ///// </summary>
        ///// <param name="row">The source row.</param>
        ///// <param name="cancel">The cancellation token.</param>
        ///// <param name="progress">The progress.</param>
        ///// <returns>The custom value.</returns>
        public override object GetValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            object value = row[ValueField.ColumnName];

            if (System.DBNull.Value.Equals(value))
            {
                value = null;
            }

            return value;
        }

        public override string ToString()
        {
            return SourceField?.DisplayName ?? "Data Source Value";
        }
    }
}