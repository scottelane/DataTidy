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
    [Operation(typeof(Dynamics365DisassociateOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365DisassociateOperation.png")]
    public class Dynamics365DisassociateOperation : Dynamics365RelationshipOperation
    {
        #region Properties

        [GlobalisedCategory(typeof(Dynamics365DisassociateOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365DisassociateOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365DisassociateOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365DisassociateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365DisassociateOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365DisassociateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 2)]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365DisassociateOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365DisassociateOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365DisassociateOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 3)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365DisassociateOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365DisassociateOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365DisassociateOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365DisassociateOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365DisassociateOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365DisassociateOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        #endregion

        public Dynamics365DisassociateOperation(Batch parentBatch) : base(parentBatch)
        { }

        protected override string GenerateFriendlyName()
        {
            return string.Format(Properties.Resources.Dynamics365DisassociateOperationFriendlyName, Entity?.PluralName ?? Properties.Resources.Dynamics365DisassociateOperationFriendlyNameEntity, AssociatedEntity?.PluralName ?? Properties.Resources.Dynamics365DisassociateOperationFriendlyNameRelatedEntity, Relationship?.SchemaName ?? Properties.Resources.Dynamics365DisassociateOperationFriendlyNameRelationship);
        }

        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection
            {
                GetAssociatedEntity(row, cancel, progress).ToEntityReference()
            };
            requests.Add(new DisassociateRequest()
            {
                Target = GetTargetEntity(row, cancel, progress).ToEntityReference(),
                RelatedEntities = relatedEntities,
                Relationship = new Relationship(Relationship.SchemaName)
            });

            return requests;
        }

        protected override string GetRequestDescription(OrganizationRequest request)
        {
            DisassociateRequest disassociateRequest = (DisassociateRequest)request;
            return string.Format(Properties.Resources.Dynamics365DisassociateOperationRequestDescription, Entity.DisplayName, disassociateRequest.Target.Id, AssociatedEntity.DisplayName, disassociateRequest.RelatedEntities[0].Id);
        }
    }
}