using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Gets a value from a data source record field.
    /// </summary>
    public class DataSourceValue : FieldValue
    {
        protected DataTableField sourceField;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Source Field"), GlobalisedDecription("The data source field to get the value from."), TypeConverter(typeof(DataSourceFieldConverter))]
        public virtual DataTableField SourceField
        {
            get { return sourceField; }
            set
            {
                if (sourceField != value)
                {
                    sourceField = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(SourceField));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataSourceValue class with the specified parent operation.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        [JsonConstructor]
        public DataSourceValue(object parent) : base(parent)
        { }

        /// <summary>
        /// Gets a value from the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The value.</returns>
        public override object GetValue(DataRow record, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            object value = record[SourceField.ColumnName];

            if (System.DBNull.Value.Equals(value))
            {
                value = null;
            }

            return value;
        }

        /// <summary>
        /// Validates the class properties.
        /// </summary>
        /// <returns>The ValidationResult.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            result.AddErrorIf(SourceField == default(DataTableField), "Please select a Source Field", nameof(SourceField));
            return result;
        }
    }
}
