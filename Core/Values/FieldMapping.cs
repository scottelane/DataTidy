using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provides a mapping between two fields.
    /// </summary>
    public class FieldMapping : IDataSourceFieldsProvider, ISecondaryDataSourceFieldsProvider, INotifyPropertyChanged
    {
        protected DataTableField sourceField;

        [GlobalisedCategory("Field Mapping"), GlobalisedDisplayName("Source Field"), GlobalisedDecription("The source data source field to compare."), TypeConverter(typeof(DataSourceFieldConverter)), Browsable(true)]
        public virtual DataTableField SourceField
        {
            get { return sourceField; }
            set
            {
                if (sourceField != value)
                {
                    sourceField = value;
                    OnPropertyChanged(nameof(SourceField));
                }
            }
        }

        private DataTableField comparisonField;

        [GlobalisedCategory("Field Mapping"), GlobalisedDisplayName("Comparison Field"), GlobalisedDecription("The comparison data source field to compare against."), TypeConverter(typeof(SecondaryDataSourceFieldConverter)), Browsable(true)]
        public virtual DataTableField ComparisonField
        {
            get { return comparisonField; }
            set
            {
                if (comparisonField != value)
                {
                    comparisonField = value;
                    OnPropertyChanged(nameof(ComparisonField));
                }
            }
        }

        private object parent;

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public virtual object Parent
        {
            get { return parent; }
            protected set
            {
                if (parent != value)
                {
                    parent = value;
                    OnPropertyChanged(nameof(Parent));
                }
            }
        }

        [JsonConstructor]
        public FieldMapping(object parent)
        {
            UpdateParentReferences(parent);
        }

        public void UpdateParentReferences(object parent)
        {
            Parent = parent;
        }

        public override string ToString()
        {
            string value = "Field Mapping";

            if (SourceField != default(DataTableField))
            {
                value = SourceField.ToString();
            }

            return value;
        }

        public List<DataTableField> GetDataSourceFields()
        {
            return ((IDataSourceFieldsProvider)Parent).GetDataSourceFields();
        }

        public List<DataTableField> GetSecondaryDataSourceFields()
        {
            return ((ISecondaryDataSourceFieldsProvider)Parent).GetSecondaryDataSourceFields();
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
    }
}
