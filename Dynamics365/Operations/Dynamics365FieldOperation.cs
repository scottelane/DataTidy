using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Microsoft.Xrm.Sdk;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Provides a base implementation for operations that set values on Dynamics 365 records.
    /// </summary>
    public abstract class Dynamics365FieldOperation : Dynamics365RecordOperation, IFieldValueCreator, ICustomMenuItemProvider
    {
        private const bool DEFAULT_DUPLICATE_DETECTION = false;

        protected BindingList<FieldValue> values = new BindingList<FieldValue>();

        [GlobalisedCategory(typeof(Dynamics365FieldOperation), nameof(Values)), GlobalisedDisplayName(typeof(Dynamics365FieldOperation), nameof(Values)), GlobalisedDecription(typeof(Dynamics365FieldOperation), nameof(Values)), Editor(typeof(Dynamics365FieldValueCollectionEditor), typeof(UITypeEditor))]
        public virtual BindingList<FieldValue> Values
        {
            get { return values; }
            set
            {
                if (values != value)
                {
                    values = value;
                    OnPropertyChanged(nameof(Values));
                }
            }
        }

        protected bool duplicateDetection = DEFAULT_DUPLICATE_DETECTION;

        [GlobalisedCategory(typeof(Dynamics365FieldOperation), nameof(DuplicateDetection)), GlobalisedDisplayName(typeof(Dynamics365FieldOperation), nameof(DuplicateDetection)), GlobalisedDecription(typeof(Dynamics365FieldOperation), nameof(DuplicateDetection)), DefaultValue(DEFAULT_DUPLICATE_DETECTION)]
        public virtual bool DuplicateDetection
        {
            get { return duplicateDetection; }
            set
            {
                if (duplicateDetection != value)
                {
                    duplicateDetection = value;
                    OnPropertyChanged(nameof(DuplicateDetection));
                }
            }
        }

        public Dynamics365FieldOperation(Batch parentBatch) : base(parentBatch)
        { }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(values.Count == 0, Properties.Resources.Dynamics365FieldOperationValidateValues, nameof(Values));

                Dictionary<string, Dynamics365Field> fields = new Dictionary<string, Dynamics365Field>();

                foreach (FieldValue value in values)
                {
                    result.Errors.AddRange(value.Validate().Errors);

                    //if (value.DestinationField is Dynamics365Field field)
                    //{
                    //    string key = string.Format("{0}:{1}", field.EntityLogicalName, field.LogicalName);

                    //    if (!fields.ContainsKey(key))
                    //    {
                    //        fields[key] = field;
                    //    }
                    //    else
                    //    {
                    // todo - this has been allowed to support customerid lookups where different rows look up to a different target entity
                    //result.AddErrorIf(true, string.Format(Properties.Resources.Dynamics365FieldOperationValidateValuesDuplicate, field.DisplayName), nameof(Values));
                    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        protected virtual Entity CreateEntityFromDataRow(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            Entity entity = GetTargetEntity(row, cancel, progress);

            foreach (FieldValue value in Values)
            {
                PopulateEntityFromFieldValue(entity, value, row, cancel, progress);
            }

            return entity;
        }

        public static Entity PopulateEntityFromFieldValue(Entity entity, FieldValue value,  DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            object newValue = value.GetValue(row, cancel, progress);
            string key = ((Dynamics365Field)value.DestinationField).LogicalName;

            // if multiple values for the same field are found, use the latest non-null field
            if (!entity.Attributes.ContainsKey(key) || (entity.Attributes.ContainsKey(key) && newValue != default(object)))
            {
                entity[key] = newValue;
            }

            return entity;
        }

        protected void AddDuplicateDetectionParameter(OrganizationRequest request)
        {
            request.Parameters.Add("SuppressDuplicateDetection", !duplicateDetection);
        }

        public override void UpdateParentReferences(Batch parentBatch)
        {
            base.UpdateParentReferences(parentBatch);
            values.ToList().ForEach(fieldValue => fieldValue.UpdateParentReferences(this));
        }

        public override IOperation Clone(bool addSuffix)
        {
            Dynamics365FieldOperation clone = (Dynamics365FieldOperation)base.Clone(addSuffix);
            clone.Values = new BindingList<FieldValue>();
            Values.ToList().ForEach(value => clone.Values.Add(value.Clone()));
            return clone;
        }

        public FieldValue CreateFieldValue(Type type)
        {
            return (FieldValue)Activator.CreateInstance(type, new object[] { this });
        }

        public List<CustomMenuItem> GetCustomMenuItems()
        {
            List<CustomMenuItem> menuItems = new List<CustomMenuItem>
            {
                new CustomMenuItem()
                {
                    Text = Properties.Resources.Dynamics365FieldOperationGenerateMappingsText,
                    ToolTip = Properties.Resources.Dynamics365FieldOperationGenerateMappingsToolTip,
                    Icon = Properties.Resources.GenerateValueMappings,
                    Item = this,
                    AsynchronousEventHandler = GenerateFieldMappings
                }
            };

            return menuItems;
        }

        private void GenerateFieldMappings(CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            progress?.Report(new ExecutionProgress(NotificationType.Information, Properties.Resources.Dynamics365FieldOperationGenerateMappingsReadingDataSourceFields));
            List<DataTableField> sourceFields = DataTableField.GetDataTableFields(DataSource?.GetDataColumns());

            cancel.ThrowIfCancellationRequested();

            progress?.Report(new ExecutionProgress(NotificationType.Information, Properties.Resources.Dynamics365FieldOperationGenerateMappingsReadingEntityFields));
            List<Field> destinationFields = Entity.GetFields(Connection).Where(field => ((Dynamics365Field)field).CanUpdate || ((Dynamics365Field)field).CanCreate).ToList();

            cancel.ThrowIfCancellationRequested();

            values?.Clear();

            foreach (DataTableField sourceField in sourceFields)
            {
                // Formats to match:
                // fieldLogicalName
                // entityLogicalName.fieldLogicalName
                // entityLogicalName.fieldLogicalName.Identifier
                // entityLogicalName.fieldLogicalName.Code
                // FieldDisplayName
                // FieldDisplayName (<other text>)
                // FieldDisplayName (Identifier)
                // FieldDisplayName (Code)

                string strippedColumnName = sourceField.ColumnName;

                if (strippedColumnName.EndsWith(".Identifier") || strippedColumnName.EndsWith(".Code"))
                {
                    strippedColumnName = strippedColumnName.Substring(0, strippedColumnName.LastIndexOf("."));
                }
                else if (strippedColumnName.EndsWith(" (Identifier)") || strippedColumnName.EndsWith(" (Code)"))
                {
                    strippedColumnName = strippedColumnName.Substring(0, strippedColumnName.LastIndexOf(" "));
                }

                // todo - also add option set support if just name is supplied
                Dynamics365Field destinationField = (Dynamics365Field)destinationFields.Find(field =>
                    strippedColumnName == ((Dynamics365Field)field).LogicalName
                || strippedColumnName == string.Concat(((Dynamics365Field)field).EntityLogicalName, ".", ((Dynamics365Field)field).LogicalName)
                || strippedColumnName == ((Dynamics365Field)field).DisplayName);

                if (destinationField != default(Dynamics365Field))
                {
                    // todo - use other fields ('name', 'entity') to correct set target and possibly create lookup data field
                    values.Add(new Dynamics365DataSourceValue(this)
                    {
                        DestinationField = destinationField,
                        SourceField = sourceField
                    });
                }

                cancel.ThrowIfCancellationRequested();
            }

            progress?.Report(new ExecutionProgress(NotificationType.Information, string.Format(Properties.Resources.Dynamics365FieldOperationGenerateMappingsSuccessful, values.Count)));
        }

        public string GetAttributeValueString(object attributeValue)
        {
            string valueString = attributeValue?.ToString() ?? string.Empty;

            if (attributeValue is OptionSetValue optionSet)
            {
                valueString = optionSet.Value.ToString();
            }
            else if (attributeValue is EntityReference entityReference)
            {
                valueString = entityReference.Id.ToString();
            }
            else if (attributeValue is EntityCollection entityCollection)
            {
                if (entityCollection.Entities.Count > 0)
                {
                    valueString = ((EntityReference)entityCollection.Entities[0].Attributes["partyid"]).Id.ToString();
                }
                else
                {
                    valueString = "null";
                }
            }
            else if (attributeValue is Money money)
            {
                valueString = money.Value.ToString();
            }

            return valueString;
        }

        public override List<Field> GetDataDestinationFields()
        {
            List<Field> fields = base.GetDataDestinationFields();

            try
            {
                foreach (FieldValue value in values)
                {
                    // prevent fields from being selected more than once
                    // todo - check - removed to allow customer fields to work with multiple lookups for one field
                    //fields.RemoveAll(field => field == value.DestinationField);
                }
            }
            catch { }

            return fields;
        }
    }
}

