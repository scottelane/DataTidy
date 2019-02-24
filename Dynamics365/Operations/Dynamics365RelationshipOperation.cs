using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using Microsoft.Xrm.Sdk;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Base class for relationship-oriented Dynamics 365 operations.
    /// </summary>
    public abstract class Dynamics365RelationshipOperation : Dynamics365RecordOperation, IDynamics365RelationshipsProvider
    {
        private const TargetSource DEFAULT_ASSOCIATED_SOURCE = TargetSource.DataSourceValue;

        protected Dynamics365Relationship relationship;

        [GlobalisedCategory(typeof(Dynamics365RelationshipOperation), nameof(Relationship)), GlobalisedDisplayName(typeof(Dynamics365RelationshipOperation), nameof(Relationship)), GlobalisedDecription(typeof(Dynamics365RelationshipOperation), nameof(Relationship)), TypeConverter(typeof(Dynamics365RelationshipConverter))]
        public virtual Dynamics365Relationship Relationship
        {
            get { return relationship; }
            set
            {
                if (relationship != value)
                {
                    relationship = value;
                    OnPropertyChanged(nameof(Relationship));
                }
            }
        }

        protected Dynamics365Entity associatedEntity;

        [GlobalisedCategory(typeof(Dynamics365RelationshipOperation), nameof(AssociatedEntity)), GlobalisedDisplayName(typeof(Dynamics365RelationshipOperation), nameof(AssociatedEntity)), GlobalisedDecription(typeof(Dynamics365RelationshipOperation), nameof(AssociatedEntity)), TypeConverter(typeof(Dynamics365EntityConverter))]
        public virtual Dynamics365Entity AssociatedEntity
        {
            get { return associatedEntity; }
            set
            {
                if (associatedEntity != value)
                {
                    associatedEntity = value;
                    OnPropertyChanged(nameof(AssociatedEntity));
                }
            }
        }

        protected TargetSource associatedSource;

        [GlobalisedCategory(typeof(Dynamics365RelationshipOperation), nameof(AssociatedSource)), GlobalisedDisplayName(typeof(Dynamics365RelationshipOperation), nameof(AssociatedSource)), GlobalisedDecription(typeof(Dynamics365RelationshipOperation), nameof(AssociatedSource)), DefaultValue(DEFAULT_ASSOCIATED_SOURCE)]
        public virtual TargetSource AssociatedSource
        {
            get { return associatedSource; }
            set
            {
                SetAssociatedSource(value);
            }
        }

        private void SetAssociatedSource(TargetSource value)
        {
            if (associatedSource != value)
            {
                associatedSource = value;

                if (associatedSource == TargetSource.DataSourceValue)
                {
                    AssociatedTarget = new Dynamics365DataSourceValue(this);
                }
                else if (associatedSource == TargetSource.LookupValue)
                {
                    AssociatedTarget = new Dynamics365LookupValue(this);
                }
                else if (associatedSource == TargetSource.UserProvidedValue)
                {
                    AssociatedTarget = new Dynamics365UserProvidedValue(this);
                }
                else
                {
                    AssociatedTarget = default(FieldValue);
                }

                RefreshBrowsableFields();
                OnPropertyChanged(nameof(AssociatedSource));
            }
        }

        protected FieldValue associatedTarget;

        [GlobalisedCategory(typeof(Dynamics365RelationshipOperation), nameof(AssociatedTarget)), GlobalisedDisplayName(typeof(Dynamics365RelationshipOperation), nameof(AssociatedTarget)), GlobalisedDecription(typeof(Dynamics365RelationshipOperation), nameof(AssociatedTarget)), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
        public virtual FieldValue AssociatedTarget
        {
            get { return associatedTarget; }
            set
            {
                if (associatedTarget != value)
                {
                    associatedTarget = value;
                    OnPropertyChanged(nameof(AssociatedTarget));
                }
            }
        }

        /// <summary>
        /// Initialises a new instance of the Dynamics365RelationshipOperation with the specified parent batch.
        /// </summary>
        /// <param name="parentBatch">The parent batch.</param>
        public Dynamics365RelationshipOperation(Batch parentBatch) : base(parentBatch)
        {
            SetAssociatedSource(DEFAULT_ASSOCIATED_SOURCE);
        }

        private void RefreshBrowsableFields()
        {
            CoreUtility.SetBrowsable(this, nameof(AssociatedTarget), (associatedSource == TargetSource.DataSourceValue || associatedSource == TargetSource.LookupValue || associatedSource == TargetSource.UserProvidedValue));
        }

        /// <summary>
        /// Validates the operation.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            try
            {
                result.AddErrorIf(Relationship == default(Dynamics365Relationship), Properties.Resources.Dynamics365RelationshipOperationValidateRelationship, nameof(Relationship));
                result.AddErrorIf(AssociatedEntity == default(Dynamics365Entity), Properties.Resources.Dynamics365RelationshipOperationValidateAssociatedEntity, nameof(AssociatedEntity));
                result.AddErrorIf(AssociatedSource == default(TargetSource), Properties.Resources.Dynamics365RelationshipOperationValidateAssociatedSource, nameof(AssociatedSource));
                result.AddErrorIf(AssociatedTarget == default(FieldValue), Properties.Resources.Dynamics365RelationshipOperationValidateAssociatedRecord, nameof(AssociatedTarget));
                if (AssociatedTarget != default(FieldValue)) result.Errors.AddRange(AssociatedTarget.Validate().Errors);
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        protected virtual Entity GetAssociatedEntity(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            Entity associatedEntity = new Entity(AssociatedEntity.LogicalName);

            if (AssociatedSource != TargetSource.None)
            {
                associatedEntity.Id = ((EntityReference)AssociatedTarget.GetValue(row, cancel, progress)).Id;
            }

            return associatedEntity;
        }

        public override IOperation Clone(bool addSuffix)
        {
            Dynamics365RelationshipOperation clone = (Dynamics365RelationshipOperation)base.Clone(addSuffix);
            clone.AssociatedTarget = AssociatedTarget?.Clone();
            return clone;
        }

        public override void UpdateParentReferences(Batch parentBatch)
        {
            base.UpdateParentReferences(parentBatch);

            if (AssociatedTarget != default(FieldValue))
            {
                AssociatedTarget.UpdateParentReferences(this);
            }
        }

        /// <summary>
        /// Gets all relationships for the specified entity.
        /// </summary>
        /// <returns>The relationships.</returns>
        public List<Dynamics365Relationship> GetRelationships()
        {
            List<Dynamics365Relationship> relationships = new List<Dynamics365Relationship>();

            try
            {
                relationships.AddRange(Dynamics365Relationship.GetRelationships(Entity, Connection).Where(r => r.RelatedEntityLogicalName == AssociatedEntity.LogicalName).ToList());
            }
            catch { }

            return relationships;
        }
    }
}
