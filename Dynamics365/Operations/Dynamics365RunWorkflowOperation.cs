using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Runs an on-demand Dynamics 365 workflow against a record.
    /// </summary>
    [Operation(typeof(Dynamics365RunWorkflowOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365RunWorkflowOperation.png")]
    public class Dynamics365RunWorkflowOperation : Dynamics365RecordOperation, IDynamics365WorkflowProvider
    {
        #region Properties

        private Dynamics365Workflow workflow;

        /// <summary>
        /// Gets or sets the on-demand workflow to run.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RunWorkflowOperation), nameof(Workflow)), GlobalisedDisplayName(typeof(Dynamics365RunWorkflowOperation), nameof(Workflow)), GlobalisedDecription(typeof(Dynamics365RunWorkflowOperation), nameof(Workflow)), TypeConverter(typeof(Dynamics365WorkflowConverter))]
        public Dynamics365Workflow Workflow
        {
            get { return workflow; }
            set
            {
                if (workflow != value)
                {
                    workflow = value;
                    OnPropertyChanged(nameof(Workflow));
                    RefreshName();
                }
            }
        }

        /// <summary>
        /// Gets or sets a connection to the Dynamics 365 organisation to run workflows in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RunWorkflowOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365RunWorkflowOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365RunWorkflowOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        /// <summary>
        /// Gets or sets the entity to run workflows against.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RunWorkflowOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365RunWorkflowOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365RunWorkflowOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 2)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        [GlobalisedCategory(typeof(Dynamics365RunWorkflowOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365RunWorkflowOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365RunWorkflowOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter)), JsonProperty(Order = 1)]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets the source of the record identifier value.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RunWorkflowOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365RunWorkflowOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365RunWorkflowOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365RunWorkflowOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365RunWorkflowOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365RunWorkflowOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        #endregion

        public Dynamics365RunWorkflowOperation(Batch parentBatch) : base(parentBatch)
        { }

        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();

            requests.Add(new ExecuteWorkflowRequest()
            {
                WorkflowId = workflow.ID,
                EntityId = GetTargetEntity(row, cancel, progress).ToEntityReference().Id
            });

            return requests;
        }

        public override Core.ValidationResult Validate()
        {
            Core.ValidationResult result = base.Validate();
            result.AddErrorIf(Workflow == default(Dynamics365Workflow), Properties.Resources.Dynamics365RunWorkflowOperationValidateWorkflow, nameof(Workflow));
            return result;
        }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return string.Format("Run {0} for records in {1}", Workflow?.Name ?? "<Workflow>", DataSource?.Name ?? "<Data Source>");
        }

        protected override string GetRequestDescription(OrganizationRequest request)
        {
            ExecuteWorkflowRequest workflowRequest = (ExecuteWorkflowRequest)request;
            return string.Format("Running {0} for {1}", Workflow.Name, workflowRequest.EntityId);
        }

        public List<Dynamics365Workflow> GetWorkflows()
        {
            List<Dynamics365Workflow> workflows = new List<Dynamics365Workflow>();

            try
            {
                workflows.AddRange(Dynamics365Workflow.GetWorkflows(Connection, Entity.LogicalName));
            }
            catch { }

            return workflows;
        }
    }
}
