using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provides the framework for getting a field value.
    /// </summary>
    public abstract class FieldValue : IDataDestinationFieldsProvider, IDataSourceFieldsProvider, INotifyPropertyChanged
    {
        protected Field destinationField;

        [GlobalisedCategory(typeof(FieldValue), nameof(DestinationField)), GlobalisedDisplayName(typeof(FieldValue), nameof(DestinationField)), GlobalisedDecription(typeof(FieldValue), nameof(DestinationField)), TypeConverter(typeof(DataDestinationFieldConverter))]
        public virtual Field DestinationField
        {
            get { return destinationField;}
            set
            {
                if (destinationField != value)
                {
                    destinationField = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(DestinationField));
                }
            }
        }

        /// <summary>
        /// Gets the parent Operation.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public object Parent { get; protected set; }

        [JsonConstructor]
        public FieldValue(object parent)
        {
            UpdateParentReferences(parent);
            RefreshBrowsableFields();
        }

        protected virtual void RefreshBrowsableFields()
        { }

        public virtual void UpdateParentReferences(object parent)
        {
            Parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method that must be overriden in derived classes to get the actual field value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="cancel"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public abstract object GetValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress);

        /// <summary>
        /// Validates the field value properties.
        /// </summary>
        /// <returns>The validation result.</returns>
        public virtual ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            result.AddErrorIf(DestinationField == default(Field), "Please select a Destination Field", nameof(DestinationField));
            return result;
        }

        public virtual FieldValue Clone()
        {
            FieldValue clone = (FieldValue)MemberwiseClone();
            return clone;
        }

        public override string ToString()
        {
            return DestinationField?.DisplayName ?? "Field Value";
        }

        #region Interface Methods

        public List<Field> GetDataDestinationFields()
        {
            return ((IDataDestinationFieldsProvider)Parent).GetDataDestinationFields();
        }

        public virtual List<DataTableField> GetDataSourceFields()
        {
            return ((IDataSourceFieldsProvider)Parent).GetDataSourceFields();
        }

        #endregion
    }
}
