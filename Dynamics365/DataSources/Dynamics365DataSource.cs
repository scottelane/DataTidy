using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Base class for all Dynamics 365 data sources.
    /// </summary>
    public abstract class Dynamics365DataSource : DataSource, IDynamics365EntitiesProvider
    {
        private const LookupBehaviour DEFAULT_LOOKUP_BEHAVIOUR = LookupBehaviour.EntityAndNameAndIdentifier;
        private const OptionSetBehaviour DEFAULT_OPTIONSET_BEHAVIOUR = OptionSetBehaviour.NameAndCode;
        private const DataTypes DEFAULT_DATA_TYPE_BEHAVIOUR = DataTypes.Neutral;
        public const string PARTYLIST_DELIMITER = ";";

        #region Properties

        public LookupBehaviour lookupBehaviour = DEFAULT_LOOKUP_BEHAVIOUR;

        /// <summary>
        /// Gets or sets a value indicating how lookups are presented in data.
        /// </summary>
        [GlobalisedCategory("Behaviour"), GlobalisedDisplayName("Lookups"), GlobalisedDecription("Determines what values are returned for a lookup when the Neutral Data Type is used."), DefaultValue(DEFAULT_LOOKUP_BEHAVIOUR), Browsable(true)]
        public LookupBehaviour LookupBehaviour
        {
            get { return lookupBehaviour; }
            set
            {
                if (lookupBehaviour != value)
                {
                    lookupBehaviour = value;
                    OnPropertyChanged(nameof(LookupBehaviour));
                }
            }
        }

        public OptionSetBehaviour optionSetBehaviour = DEFAULT_OPTIONSET_BEHAVIOUR;

        /// <summary>
        /// Gets or sets a value indicating how option sets are presented in data.
        /// </summary>
        [GlobalisedCategory("Behaviour"), GlobalisedDisplayName("Option Sets"), GlobalisedDecription("Determines what values are returned for an option set when the Neutral Data Type is used."), DefaultValue(DEFAULT_OPTIONSET_BEHAVIOUR), Browsable(true)]
        public OptionSetBehaviour OptionSetBehaviour
        {
            get { return optionSetBehaviour; }
            set
            {
                if (optionSetBehaviour != value)
                {
                    optionSetBehaviour = value;
                    OnPropertyChanged(nameof(OptionSetBehaviour));
                }
            }
        }

        public DataTypes dataTypes = DEFAULT_DATA_TYPE_BEHAVIOUR;

        /// <summary>
        /// Gets or sets a value indicating whether returned values will remain in their original value or be converted to a neutral data type.
        /// </summary>
        [GlobalisedCategory("Behaviour"), GlobalisedDisplayName("Data Types"), GlobalisedDecription("Determines whether values will remain in their original value or be converted to a neutral data type."), DefaultValue(DEFAULT_DATA_TYPE_BEHAVIOUR), Browsable(true)]
        public DataTypes DataTypes
        {
            get { return dataTypes; }
            set
            {
                if (dataTypes != value)
                {
                    dataTypes = value;
                    OnPropertyChanged(nameof(DataTypes));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the Dynamics365DataSource class with the specified Connection.
        /// </summary>
        /// <param name="connection">The Connection.</param>
        public Dynamics365DataSource(IConnection connection) : base(connection)
        { }

        /// <summary>
        /// Creates an empty DataTable for the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>The data table.</returns>
        protected DataTable CreateEmptyDataTable(List<Dynamics365Field> fields)
        {
            DataTable dataTable = new DataTable(Name);

            foreach (Dynamics365Field field in fields)
            {
                EntityMetadata entityMetadata = Dynamics365Entity.Create(field.EntityLogicalName, (Dynamics365Connection)Parent).GetEntityMetadata((Dynamics365Connection)Parent);
                AttributeMetadata attributeMetadata = entityMetadata.Attributes.FirstOrDefault(findField => findField.LogicalName == field.LogicalName);

                if (attributeMetadata != default(AttributeMetadata))
                {
                    AddDataColumns(dataTable, field, attributeMetadata);
                }
                else
                {
                    throw new ApplicationException(string.Format("The field {0} could not be found in {1}", field.LogicalName, entityMetadata.LogicalName));
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Adds DataColumns to the specified dataTable for the specified field.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="field">The field.</param>
        /// <param name="attributeMetadata">The field attribute metadata.</param>
        protected void AddDataColumns(DataTable dataTable, Dynamics365Field field, AttributeMetadata attributeMetadata)
        {
            Dynamics365TypeConverter converter = new Dynamics365TypeConverter(attributeMetadata);

            if (DataTypes == DataTypes.Native)
            {
                DataColumn nativeColumn = new DataColumn()
                {
                    ColumnName = string.Format("{0}.{1}", field.EntityLogicalName, field.LogicalName),
                    Caption = field.DisplayName,
                    DataType = converter.Dynamics365Type
                };

                if (!dataTable.Columns.Contains(nativeColumn.ColumnName))
                {
                    dataTable.Columns.Add(nativeColumn);
                }
            }
            else if (DataTypes == DataTypes.Neutral)
            {
                if (converter.Dynamics365Type == typeof(EntityReference) || converter.Dynamics365Type == typeof(EntityCollection))
                {
                    if (LookupBehaviour == LookupBehaviour.Name || LookupBehaviour == LookupBehaviour.NameAndIdentifier || LookupBehaviour == LookupBehaviour.EntityAndNameAndIdentifier)
                    {
                        DataColumn nameColumn = new DataColumn()
                        {
                            ColumnName = string.Format("{0}.{1}{2}", field.EntityLogicalName, field.LogicalName, LookupBehaviour == LookupBehaviour.Name ? string.Empty : ".Name"),
                            Caption = string.Format("{0}{1}", field.DisplayName, LookupBehaviour == LookupBehaviour.Name ? string.Empty : " (Name)"),
                            DataType = typeof(string)
                        };

                        if (!dataTable.Columns.Contains(nameColumn.ColumnName)) // todo - fetch xml view on team entity caused duplication - investigate why
                        {
                            dataTable.Columns.Add(nameColumn);
                        }
                    }

                    if (LookupBehaviour == LookupBehaviour.Identifier || LookupBehaviour == LookupBehaviour.NameAndIdentifier || LookupBehaviour == LookupBehaviour.EntityAndNameAndIdentifier)
                    {
                        DataColumn identifierColumn = new DataColumn()
                        {
                            ColumnName = string.Format("{0}.{1}{2}", field.EntityLogicalName, field.LogicalName, LookupBehaviour == LookupBehaviour.Identifier ? string.Empty : ".Identifier"),
                            Caption = string.Format("{0}{1}", field.DisplayName, LookupBehaviour == LookupBehaviour.Identifier ? string.Empty : " (Identifier)"),
                            DataType = converter.Dynamics365Type == typeof(EntityReference) ? typeof(Guid) : typeof(string)
                        };
                        dataTable.Columns.Add(identifierColumn);
                    }

                    if (LookupBehaviour == LookupBehaviour.Entity || LookupBehaviour == LookupBehaviour.EntityAndIdentifier || LookupBehaviour == LookupBehaviour.EntityAndNameAndIdentifier)
                    {
                        DataColumn entityColumn = new DataColumn()
                        {
                            ColumnName = string.Format("{0}.{1}{2}", field.EntityLogicalName, field.LogicalName, LookupBehaviour == LookupBehaviour.Entity ? string.Empty : ".Entity"),
                            Caption = string.Format("{0}{1}", field.DisplayName, LookupBehaviour == LookupBehaviour.Entity ? string.Empty : " (Entity)"),
                            DataType = typeof(string)
                        };

                        if (!dataTable.Columns.Contains(entityColumn.ColumnName))
                        {
                            dataTable.Columns.Add(entityColumn);
                        }
                    }
                }
                else if (converter.Dynamics365Type == typeof(OptionSetValue) || converter.Dynamics365Type == typeof(OptionSetValueCollection))
                {
                    if (OptionSetBehaviour == OptionSetBehaviour.Name || OptionSetBehaviour == OptionSetBehaviour.NameAndCode)
                    {
                        DataColumn nameColumn = new DataColumn()
                        {
                            ColumnName = string.Format("{0}.{1}{2}", field.EntityLogicalName, field.LogicalName, OptionSetBehaviour == OptionSetBehaviour.Name ? string.Empty : ".Name"),
                            Caption = string.Format("{0}{1}", field.DisplayName, OptionSetBehaviour == OptionSetBehaviour.Name ? string.Empty : " (Name)"),
                            DataType = typeof(string)
                        };

                        if (!dataTable.Columns.Contains(nameColumn.ColumnName))
                        {
                            dataTable.Columns.Add(nameColumn);
                        }
                    }

                    if (OptionSetBehaviour == OptionSetBehaviour.Code || OptionSetBehaviour == OptionSetBehaviour.NameAndCode)
                    {
                        DataColumn codeColumn = new DataColumn()
                        {
                            ColumnName = string.Format("{0}.{1}{2}", field.EntityLogicalName, field.LogicalName, OptionSetBehaviour == OptionSetBehaviour.Code ? string.Empty : ".Code"),
                            Caption = string.Format("{0}{1}", field.DisplayName, OptionSetBehaviour == OptionSetBehaviour.Code ? string.Empty : " (Code)"),
                            DataType = converter.Dynamics365Type == typeof(OptionSetValue) ? typeof(int) : typeof(string)
                        };

                        if (!dataTable.Columns.Contains(codeColumn.ColumnName))
                        {
                            dataTable.Columns.Add(codeColumn);
                        }
                    }
                }
                else
                {
                    DataColumn neutralColumn = new DataColumn()
                    {
                        ColumnName = string.Format("{0}.{1}", field.EntityLogicalName, field.LogicalName),
                        Caption = field.DisplayName,
                        DataType = converter.NeutralType
                    };

                    if (!dataTable.Columns.Contains(neutralColumn.ColumnName))
                    {
                        dataTable.Columns.Add(neutralColumn);
                    }
                }
            }
        }

        /// <summary>
        /// Appends rows to the specified data from the specified EntityCollection.
        /// </summary>
        /// <param name="dataTable">The DataTable.</param>
        /// <param name="entities">The EntityCollection.</param>
        /// <returns>A DataTable with appended rows.</returns>
        protected DataTable AppendRows(DataTable dataTable, EntityCollection entities)
        {
            return AppendRows(dataTable, entities, int.MaxValue);
        }

        /// <summary>
        /// Appends rows to the specified data table from the specified EntityCollection.
        /// </summary>
        /// <param name="dataTable">The DataTable.</param>
        /// <param name="entities">The EntityCollection.</param>
        /// <param name="recordLimit">The record limit.</param>
        /// <returns>A DataTable with appended rows.</returns>
        protected DataTable AppendRows(DataTable dataTable, EntityCollection entities, int recordLimit)
        {
            Dictionary<string, EntityMetadata> metadata = new Dictionary<string, EntityMetadata>();

            foreach (DataColumn column in dataTable.Columns)
            {
                string entityLogicalName = column.ColumnName.Substring(0, column.ColumnName.IndexOf("."));

                if (!metadata.ContainsKey(entityLogicalName))
                {
                    metadata.Add(entityLogicalName, Dynamics365Entity.Create(entityLogicalName, (Dynamics365Connection)Parent).GetEntityMetadata((Dynamics365Connection)Parent));
                }
            }

            for (int recordIndex = 0; recordIndex < entities.Entities.Count && dataTable.Rows.Count < recordLimit; recordIndex++)
            {
                Entity record = entities.Entities[recordIndex];
                DataRow row = dataTable.NewRow();

                foreach (KeyValuePair<string, object> attribute in record.Attributes)
                {
                    string entityLogicalName = record.LogicalName;
                    string attributeLogicalName = attribute.Key;
                    object attributeValue = attribute.Value;

                    if (attribute.Value is AliasedValue)
                    {
                        AliasedValue aliasedValue = (AliasedValue)attribute.Value;
                        entityLogicalName = aliasedValue.EntityLogicalName;
                        attributeLogicalName = aliasedValue.AttributeLogicalName;
                        attributeValue = aliasedValue.Value;
                    }

                    string nativeColumnName = string.Format("{0}.{1}", entityLogicalName, attributeLogicalName);

                    if (DataTypes == DataTypes.Native)
                    {
                        if (dataTable.Columns.Contains(nativeColumnName))
                        {
                            row[nativeColumnName] = attributeValue;
                        }
                    }
                    else if (DataTypes == DataTypes.Neutral)
                    {
                        AttributeMetadata attributeMetadata = metadata[entityLogicalName].Attributes.FirstOrDefault(findField => findField.LogicalName == attributeLogicalName);
                        Dynamics365TypeConverter converter = new Dynamics365TypeConverter(attributeMetadata);

                        if (converter.Dynamics365Type == typeof(EntityReference) || converter.Dynamics365Type == typeof(EntityCollection))
                        {
                            if (LookupBehaviour == LookupBehaviour.Name || LookupBehaviour == LookupBehaviour.NameAndIdentifier || LookupBehaviour == LookupBehaviour.EntityAndNameAndIdentifier)
                            {
                                string nameColumn = string.Format("{0}.Name", nativeColumnName);

                                if (dataTable.Columns.Contains(nameColumn))
                                {
                                    if (converter.Dynamics365Type == typeof(EntityReference))
                                    {
                                        row[nameColumn] = ((EntityReference)attributeValue).Name;
                                    }
                                    else if (converter.Dynamics365Type == typeof(EntityCollection))
                                    {
                                        string names = string.Empty;

                                        foreach (Entity entity in ((EntityCollection)attributeValue).Entities)
                                        {
                                            EntityReference party = (EntityReference)entity["partyid"];
                                            names = names == string.Empty ? party.Name : string.Concat(names, PARTYLIST_DELIMITER, party.Name);
                                        }

                                        row[nameColumn] = names;
                                    }
                                }
                            }

                            if (LookupBehaviour == LookupBehaviour.Identifier || LookupBehaviour == LookupBehaviour.NameAndIdentifier || LookupBehaviour == LookupBehaviour.EntityAndNameAndIdentifier)
                            {
                                string identifierColumn = string.Format("{0}.Identifier", nativeColumnName);

                                if (dataTable.Columns.Contains(identifierColumn))
                                {
                                    if (converter.Dynamics365Type == typeof(EntityReference))
                                    {
                                        row[identifierColumn] = ((EntityReference)attributeValue).Id.ToString();
                                    }
                                    else if (converter.Dynamics365Type == typeof(EntityCollection))
                                    {
                                        string identifiers = string.Empty;

                                        foreach (Entity entity in ((EntityCollection)attributeValue).Entities)
                                        {
                                            EntityReference party = (EntityReference)entity["partyid"];
                                            identifiers = identifiers == string.Empty ? party.Id.ToString() : string.Concat(identifiers, PARTYLIST_DELIMITER, party.Id.ToString());
                                        }

                                        row[identifierColumn] = identifiers;
                                    }
                                }
                            }

                            if (LookupBehaviour == LookupBehaviour.Entity || LookupBehaviour == LookupBehaviour.EntityAndNameAndIdentifier)
                            {
                                string entityColumn = string.Format("{0}.Entity", nativeColumnName);

                                if (dataTable.Columns.Contains(entityColumn))
                                {
                                    if (converter.Dynamics365Type == typeof(EntityReference))
                                    {
                                        row[entityColumn] = ((EntityReference)attributeValue).LogicalName;
                                    }
                                    else if (converter.Dynamics365Type == typeof(EntityCollection))
                                    {
                                        string logicalNames = string.Empty;

                                        foreach (Entity entity in ((EntityCollection)attributeValue).Entities)
                                        {
                                            EntityReference party = (EntityReference)entity["partyid"];
                                            logicalNames = logicalNames == string.Empty ? party.LogicalName : string.Concat(logicalNames, PARTYLIST_DELIMITER, party.LogicalName);
                                        }

                                        row[entityColumn] = logicalNames;
                                    }
                                }
                            }
                        }
                        else if (converter.Dynamics365Type == typeof(OptionSetValue))
                        {
                            if (OptionSetBehaviour == OptionSetBehaviour.Name || OptionSetBehaviour == OptionSetBehaviour.NameAndCode)
                            {
                                string nameColumn = string.Format("{0}.Name", nativeColumnName);

                                if (dataTable.Columns.Contains(nameColumn))
                                {
                                    EnumAttributeMetadata enumMetadata = (EnumAttributeMetadata)attributeMetadata;
                                    OptionMetadata optionMetadata = enumMetadata.OptionSet.Options.FirstOrDefault(option => option.Value == ((OptionSetValue)attributeValue).Value);

                                    if (optionMetadata != default(OptionMetadata))
                                    {
                                        row[nameColumn] = optionMetadata.Label.UserLocalizedLabel.Label;
                                    }
                                }
                            }

                            if (OptionSetBehaviour == OptionSetBehaviour.Code || OptionSetBehaviour == OptionSetBehaviour.NameAndCode)
                            {
                                string codeColumn = string.Format("{0}.Code", nativeColumnName);

                                if (dataTable.Columns.Contains(codeColumn))
                                {
                                    row[codeColumn] = ((OptionSetValue)attributeValue).Value;
                                }
                            }
                        }
                        else if (converter.Dynamics365Type == typeof(OptionSetValueCollection))
                        {
                            if (OptionSetBehaviour == OptionSetBehaviour.Name || OptionSetBehaviour == OptionSetBehaviour.NameAndCode)
                            {
                                string nameColumn = string.Format("{0}.Name", nativeColumnName);

                                if (dataTable.Columns.Contains(nameColumn))
                                {
                                    List<string> labels = new List<string>();
                                    EnumAttributeMetadata enumMetadata = (EnumAttributeMetadata)attributeMetadata;
                                    OptionSetValueCollection optionSetValues = (OptionSetValueCollection)attributeValue;

                                    foreach (OptionSetValue optionSetValue in optionSetValues)
                                    {
                                        OptionMetadata optionMetadata = enumMetadata.OptionSet.Options.FirstOrDefault(option => option.Value == optionSetValue.Value);

                                        if (optionMetadata != default(OptionMetadata))
                                        {
                                            labels.Add(optionMetadata.Label.UserLocalizedLabel.Label);
                                        }
                                    }

                                    row[nameColumn] = string.Join(Dynamics365TypeConverter.MULTI_OPTION_SET_DELIMITER, labels.Select(label => label));
                                }
                            }

                            if (OptionSetBehaviour == OptionSetBehaviour.Code || OptionSetBehaviour == OptionSetBehaviour.NameAndCode)
                            {
                                string codeColumn = string.Format("{0}.Code", nativeColumnName);

                                if (dataTable.Columns.Contains(codeColumn))
                                {
                                    OptionSetValueCollection optionSetValues = (OptionSetValueCollection)attributeValue;
                                    row[codeColumn] = string.Join(Dynamics365TypeConverter.MULTI_OPTION_SET_DELIMITER, optionSetValues.Select(optionSet => optionSet.Value.ToString()));
                                }
                            }
                        }
                        else
                        {
                            if (dataTable.Columns.Contains(nativeColumnName))
                            {
                                row[nativeColumnName] = converter.ConvertFrom(null, null, attributeValue);
                            }
                        }
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        /// Gets a list of Dynamics365Entity objects.
        /// </summary>
        /// <returns>The list of Dynamics365Entity objects.</returns>
        public List<Dynamics365Entity> GetEntities()
        {
            List<Dynamics365Entity> entities = new List<Dynamics365Entity>();

            try
            {
                if (Parent != default(IConnection))
                {
                    entities.AddRange(Dynamics365Entity.GetEntities((Dynamics365Connection)Parent));
                }
            }
            catch { }

            return entities;
        }
    }

    /// <summary>
    /// Defines how option sets are presented in returned data.
    /// </summary>
    public enum OptionSetBehaviour
    {
        Code,
        Name,
        NameAndCode
    }

    /// <summary>
    /// Defines how lookups are presented in returned data.
    /// </summary>
    public enum LookupBehaviour
    {
        Entity,
        Name,
        Identifier,
        EntityAndIdentifier,
        NameAndIdentifier,
        EntityAndNameAndIdentifier
    }

    /// <summary>
    /// Defines whether returned values will remain in their native data type or be converted to a neutral data type.
    /// </summary>
    public enum DataTypes
    {
        Native,
        Neutral
    }
}

