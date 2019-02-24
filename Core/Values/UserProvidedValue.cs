using System;
using System.Data;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Gets a user-provided value.
    /// </summary>
    public class UserProvidedValue : FieldValue
    {
        protected string value;

        /// <summary>
        /// Gets or sets the user-provided value.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Value"), GlobalisedDecription("The user-provided value .")]
        public virtual string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the UserProvidedValue class with the specified parent operation.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        [JsonConstructor]
        public UserProvidedValue(object parent) : base(parent)
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
    }
}
