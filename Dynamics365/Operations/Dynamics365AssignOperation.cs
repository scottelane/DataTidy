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
using Newtonsoft.Json;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Assigns Dynamics 365 records to a user or team.
    /// </summary>
    [Operation(typeof(Dynamics365AssignOperation), "ScottLane.DataTidy.Dynamics365.Resources.Dynamics365AssignOperation.png")]
    public class Dynamics365AssignOperation : Dynamics365RecordOperation, IDynamics365UserOrTeamOwnedEntitiesProvider, IDynamics365TeamsProvider, IDynamics365UsersProvider, INotifyPropertyChanged, IDynamics365AssignDataDestinationFieldsProvider, IFieldValueCreator
    {
        private const string OWNER_FIELD_LOGICAL_NAME = "ownerid";
        private const string USER_ENTITY_LOGICAL_NAME = "systemuser";
        private const OwnerType DEFAULT_OWNER_TYPE = OwnerType.User;
        private const AssignMode DEFAULT_ASSIGN_MODE = AssignMode.Fixed;

        #region Properties

        private AssignMode mode = DEFAULT_ASSIGN_MODE;

        [GlobalisedCategory("Assign"), GlobalisedDisplayName("Mode"), GlobalisedDecription("If Fixed, a single user or team can be set for all records. If Variable, different users or teams can be set for each record."), DefaultValue(DEFAULT_ASSIGN_MODE)]
        public AssignMode Mode
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

        /// <summary>
        /// Gets or sets the source of the target record identifier.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(TargetSource)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(TargetSource)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(TargetSource)), DefaultValue(DEFAULT_TARGET_SOURCE)]
        public override TargetSource TargetSource
        {
            get { return base.TargetSource; }
            set { base.TargetSource = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the record targeted by the operation.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(Target)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(Target)), GlobalisedDecription(typeof(Dynamics365RecordOperation), nameof(Target)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public override FieldValue Target
        {
            get { return base.Target; }
            set { base.Target = value; }
        }

        /// <summary>
        /// Gets or sets the data source to read records from.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(DataSource)), GlobalisedDecription(typeof(Dynamics365AssignOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter))]
        public override IDataSource DataSource
        {
            get { return base.DataSource; }
            set { base.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets a connection to the Dynamics 365 organisation to assign records in.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(Connection)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(Connection)), GlobalisedDecription(typeof(Dynamics365AssignOperation), nameof(Connection)), TypeConverter(typeof(ConnectionConverter)), JsonProperty(Order = 1)]
        public override Dynamics365Connection Connection
        {
            get { return base.Connection; }
            set { base.Connection = value; }
        }

        /// <summary>
        /// Gets or sets the Dynamics365Entity containing records to assign.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(Entity)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(Entity)), GlobalisedDecription(typeof(Dynamics365AssignOperation), nameof(Entity)), TypeConverter(typeof(Dynamics365UserOrTeamOwnedEntityConverter)), JsonProperty(Order = 2)]
        public override Dynamics365Entity Entity
        {
            get { return base.Entity; }
            set { base.Entity = value; }
        }

        private OwnerType ownerType = DEFAULT_OWNER_TYPE;

        /// <summary>
        /// Gets or sets the type of owner to assign records to.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(OwnerType)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(OwnerType)), GlobalisedDecription(typeof(Dynamics365AssignOperation), nameof(OwnerType)), Browsable(true), DefaultValue(DEFAULT_OWNER_TYPE)]
        public OwnerType OwnerType
        {
            get { return ownerType; }
            set
            {
                if (ownerType != value)
                {
                    ownerType = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(OwnerType));
                    RefreshName();
                }
            }
        }

        private Dynamics365Team team;

        /// <summary>
        /// Gets or sets the Dynamics365Team to assign as the record owner.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(Team)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(Team)), GlobalisedDecription(typeof(Dynamics365AssignOperation), nameof(Team)), TypeConverter(typeof(Dynamics365TeamConverter)), Browsable(true), JsonProperty(Order = 3)]
        public Dynamics365Team Team
        {
            get { return team; }
            set
            {
                if (team != value)
                {
                    team = value;
                    OnPropertyChanged(nameof(Team));
                    RefreshName();
                }
            }
        }

        private Dynamics365User user;

        /// <summary>
        /// Gets or sets the Dynamics365User to assign as the record owner.
        /// </summary>
        [GlobalisedCategory(typeof(Dynamics365AssignOperation), nameof(User)), GlobalisedDisplayName(typeof(Dynamics365AssignOperation), nameof(User)), GlobalisedDecription(typeof(Dynamics365AssignOperation), nameof(User)), TypeConverter(typeof(Dynamics365UserConverter)), Browsable(true), JsonProperty(Order = 3)]
        public Dynamics365User User
        {
            get { return user; }
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged(nameof(User));
                    RefreshName();
                }
            }
        }

        protected BindingList<FieldValue> owner = new BindingList<FieldValue>();

        [GlobalisedCategory("Assign"), GlobalisedDisplayName("Owner"), GlobalisedDecription("The user or team to assign records to."), Editor(typeof(Dynamics365AssignFieldValueCollectionEditor), typeof(UITypeEditor)), Browsable(true)]
        public virtual BindingList<FieldValue> Owner
        {
            get { return owner; }
            set
            {
                if (owner != value)
                {
                    owner = value;
                    OnPropertyChanged(nameof(Owner));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365AssignOperation class with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365AssignOperation(Batch parentBatch) : base(parentBatch)
        {
            RefreshBrowsableFields();
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(OwnerType), Mode == AssignMode.Fixed);
            CoreUtility.SetBrowsable(this, nameof(Team), Mode == AssignMode.Fixed && OwnerType == OwnerType.Team);
            CoreUtility.SetBrowsable(this, nameof(User), Mode == AssignMode.Fixed && OwnerType == OwnerType.User);
            CoreUtility.SetBrowsable(this, nameof(Owner), Mode == AssignMode.Variable);
        }

        /// <summary>
        /// Generates a friendly name for the operation.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            string owner = Mode == AssignMode.Fixed && OwnerType == OwnerType.User ? User?.Name ?? Properties.Resources.Dynamics365AssignOperationFriendlyNameUser : Mode == AssignMode.Fixed && OwnerType == OwnerType.Team ? Team?.Name ?? Properties.Resources.Dynamics365AssignOperationFriendlyNameTeam : Properties.Resources.Dynamics365AssignOperationFriendlyNameUnknown;

            return string.Format(Properties.Resources.Dynamics365AssignOperationFriendlyName,
                Entity?.PluralName ?? Properties.Resources.Dynamics365AssignOperationFriendlyNameEntity,
                owner,
                DataSource?.Name ?? Properties.Resources.Dynamics365AssignOperationFriendlyNameDataSource);
        }

        /// <summary>
        /// Validates the operation.
        /// </summary>
        /// <returns>The ValidationResult.</returns>
        public override Core.ValidationResult Validate()
        {
            Core.ValidationResult result = base.Validate();
            result.AddErrorIf(Mode == AssignMode.Fixed && OwnerType == OwnerType.User && User == default(Dynamics365User), Properties.Resources.Dynamics365AssignOperationValidateUser, nameof(User));
            result.AddErrorIf(Mode == AssignMode.Fixed && OwnerType == OwnerType.Team && Team == default(Dynamics365Team), Properties.Resources.Dynamics365AssignOperationValidateTeam, nameof(Team));

            List<ValidationError> valueErrors = new List<ValidationError>();

            foreach (FieldValue value in owner)
            {
                valueErrors.AddRange(value.Validate().Errors);
            }

            result.Errors.AddRange(valueErrors);

            if (valueErrors.Count == 0)
            {
                result.AddErrorIf(Mode == AssignMode.Variable && Owner.Count(fieldValue => ((Dynamics365Field)fieldValue.DestinationField).LogicalName == OWNER_FIELD_LOGICAL_NAME) == 0, "Please specify at least one value for the Owner field.", nameof(Owner));
            }

            return result;
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

            if (Connection.IsVersionOrAbove(CrmVersion.Crm2016))
            {
                UpdateRequest request = new UpdateRequest()
                {
                    Target = GetTargetEntity(row, cancel, progress)
                };

                if (Mode == AssignMode.Fixed)
                {
                    request.Target.Attributes[OWNER_FIELD_LOGICAL_NAME] = User != null ? User.ToEntityReference() : Team.ToEntityReference();
                }
                else
                {
                    PopulateEntityFromDataRow(request.Target, row, cancel, progress);
                }

                requests.Add(request);
            }
            else
            {
                requests.Add(new AssignRequest()
                {
                    Assignee = User != null ? User.ToEntityReference() : Team.ToEntityReference(),
                    Target = GetTargetEntity(row, cancel, progress).ToEntityReference()
                });
            }

            return requests;
        }

        private void PopulateEntityFromDataRow(Entity entity, DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            foreach (FieldValue value in Owner)
            {
                Dynamics365FieldOperation.PopulateEntityFromFieldValue(entity, value, row, cancel, progress);
            }
        }

        /// <summary>
        /// Gets a description of the specified request. 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The request description.</returns>
        protected override string GetRequestDescription(OrganizationRequest request)
        {
            string requestDescription = string.Empty;
            string owner = Mode == AssignMode.Fixed && OwnerType == OwnerType.User ? User.Name : Mode == AssignMode.Fixed && OwnerType == OwnerType.Team ? Team.Name : Properties.Resources.Dynamics365AssignOperationFriendlyNameUnknown;

            if (request is UpdateRequest updateRequest)
            {
                if (Mode == AssignMode.Variable)
                {
                    EntityReference reference = (EntityReference)updateRequest.Target.Attributes[OWNER_FIELD_LOGICAL_NAME];
                    owner = string.Format("{0} {1}", reference.LogicalName == USER_ENTITY_LOGICAL_NAME ? "User" : "Team", reference.Id.ToString());
                }

                requestDescription = string.Format(Properties.Resources.Dynamics365AssignOperationRequestDescription, Entity.DisplayName, updateRequest.Target.Id, owner);
            }
            else if (request is AssignRequest assignRequest)
            {
                requestDescription = string.Format(Properties.Resources.Dynamics365AssignOperationRequestDescription, Entity.DisplayName, assignRequest.Target.Id, owner);
            }

            return requestDescription;
        }

        public override void UpdateParentReferences(Batch parentBatch)
        {
            base.UpdateParentReferences(parentBatch);
            owner.ToList().ForEach(fieldValue => fieldValue.UpdateParentReferences(this));
        }

        public override IOperation Clone(bool addSuffix)
        {
            Dynamics365AssignOperation clone = (Dynamics365AssignOperation)base.Clone(addSuffix);
            clone.Owner = new BindingList<FieldValue>();
            Owner.ToList().ForEach(owner => clone.Owner.Add(owner.Clone()));
            return clone;
        }

        #region Interface methods

        /// <summary>
        /// Gets a list of entities that are owned by a user or team.
        /// </summary>
        /// <returns>A list of Dynamics365Entity objects.</returns>
        public List<Dynamics365Entity> GetUserOrTeamOwnedEntities()
        {
            List<Dynamics365Entity> entities = new List<Dynamics365Entity>();

            try
            {
                entities.AddRange(Dynamics365Entity.GetEntities(Connection).Where(entity => entity.IsUserTeamOwned).ToList());
            }
            catch { }

            return entities;
        }

        /// <summary>
        /// Gets a list of Dynamics 365 teams.
        /// </summary>
        /// <returns>A list of Dynamics365Team objects.</returns>
        public List<Dynamics365Team> GetTeams()
        {
            List<Dynamics365Team> teams = new List<Dynamics365Team>();

            try
            {
                teams.AddRange(Dynamics365Team.GetTeams(Connection).ToList());
            }
            catch { }

            return teams;
        }

        /// <summary>
        /// Gets a list of Dynamics 365 users.
        /// </summary>
        /// <returns>A list of DynamcisCrmUser objects.</returns>
        public List<Dynamics365User> GetUsers()
        {
            List<Dynamics365User> users = new List<Dynamics365User>();

            try
            {
                users.AddRange(Dynamics365User.GetUsers(Connection).ToList());
            }
            catch { }

            return users;
        }

        public FieldValue CreateFieldValue(Type type)
        {
            return (FieldValue)Activator.CreateInstance(type, new object[] { this });
        }

        public List<Field> GetDynamics365AssignDataDestinationFields()
        {
            List<Field> fields = new List<Field>();
            try
            {
                fields.AddRange(Entity.GetFields(Connection).Where(field => ((Dynamics365Field)field).LogicalName == OWNER_FIELD_LOGICAL_NAME));
            }
            catch { }

            return fields;
        }

        #endregion
    }

    /// <summary>
    /// Defines the types of entity ownership.
    /// </summary>
    public enum OwnerType
    {
        Team,
        User
    }

    public enum AssignMode
    {
        Fixed,
        Variable
    }
}