using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Update
    /// </summary>
    [Operation(typeof(Dynamics365UpsertOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365UpsertOperation.png")]
    public class Dynamics365UpsertOperation : Dynamics365FieldOperation
    {
        #region Properties

        protected BindingList<FieldValue> keyValues = new BindingList<FieldValue>();

        /// <summary>
        /// Gets or sets the field values to create.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpsertOperation), nameof(KeyValues)), GlobalisedDisplayName(typeof(Dynamics365UpsertOperation), nameof(KeyValues)), GlobalisedDecription(typeof(Dynamics365UpsertOperation), nameof(KeyValues)), Editor(typeof(Dynamics365FieldValueCollectionEditor), typeof(UITypeEditor)), JsonProperty(Order = 3)]
        public BindingList<FieldValue> KeyValues
        {
            get { return keyValues; }
            set
            {
                if (keyValues != value)
                {
                    keyValues = value;

                    if (keyValues != null)
                    {
                        keyValues.ListChanged += KeyValues_ListChanged;
                    }

                    OnPropertyChanged(nameof(KeyValues));
                }
            }
        }

        private void KeyValues_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(KeyValues));
        }

        /// <summary>
        /// Gets or sets a connection to the Dynamics 365 organisation to create records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpsertOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365UpsertOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365UpsertOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        /// <summary>
        /// Gets or sets the entity to create records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpsertOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365UpsertOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365UpsertOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 2)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365UpsertOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365UpsertOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365UpsertOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 1)]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }
        /// <summary>
        /// Gets or sets the source of the record identifier value.
        /// </summary>
        [Browsable(true), JsonIgnore]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [Browsable(true), JsonIgnore]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        /// <summary>
        /// Gets or sets the field values to create.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpsertOperation), nameof(Values)), GlobalisedDisplayName(typeof(Dynamics365UpsertOperation), nameof(Values)), GlobalisedDecription(typeof(Dynamics365UpsertOperation), nameof(Values)), Editor(typeof(Dynamics365FieldValueCollectionEditor), typeof(UITypeEditor)), JsonProperty(Order = 3)]
        public override BindingList<FieldValue> Values
        {
            get { return base.Values; }
            set { base.Values = value; }
        }

        #endregion

        public Dynamics365UpsertOperation(Batch parentBatch) : base(parentBatch)
        {
            RefreshBrowsableFields();
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(TargetSource), false);
            CoreUtility.SetBrowsable(this, nameof(Target), false);
        }

        public override bool CanValidateTarget()
        {
            return false;
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(KeyValues.Count == 0, Properties.Resources.Dynamics365UpsertOperationValidateKeyValues, nameof(KeyValues));

                foreach (FieldValue value in keyValues)
                {
                    result.Errors.AddRange(value.Validate().Errors);
                }
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365UpsertOperationFriendlyName, Entity?.PluralName ?? Properties.Resources.Dynamics365UpsertOperationFriendlyNameEntity, DataSource?.Name ?? Properties.Resources.Dynamics365UpsertOperationFriendlyNameDataSource);
        }

        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();
            EntityMetadata entityMetadata = Entity.GetEntityMetadata(Connection);
            UpsertRequest request = new UpsertRequest() { Target = CreateEntityFromDataRow(row, cancel, progress) };
            AddDuplicateDetectionParameter(request);
            requests.Add(request);

            return requests;
        }

        protected override Entity GetTargetEntity(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            KeyAttributeCollection keyAttributes = new KeyAttributeCollection();

            foreach (FieldValue fieldValue in keyValues)
            {
                keyAttributes.Add(((Dynamics365Field)fieldValue.DestinationField).LogicalName, fieldValue.GetValue(row, cancel, progress));
            }

            return new Entity(Entity.LogicalName, keyAttributes);
        }

        public override void UpdateParentReferences(Batch parentBatch)
        {
            base.UpdateParentReferences(parentBatch);
            keyValues.ToList().ForEach(keyValue => keyValue.UpdateParentReferences(this));
        }


        protected override string GetRequestDescription(OrganizationRequest request)
        {
            UpsertRequest upsertRequest = (UpsertRequest)request;
            string values = string.Join(", ", upsertRequest.Target.Attributes.Select(attribute => GetAttributeValueString(attribute.Value)));

            return string.Format(Properties.Resources.Dynamics365UpsertOperationRequestDescription, Entity.DisplayName, values);
        }

        /// <summary>
        /// Finds Dynamics 365 fields that can be created and updated.
        /// </summary>
        public override List<Field> GetDataDestinationFields()
        {
            List<Field> fields = new List<Field>();

            try
            {
                // todo - should really split this into two and use cancreate fields for 'key value' fields and canupdate for 'value' fields
                fields.AddRange(Entity.GetFields(Connection).Where(field => ((Dynamics365Field)field).CanCreate || ((Dynamics365Field)field).CanUpdate).ToList());
            }
            catch { }

            return fields;
        }
    }
}