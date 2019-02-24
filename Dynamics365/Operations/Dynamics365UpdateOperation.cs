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
    /// Updated Dynamics 365 entity records.
    /// </summary>
    [Operation(typeof(Dynamics365UpdateOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365UpdateOperation.png")]
    public class Dynamics365UpdateOperation : Dynamics365FieldOperation
    {
        private new const TargetSource DEFAULT_TARGET_SOURCE = TargetSource.DataSourceValue;

        #region Properties

        /// <summary>
        /// Gets or sets a connection to the Dynamics 365 organisation to create records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpdateOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365UpdateOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365UpdateOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        /// <summary>
        /// Gets or sets the entity to create records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpdateOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365UpdateOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365UpdateOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 2)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365UpdateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365UpdateOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365UpdateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 1)]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets the source of the record identifier value.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpdateOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365UpdateOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365UpdateOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpdateOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365UpdateOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365UpdateOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        /// <summary>
        /// Gets or sets the field values to create.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365UpdateOperation), nameof(Values)), GlobalisedDisplayName(typeof(Dynamics365UpdateOperation), nameof(Values)), GlobalisedDecription(typeof(Dynamics365UpdateOperation), nameof(Values)), Editor(typeof(Dynamics365FieldValueCollectionEditor), typeof(UITypeEditor)), JsonProperty(Order = 3)]
        public override BindingList<FieldValue> Values
        {
            get { return base.Values; }
            set { base.Values = value; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365UpdateOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365UpdateOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365UpdateOperationFriendlyName, Entity?.PluralName ?? Properties.Resources.Dynamics365UpdateOperationFriendlyNameEntity, DataSource?.Name ?? Properties.Resources.Dynamics365UpdateOperationFriendlyNameDataSource);
        }

        /// <summary>
        /// Create a list of OrganizationRequest objects for the specified data row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A list of OrganizationRequest objects.</returns>
        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();
            EntityMetadata entityMetadata = Entity.GetEntityMetadata(Connection);

            UpdateRequest request = new UpdateRequest() { Target = CreateEntityFromDataRow(row, cancel, progress) };
            AddDuplicateDetectionParameter(request);
            requests.Add(request);

            return requests;
        }

        /// <summary>
        /// Gets a description of the specified request. 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The request description.</returns>
        protected override string GetRequestDescription(OrganizationRequest request)
        {
            UpdateRequest updateRequest = (UpdateRequest)request;

            string description = updateRequest.Target.LogicalName;
            string values = string.Join(", ", updateRequest.Target.Attributes.Select(attribute => GetAttributeValueString(attribute.Value)));

            description = string.Format(Properties.Resources.Dynamics365UpdateOperationRequestDescription, Entity.DisplayName, updateRequest.Target.Id.ToString(), values);

            return description;
        }

        /// <summary>
        /// Finds Dynamics 365 fields that can be updated.
        /// </summary>
        public override List<Field> GetDataDestinationFields()
        {
            List<Field> fields = base.GetDataDestinationFields();

            try
            {
                fields.RemoveAll(field => !(((Dynamics365Field)field).CanUpdate || ((Dynamics365Field)field).IsPrimaryId));
            }
            catch { }

            return fields;
        }
    }
}
