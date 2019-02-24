using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using ScottLane.DataTidy.Core;
using Newtonsoft.Json;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Deletes Dynamics 365 records.
    /// </summary>
    [Operation(typeof(Dynamics365DeleteOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365DeleteOperation.png")]
    public class Dynamics365DeleteOperation : Dynamics365RecordOperation
    {
        #region Properties

        /// <summary>
        /// Gets or sets a connection to the Dynamics 365 organisation to create records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365DeleteOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365DeleteOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365DeleteOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        /// <summary>
        /// Gets or sets the entity to create records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365DeleteOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365DeleteOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365DeleteOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 2)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365DeleteOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365DeleteOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365DeleteOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 1)]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets the source of the record identifier value.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365DeleteOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365DeleteOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365DeleteOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365DeleteOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365DeleteOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365DeleteOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365DeleteOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365DeleteOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Creates a list of organisation requests from a data row.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A list of organisation requests.</returns>
        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();
            DeleteRequest request = new DeleteRequest()
            {
                Target = GetTargetEntity(row, cancel, progress).ToEntityReference()
            };
            requests.Add(request);
            return requests;
        }

        /// <summary>
        /// Gets a description for a delete organisation request.
        /// </summary>
        /// <param name="request">The organisation request.</param>
        /// <returns>The request description.</returns>
        protected override string GetRequestDescription(OrganizationRequest request)
        {
            DeleteRequest deleteRequest = (DeleteRequest)request;
            return string.Format(Properties.Resources.Dynamics365DeleteOperationRequestDescription, Entity.DisplayName, deleteRequest.Target.Id);
        }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365DeleteOperationFriendlyName, Entity?.PluralName ?? Properties.Resources.Dynamics365DeleteOperationFriendlyNameEntity, Connection?.Name ?? Properties.Resources.Dynamics365DeleteOperationFriendlyNameConnection, DataSource?.Name ?? Properties.Resources.Dynamics365DeleteOperationFriendlyNameDataSource);
        }
    }
}
