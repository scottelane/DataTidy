using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// todo - consider moving this to an interface - would need to provide a reference to parent lookup. make parent a generic object? check if parent supports interface and if so type to interface
    /// </summary>
    public abstract class LookupCriteria : ILookupSourceFieldsProvider, IDataSourceFieldsProvider
    {
        private const ComparisonOperator DEFAULT_COMPARISON_OPERATOR = ComparisonOperator.Equals;

        protected ComparisonOperator comparisonOperator = DEFAULT_COMPARISON_OPERATOR;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Operator"), GlobalisedDecription("The comparison operator to apply."), DefaultValue(DEFAULT_COMPARISON_OPERATOR), Browsable(true)]
        public virtual ComparisonOperator ComparisonOperator
        {
            get { return comparisonOperator; }
            set
            {
                if (comparisonOperator != value)
                {
                    comparisonOperator = value;
                    OnPropertyChanged(nameof(ComparisonOperator));
                }
            }
        }

        protected DataTableField sourceField;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Field"), GlobalisedDecription("The field in the data source being looked up to compare"), TypeConverter(typeof(LookupSourceFieldConverter)), Browsable(true)]
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

        public virtual string FilterPreview
        {
            get { return string.Format("{0} {1} '{2}'", SourceField?.DisplayName ?? "<Field>", ComparisonOperator == ComparisonOperator.Equals ? "=" : "<>", "<Value>"); }
        }

        public LookupCriteria(object parent)
        {
            UpdateParentReferences(parent);
        }

        public void UpdateParentReferences(object parent)
        {
            Parent = parent;
        }

        public virtual LookupCriteria Clone()
        {
            LookupCriteria clone = (LookupCriteria)MemberwiseClone();
            return clone;
        }

        public abstract object GetValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress);

        public override string ToString()
        {
            return SourceField?.DisplayName ?? "Lookup Criteria";
        }

        public List<DataTableField> GetLookupSourceFields()
        {
            return ((ILookupSourceFieldsProvider)parent).GetLookupSourceFields();
        }

        public List<DataTableField> GetDataSourceFields()
        {
            return ((IDataSourceFieldsProvider)Parent).GetDataSourceFields();
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

    public enum ComparisonOperator
    {
        Equals,
        DoesNotEqual
    }
}
