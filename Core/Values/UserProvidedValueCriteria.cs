using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    public class UserProvidedValueCriteria : LookupCriteria
    {
        private string value;

        /// <summary>
        /// The user-provided value.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Value"), GlobalisedDecription("The user-provided value to use.")]
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public override string FilterPreview
        {
            get { return string.Format("{0} {1} '{2}'", SourceField != default(DataTableField) ? SourceField.DisplayName : "<Field>", ComparisonOperator == ComparisonOperator.Equals ? "=" : "<>", Value ?? "<Value>"); }
        }

        [JsonConstructor]
        public UserProvidedValueCriteria(object parent) : base(parent)
        { }

        /// <summary>
        /// Derives a mapping value from a custom value.
        /// </summary>
        /// <param name="row">The source row.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>The custom value.</returns>
        public override object GetValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            object value = Value;

            if (System.DBNull.Value.Equals(value))
            {
                value = null;
            }

            return value;
        }

        public override string ToString()
        {
            return SourceField?.DisplayName ?? "User-Provided Value";
        }
    }
}