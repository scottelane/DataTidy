using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using ScottLane.DataTidy.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Gets a value from a Dynamics 365 data source.
    /// </summary>
    public class Dynamics365DataSourceValue : DataSourceValue, IDynamics365TargetEntitiesProvider
    {
        private const TargetMode DEFAULT_TARGET_MODE = TargetMode.Fixed;

        [GlobalisedDisplayName(typeof(Dynamics365DataSourceValue), nameof(DestinationField)), GlobalisedDecription(typeof(Dynamics365DataSourceValue), nameof(DestinationField)), TypeConverter(typeof(Dynamics365FieldConverter)), JsonProperty(Order = 1)]
        public override Field DestinationField
        {
            get { return base.DestinationField; }
            set
            {
                base.DestinationField = value;
                RefreshBrowsableFields();
            }
        }

        private TargetMode targetMode = DEFAULT_TARGET_MODE;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Target Mode"), GlobalisedDecription("Indicates whether the Target Entity will be used for all records, or whether the entity varies based on the entity logical name specified in the Target Entity Field."), DefaultValue(DEFAULT_TARGET_MODE), Browsable(true), JsonProperty(Order = 2)]
        public TargetMode TargetMode
        {
            get { return targetMode; }
            set
            {
                if (targetMode != value)
                {
                    targetMode = value;
                    RefreshBrowsableFields();
                    OnPropertyChanged(nameof(TargetMode));
                }
            }
        }

        private Dynamics365Entity targetEntity;

        /// <summary>
        /// Gets or sets the entity that will be looked up if the destination field is a multi-target lookup.
        /// </summary>
        [GlobalisedCategory("General"), GlobalisedDisplayName("Target Entity"), GlobalisedDecription("The lookup target entity."), TypeConverter(typeof(Dynamics365TargetEntityConverter)), Browsable(true), JsonProperty(Order = 3)]
        public Dynamics365Entity TargetEntity
        {
            get { return targetEntity; }
            set
            {
                if (targetEntity != value)
                {
                    targetEntity = value;
                    OnPropertyChanged(nameof(TargetEntity));
                }
            }
        }

        private Field targetEntityField;

        [GlobalisedCategory("General"), GlobalisedDisplayName("Target Entity Field"), GlobalisedDecription("The data source field containing the logical name of the lookup target entity."), TypeConverter(typeof(DataSourceFieldConverter)), Browsable(false), JsonProperty(Order = 4)]
        public Field TargetEntityField
        {
            get { return targetEntityField; }
            set
            {
                if (targetEntityField != value)
                {
                    targetEntityField = value;
                    OnPropertyChanged(nameof(TargetEntityField));
                }
            }
        }


        /// <summary>
        /// Initialises a new instance of the Dynamics365DataSourceValue class with the specified parent operation.
        /// </summary>
        /// <param name="parentOperation">The parent operation.</param>
        public Dynamics365DataSourceValue(IOperation parentOperation) : base(parentOperation)
        { }

        protected override void RefreshBrowsableFields()
        {
            base.RefreshBrowsableFields();
            CoreUtility.SetBrowsable(this, nameof(TargetMode), ((Dynamics365Field)DestinationField)?.Targets?.Length > 1);
            CoreUtility.SetBrowsable(this, nameof(TargetEntity), ((Dynamics365Field)DestinationField)?.Targets?.Length > 1 && targetMode == TargetMode.Fixed);
            CoreUtility.SetBrowsable(this, nameof(TargetEntityField), ((Dynamics365Field)DestinationField)?.Targets?.Length > 1 && targetMode == TargetMode.Variable);
        }

        public override object GetValue(DataRow row, CancellationToken cancel, IProgress<ExecutionProgress> progress)
        {
            object value = base.GetValue(row, cancel, progress);
            string targetEntityLogicalName = default(string);

            if (((Dynamics365Field)DestinationField)?.Targets?.Length > 1)
            {
                if (targetMode == TargetMode.Fixed && TargetEntity != default(Dynamics365Entity))
                {
                    targetEntityLogicalName = TargetEntity.LogicalName;
                }
                else if (targetMode == TargetMode.Variable && TargetEntityField != default(DataTableField))
                {
                    targetEntityLogicalName = row[((DataTableField)targetEntityField).ColumnName].ToString();
                }
            }

            return Dynamics365TypeConverter.Convert(value, ((Dynamics365RecordOperation)Parent).Entity, ((Dynamics365Field)DestinationField).LogicalName, ((Dynamics365RecordOperation)Parent).Connection, targetEntityLogicalName);
        }

        public override ValidationResult Validate()
        {
            ValidationResult result = base.Validate();
            result.AddErrorIf(((Dynamics365Field)DestinationField)?.Targets?.Length > 1 && targetMode == TargetMode.Fixed && TargetEntity == default(Dynamics365Entity), string.Format("Please select a Target Entity for the '{0}' field value mapping", DestinationField?.DisplayName ?? "<Unknown>"));
            result.AddErrorIf(((Dynamics365Field)DestinationField)?.Targets?.Length > 1 && targetMode == TargetMode.Variable && TargetEntityField == default(Field), string.Format("Please select a Target Entity Field for the '{0}' field value mapping", DestinationField?.DisplayName ?? "<Unknown>"));
            return result;
        }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The destination field display name.</returns>
        public override string ToString()
        {
            return DestinationField?.DisplayName ?? string.Empty;
        }

        public List<Dynamics365Entity> GetTargetEntities()
        {
            List<Dynamics365Entity> targetEntities = new List<Dynamics365Entity>();

            try
            {
                // todo - this should only display on the form for fields that are lookups. it's not tied in any way to the dynamics365lookupvalue
                if (((Dynamics365Field)DestinationField).Targets != default(string[]))
                {
                    foreach (string target in ((Dynamics365Field)DestinationField).Targets)
                    {
                        targetEntities.Add(Dynamics365Entity.Create(target, ((Dynamics365RecordOperation)Parent).Connection));
                    }
                }
            }
            catch
            { }

            return targetEntities;
        }
    }

    public enum TargetMode
    {
        Fixed,
        Variable
    }
}
