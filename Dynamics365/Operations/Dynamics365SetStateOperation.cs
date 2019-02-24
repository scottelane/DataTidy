using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    [Operation(typeof(Dynamics365SetStateOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365SetStateOperation.png")]
    public class Dynamics365SetStateOperation : Dynamics365RecordOperation, IDynamics365StatesProvider, IDynamics365StatusesProvider, IDynamics365SetStateDataDestinationFieldsProvider, IFieldValueCreator
    {
        private const string STATECODE_NAME = "statecode";
        private const string STATUSCODE_NAME = "statuscode";
        public const string CASE_ENTITY_NAME = "incident";
        public const string CASE_RESOLUTION_INCIDENT_ID = "incidentid";
        public const string CASE_RESOLUTION_ENTITY_NAME = "incidentresolution";
        public const string QUOTE_ENTITY_NAME = "quote";
        public const string QUOTE_CLOSE_QUOTE_ID = "quoteid";
        public const string QUOTE_CLOSE_ENTITY_NAME = "quoteclose";
        public const string OPPORTUNITY_ENTITY_NAME = "opportunity";
        public const string OPPORTUNITY_CLOSE_ENTITY_NAME = "opportunityclose";
        public const string OPPORTUNITY_CLOSE_OPPORTUNITY_ID = "opportunityid";
        public const string PROCESS_ENTITY_NAME = "process";
        private const string METADATA_CACHE_KEY = "metadata";
        private const SetStateMode DEFAULT_SET_STATE_MODE = SetStateMode.Fixed;

        #region Properties

        private SetStateMode mode = DEFAULT_SET_STATE_MODE;

        [GlobalisedCategory("Set State"), GlobalisedDisplayName("Mode"), GlobalisedDecription("If Fixed, a single state and status code can be set for all records. If Variable, different state and status codes can be set for each record in addition to supporting information for case, quote and opportunity entities."), DefaultValue(DEFAULT_SET_STATE_MODE)]
        public SetStateMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(Mode));
                    RefreshName();
                }
            }
        }

        private Dynamics365State state;

        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(State)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(State)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(State)), TypeConverter(typeof(Dynamics365StateConverter)), Browsable(true)]
        public Dynamics365State State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged(nameof(State));
                    RefreshName();
                }
            }
        }

        private Dynamics365Status status;

        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(Status)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(Status)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(Status)), TypeConverter(typeof(Dynamics365StatusConverter)), Browsable(true)]
        public Dynamics365Status Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                    RefreshName();
                }
            }
        }

        protected BindingList<FieldValue> values = new BindingList<FieldValue>();

        [GlobalisedCategory("Set State"), GlobalisedDisplayName("Values"), GlobalisedDecription("The state, status and supporting values for case, quote and opportunity closure entities."), Editor(typeof(Dynamics365SetStateFieldValueCollectionEditor), typeof(UITypeEditor)), Browsable(true)]
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

        /// <summary>
        /// Gets or sets the source of the target record identifier.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        /// <summary>
        /// Gets or sets the data source to read records from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter))]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets a connection to the Dynamics 365 organisation to assign records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        /// <summary>
        /// Gets or sets the Dynamics365Entity containing records to assign.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365SetStateOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365SetStateOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365SetStateOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365EntityConverter)), JsonProperty(Order = 2)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set
            {
                if (entity != value)
                {
                    entity = value;
                    OnPropertyChanged(nameof(Entity));
                    RefreshName();
                }
            }
        }

        #endregion

        public Dynamics365SetStateOperation(Batch parentBatch) : base(parentBatch)
        {
            RefreshBrowsableFields();
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(State), mode == SetStateMode.Fixed);
            CoreUtility.SetBrowsable(this, nameof(Status), mode == SetStateMode.Fixed);
            CoreUtility.SetBrowsable(this, nameof(Values), mode != SetStateMode.Fixed);
        }

        protected override string GenerateFriendlyName()
        {
            string state = mode == SetStateMode.Fixed ? State?.Name : null;
            string status = mode == SetStateMode.Fixed ? Status?.Name : null;

            return string.Format(Properties.Resources.Dynamics365SetStateOperationFriendlyName, Entity?.PluralName ?? Properties.Resources.Dynamics365SetStateOperationFriendlyNameEntity, state ?? Properties.Resources.Dynamics365SetStateOperationFriendlyNameState, status ?? Properties.Resources.Dynamics365SetStateOperationFriendlyNameStatus);
        }

        public override Core.ValidationResult Validate()
        {
            Core.ValidationResult result = base.Validate();
            result.AddErrorIf(Mode == SetStateMode.Fixed && State == default(Dynamics365State), Properties.Resources.Dynamics365SetStateOperationValidateState, nameof(State));
            result.AddErrorIf(Mode == SetStateMode.Fixed && Status == default(Dynamics365Status), Properties.Resources.Dynamics365SetStateOperationValidateStatus, nameof(Status));

            List<ValidationError> valueErrors = new List<ValidationError>();

            foreach (FieldValue value in values)
            {
                valueErrors.AddRange(value.Validate().Errors);
            }

            result.Errors.AddRange(valueErrors);

            if (valueErrors.Count == 0)
            {
                result.AddErrorIf(Mode == SetStateMode.Variable && Values.Count(fieldValue => ((Dynamics365Field)fieldValue.DestinationField).LogicalName == STATECODE_NAME) == 0, Properties.Resources.Dynamics365SetStateOperationValidateState, nameof(Values)); // todo - new message
                result.AddErrorIf(Mode == SetStateMode.Variable && Values.Count(fieldValue => ((Dynamics365Field)fieldValue.DestinationField).LogicalName == STATUSCODE_NAME) == 0, Properties.Resources.Dynamics365SetStateOperationValidateState, nameof(Values)); // todo - new message
            }

            return result;
        }

        protected override List<OrganizationRequest> CreateOrganisationRequests(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            List<OrganizationRequest> requests = new List<OrganizationRequest>();

            // todo - hard coded strings
            OptionSetValue stateValue = mode == SetStateMode.Fixed ? new OptionSetValue(State.Code) : (OptionSetValue)Values.First(fieldValue => ((Dynamics365Field)fieldValue.DestinationField).LogicalName == STATECODE_NAME).GetValue(row, cancel, progress);
            OptionSetValue statusValue = mode == SetStateMode.Fixed ? new OptionSetValue(Status.Code) : (OptionSetValue)Values.First(fieldValue => ((Dynamics365Field)fieldValue.DestinationField).LogicalName == STATUSCODE_NAME).GetValue(row, cancel, progress);

            if (Entity.LogicalName == CASE_ENTITY_NAME && stateValue.Value == StateCode.CaseResolved)
            {
                EntityMetadata metadata = Dynamics365Entity.Create(CASE_RESOLUTION_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                Entity incidentResolution = new Entity(metadata.LogicalName);
                incidentResolution[CASE_RESOLUTION_INCIDENT_ID] = GetTargetEntity(row, cancel, progress).ToEntityReference();

                if (mode == SetStateMode.Variable)
                {
                    PopulateEntityFromDataRow(incidentResolution, row, cancel, progress);
                }

                requests.Add(new CloseIncidentRequest()
                {
                    IncidentResolution = incidentResolution,
                    Status = statusValue
                });
            }
            else if (Entity.LogicalName == QUOTE_ENTITY_NAME && stateValue.Value == StateCode.QuoteDraft)
            {
                requests.Add(new ReviseQuoteRequest()
                {
                    QuoteId = GetTargetEntity(row, cancel, progress).Id,
                    ColumnSet = new ColumnSet(true)
                });
            }
            else if (Entity.LogicalName == QUOTE_ENTITY_NAME && stateValue.Value == StateCode.QuoteWon)
            {
                EntityMetadata metadata = Dynamics365Entity.Create(QUOTE_CLOSE_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                Entity quoteClose = new Entity(metadata.LogicalName);
                quoteClose[QUOTE_CLOSE_QUOTE_ID] = GetTargetEntity(row, cancel, progress).ToEntityReference();

                if (mode == SetStateMode.Variable)
                {
                    PopulateEntityFromDataRow(quoteClose, row, cancel, progress);
                }

                requests.Add(new WinQuoteRequest()
                {
                    QuoteClose = quoteClose,
                    Status = statusValue
                });
            }
            else if (Entity.LogicalName == QUOTE_ENTITY_NAME && stateValue.Value == StateCode.QuoteClosed)
            {
                EntityMetadata metadata = Dynamics365Entity.Create(QUOTE_CLOSE_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                Entity quoteClose = new Entity(metadata.LogicalName);
                quoteClose[QUOTE_CLOSE_QUOTE_ID] = GetTargetEntity(row, cancel, progress).ToEntityReference();

                if (mode == SetStateMode.Variable)
                {
                    PopulateEntityFromDataRow(quoteClose, row, cancel, progress);
                }

                requests.Add(new CloseQuoteRequest()
                {
                    QuoteClose = quoteClose,
                    Status = statusValue
                });
            }
            else if (Entity.LogicalName == OPPORTUNITY_ENTITY_NAME && stateValue.Value == StateCode.OpportunityWon)
            {
                EntityMetadata metadata = Dynamics365Entity.Create(OPPORTUNITY_CLOSE_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                Entity opportunityClose = new Entity(metadata.LogicalName);
                opportunityClose[OPPORTUNITY_CLOSE_OPPORTUNITY_ID] = GetTargetEntity(row, cancel, progress).ToEntityReference();

                if (mode == SetStateMode.Variable)
                {
                    PopulateEntityFromDataRow(opportunityClose, row, cancel, progress);
                }

                requests.Add(new WinOpportunityRequest()
                {
                    OpportunityClose = opportunityClose,
                    Status = statusValue
                });
            }
            else if (Entity.LogicalName == OPPORTUNITY_ENTITY_NAME && State.Code == StateCode.OpportunityLost)
            {
                EntityMetadata metadata = Dynamics365Entity.Create(OPPORTUNITY_CLOSE_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                Entity opportunityClose = new Entity(metadata.LogicalName);
                opportunityClose[OPPORTUNITY_CLOSE_OPPORTUNITY_ID] = GetTargetEntity(row, cancel, progress).ToEntityReference();

                if (mode == SetStateMode.Variable)
                {
                    PopulateEntityFromDataRow(opportunityClose, row, cancel, progress);
                }

                requests.Add(new LoseOpportunityRequest()
                {
                    OpportunityClose = opportunityClose,
                    Status = statusValue
                });
            }
            else if (Entity.LogicalName == PROCESS_ENTITY_NAME)
            {
                requests.Add(new SetStateRequest()
                {
                    EntityMoniker = new EntityReference(Entity.LogicalName, GetTargetEntity(row, cancel, progress).Id),
                    State = new OptionSetValue(stateValue.Value),
                    Status = new OptionSetValue(statusValue.Value)
                });
            }
            else
            {
                UpdateRequest request = new UpdateRequest()
                {
                    Target = GetTargetEntity(row, cancel, progress)
                };

                request.Target.Attributes[STATECODE_NAME] = stateValue;
                request.Target.Attributes[STATUSCODE_NAME] = statusValue;
                requests.Add(request);
            }

            return requests;
        }

        private void PopulateEntityFromDataRow(Entity entity, DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            foreach (FieldValue value in Values)
            {
                string logicalName = ((Dynamics365Field)value.DestinationField).LogicalName;

                if (logicalName != STATECODE_NAME && logicalName != STATUSCODE_NAME)
                {
                    Dynamics365FieldOperation.PopulateEntityFromFieldValue(entity, value, row, cancel, progress);
                }
            }
        }

        protected override string GetRequestDescription(OrganizationRequest request)
        {
            Guid id = Guid.Empty;
            string state = mode == SetStateMode.Fixed ? State.Name : "<State>";
            string status = mode == SetStateMode.Fixed ? Status.Name : "<Status>";

            if (request is UpdateRequest updateRequest)
            {
                id = updateRequest.Target.Id;
            }
            else if (request is SetStateRequest setStateRequest)
            {
                id = setStateRequest.EntityMoniker.Id;
            }
            else if (request is CloseIncidentRequest closeIncidentRequest)
            {
                id = ((EntityReference)closeIncidentRequest.IncidentResolution[CASE_RESOLUTION_INCIDENT_ID]).Id;
                state = "Resolved";

                EntityMetadata metadata = Dynamics365Entity.Create(CASE_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                StatusAttributeMetadata statusMetadata = ((StatusAttributeMetadata)metadata.Attributes.First(field => field.LogicalName == STATUSCODE_NAME));
                status = statusMetadata.OptionSet.Options.First(option => option.Value == closeIncidentRequest.Status.Value).Label.UserLocalizedLabel.Label;
            }
            else if (request is ReviseQuoteRequest reviseQuoteRequest)
            {
                id = reviseQuoteRequest.QuoteId;
                state = "Draft";
                status = "In Progress";
            }
            else if (request is WinQuoteRequest winQuoteRequest)
            {
                id = ((EntityReference)winQuoteRequest.QuoteClose[QUOTE_CLOSE_QUOTE_ID]).Id;
                state = "Won";
                status = "Won";
            }
            else if (request is CloseQuoteRequest closeQuoteRequest)
            {
                id = ((EntityReference)closeQuoteRequest.QuoteClose[QUOTE_CLOSE_QUOTE_ID]).Id;
                state = "Closed";

                EntityMetadata metadata = Dynamics365Entity.Create(QUOTE_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                StatusAttributeMetadata statusMetadata = ((StatusAttributeMetadata)metadata.Attributes.First(field => field.LogicalName == STATUSCODE_NAME));
                status = statusMetadata.OptionSet.Options.First(option => option.Value == closeQuoteRequest.Status.Value).Label.UserLocalizedLabel.Label;
            }
            else if (request is WinOpportunityRequest winOpportunityRequest)
            {
                id = ((EntityReference)winOpportunityRequest.OpportunityClose[OPPORTUNITY_CLOSE_OPPORTUNITY_ID]).Id;
                state = "Won";

                EntityMetadata metadata = Dynamics365Entity.Create(OPPORTUNITY_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                StatusAttributeMetadata statusMetadata = ((StatusAttributeMetadata)metadata.Attributes.First(field => field.LogicalName == STATUSCODE_NAME));
                status = statusMetadata.OptionSet.Options.First(option => option.Value == winOpportunityRequest.Status.Value).Label.UserLocalizedLabel.Label;
            }
            else if (request is LoseOpportunityRequest loseOpportunityRequest)
            {
                id = ((EntityReference)loseOpportunityRequest.OpportunityClose[OPPORTUNITY_CLOSE_OPPORTUNITY_ID]).Id;
                state = "Lost";

                EntityMetadata metadata = Dynamics365Entity.Create(OPPORTUNITY_ENTITY_NAME, Connection).GetEntityMetadata(Connection);
                StatusAttributeMetadata statusMetadata = ((StatusAttributeMetadata)metadata.Attributes.First(field => field.LogicalName == STATUSCODE_NAME));
                status = statusMetadata.OptionSet.Options.First(option => option.Value == loseOpportunityRequest.Status.Value).Label.UserLocalizedLabel.Label;
            }

            return string.Format(Properties.Resources.Dynamics365SetStateOperationRequestDescription, Entity.DisplayName, id, state, status);
        }

        public override void UpdateParentReferences(Batch parentBatch)
        {
            base.UpdateParentReferences(parentBatch);
            values.ToList().ForEach(fieldValue => fieldValue.UpdateParentReferences(this));
        }

        public override IOperation Clone(bool addSuffix)
        {
            Dynamics365SetStateOperation clone = (Dynamics365SetStateOperation)base.Clone(addSuffix);
            clone.Values = new BindingList<FieldValue>();
            Values.ToList().ForEach(value => clone.Values.Add(value.Clone()));
            return clone;
        }

        #region Interface Methods

        public List<Dynamics365State> GetStates()
        {
            List<Dynamics365State> states = new List<Dynamics365State>();

            try
            {
                states.AddRange(Dynamics365State.GetStates(Entity, Connection));
            }
            catch
            { }

            return states;
        }

        public List<Dynamics365Status> GetStatuses()
        {
            List<Dynamics365Status> statuses = new List<Dynamics365Status>();

            try
            {
                statuses.AddRange(State.Statuses);
            }
            catch
            { }

            return statuses;
        }

        public List<Field> GetDynamics365SetStateDataDestinationFields()
        {
            List<Field> fields = new List<Field>();
            try
            {
                List<Dynamics365State> states = Dynamics365State.GetStates(Entity, Connection);

                if (states?.Count > 0)
                {
                    // add state and status field from destination entity
                    Dynamics365State state = states[0];
                    Dynamics365Status status = state.Statuses[0];
                    fields.AddRange(Entity.GetFields(Connection).Where(field => ((Dynamics365Field)field).LogicalName == state.LogicalName || ((Dynamics365Field)field).LogicalName == status.LogicalName));
                }

                if (Entity.LogicalName == CASE_ENTITY_NAME)
                {
                    Dynamics365Entity incidentResolution = Dynamics365Entity.Create(CASE_RESOLUTION_ENTITY_NAME, Connection);
                    fields.AddRange(incidentResolution.GetFields(Connection).Where(field => ((Dynamics365Field)field).LogicalName == "subject" || ((Dynamics365Field)field).LogicalName == "description"));
                }
                else if (Entity.LogicalName == QUOTE_ENTITY_NAME)
                {
                    Dynamics365Entity quoteClose = Dynamics365Entity.Create(CASE_RESOLUTION_ENTITY_NAME, Connection);
                    fields.AddRange(quoteClose.GetFields(Connection).Where(field => ((Dynamics365Field)field).LogicalName == "subject" || ((Dynamics365Field)field).LogicalName == "description"));
                }
                else if (Entity.LogicalName == OPPORTUNITY_ENTITY_NAME)
                {
                    Dynamics365Entity opportunityClose = Dynamics365Entity.Create(CASE_RESOLUTION_ENTITY_NAME, Connection);
                    fields.AddRange(opportunityClose.GetFields(Connection).Where(field => ((Dynamics365Field)field).LogicalName == "subject" || ((Dynamics365Field)field).LogicalName == "description" || ((Dynamics365Field)field).LogicalName == "actualend" || ((Dynamics365Field)field).LogicalName == "actualrevenue"));
                }

                // todo - check this is correct. need to be able to support multiple values for 'first non-null value' logic
                //foreach (FieldValue value in values)
                //{
                //    // prevent fields from being selected more than once
                //    fields.RemoveAll(field => field == value.DestinationField);
                //}
            }
            catch { }

            return fields;
        }

        public FieldValue CreateFieldValue(Type type)
        {
            return (FieldValue)Activator.CreateInstance(type, new object[] { this });
        }

        #endregion
    }

    // todo - put in central class with other entity details
    public class StateCode
    {
        public static readonly int CaseResolved = 1;
        public static readonly int QuoteDraft = 0;
        public static readonly int QuoteWon = 2;
        public static readonly int QuoteClosed = 3;
        public static readonly int OpportunityWon = 1;
        public static readonly int OpportunityLost = 2;
    }

    public enum SetStateMode
    {
        Fixed,
        Variable
    }
}