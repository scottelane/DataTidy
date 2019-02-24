using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    [Operation(typeof(Dynamics365AssociateOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365AssociateOperation.png")]
    public class Dynamics365AssociateOperation : Dynamics365RelationshipOperation
    {
        #region Properties

        [GlobalisedCategory(typeof(Dynamics365AssociateOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365AssociateOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365AssociateOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365AssociateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365AssociateOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365AssociateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 2)]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365AssociateOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365AssociateOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365AssociateOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 3)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365AssociateOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365AssociateOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365AssociateOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365AssociateOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365AssociateOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365AssociateOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365AssociateOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365AssociateOperation(Batch parentBatch) : base(parentBatch)
        { }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365AssociateOperationFriendlyName, Entity?.PluralName ?? Properties.Resources.Dynamics365AssociateOperationFriendlyNameEntity, AssociatedEntity?.PluralName ?? Properties.Resources.Dynamics365AssociateOperationFriendlyNameRelatedEntity, Relationship?.SchemaName ?? Properties.Resources.Dynamics365AssociateOperationFriendlyNameRelationship);
        }

        /// <summary>
        /// Creates organisation requrests for the operation.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>A list of organisation requests.</returns>
        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection
            {
                GetAssociatedEntity(row, cancel, progress).ToEntityReference()
            };
            requests.Add(new AssociateRequest()
            {
                Target = GetTargetEntity(row, cancel, progress).ToEntityReference(),
                RelatedEntities = relatedEntities,
                Relationship = new Relationship(Relationship.SchemaName)
            });

            return requests;
        }

        protected override string GetRequestDescription(OrganizationRequest request)
        {
            AssociateRequest associateRequest = (AssociateRequest)request;
            return string.Format(Properties.Resources.Dynamics365AssociateOperationRequestDescription, Entity.DisplayName, associateRequest.Target.Id, AssociatedEntity.DisplayName, associateRequest.RelatedEntities[0].Id);
        }
    }
}