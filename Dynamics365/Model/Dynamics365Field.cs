using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Models a Dynamics 365 field.
    /// </summary>
    public class Dynamics365Field : Field, IComparable<Dynamics365Field>, IEquatable<Dynamics365Field>
    {
        /// <summary>
        /// Gets or sets the logical name.
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is a primary ID.
        /// </summary>
        public bool IsPrimaryId { get; set; }

        /// <summary>
        /// Gets or sets the entity logical name.
        /// </summary>
        public string EntityLogicalName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanCreate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanUpdate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[] Targets { get; set; }

        /// <summary>
        /// Gets a string representation of the instance by combining the display name and logical name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", DisplayName, LogicalName);
        }

        public bool Equals(Dynamics365Field other)
        {
            if (other == default(Dynamics365Field))
            {
                return false;
            }

            return EntityLogicalName == other.EntityLogicalName && LogicalName == other.LogicalName;
        }

        /// <summary>
        /// Compares a Dynamics365Field to another instance.
        /// </summary>
        /// <param name="other">The other Dynamics365Field.</param>
        /// <returns>An integer that indicates whether the instance is less than, equal to or greater than another instance.</returns>
        public int CompareTo(Dynamics365Field other)
        {
            if (other == null)
            {
                return 1;
            }

            return LogicalName.CompareTo(other.LogicalName);
        }

        /// <summary>
        /// Gets entity fields relevant to the operation type.
        /// </summary>
        /// <param name="operationType">The operation type.</param>
        /// <returns>The fields.</returns>
        public static List<Dynamics365Field> GetFields(Dynamics365Entity entity, Dynamics365Connection connection)
        {
            if (entity == default(Dynamics365Entity)) throw new ArgumentException("Entity cannot be null", nameof(entity));
            if (connection == default(Dynamics365Connection)) throw new ArgumentException("Connection cannot be null", nameof(connection));

            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetFields:{0}", entity.LogicalName);
            List<Dynamics365Field> fields = (List<Dynamics365Field>)cache[cacheKey];

            if (fields == default(List<Dynamics365Field>))
            {
                fields = new List<Dynamics365Field>();
                RetrieveEntityRequest request = new RetrieveEntityRequest()
                {
                    LogicalName = entity.LogicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = false
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    RetrieveEntityResponse response = (RetrieveEntityResponse)proxy.Execute(request);

                    foreach (AttributeMetadata attributeMetadata in response.EntityMetadata.Attributes)
                    {
                        if ((bool)attributeMetadata.IsValidForRead)
                        {
                            Dynamics365Field field = CreateFromMetadata(attributeMetadata, connection);

                            if (field != null)
                            {
                                fields.Add(field);
                            }
                        }
                    }
                }

                fields.Sort((field1, field2) => field1.DisplayName.CompareTo(field2.DisplayName));
                cache[string.Format(cacheKey, entity.LogicalName)] = fields;
            }

            return fields;
        }

        /// <summary>
        /// Creates a Dynamics365Field instance from the field's AttributeMetadata.
        /// </summary>
        /// <param name="attributeMetadata">The AttributeMetadata.</param>
        /// <returns>A Dynamics365Field.</returns>
        public static Dynamics365Field CreateFromMetadata(AttributeMetadata attributeMetadata, Dynamics365Connection connection)
        {
            Dynamics365Field field = null;
            Dynamics365TypeConverter converter = new Dynamics365TypeConverter(attributeMetadata);

            if (converter.CanConvertTo(converter.Dynamics365Type) && attributeMetadata.DisplayName.LocalizedLabels.Count > 0)
            {
                field = new Dynamics365Field()
                {
                    LogicalName = attributeMetadata.LogicalName,
                    DisplayName = attributeMetadata.DisplayName.UserLocalizedLabel.Label,
                    EntityLogicalName = attributeMetadata.EntityLogicalName,
                    IsPrimaryId = (bool)attributeMetadata.IsPrimaryId,
                    CanCreate = (bool)attributeMetadata.IsValidForCreate,
                    CanUpdate = (bool)attributeMetadata.IsValidForUpdate,
                    Targets = attributeMetadata is LookupAttributeMetadata ? ((LookupAttributeMetadata)attributeMetadata).Targets : default(string[])
                };

                // see https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/configure-activity-feeds
                if ((field.EntityLogicalName == "post" || field.EntityLogicalName == "postfollow") && attributeMetadata is LookupAttributeMetadata && field.Targets?.Length == 0)
                {
                    using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                    {
                        ConnectionCache cache = new ConnectionCache(connection);
                        string cacheKey = string.Format("GetPostConfigurations");
                        string[] targets = (string[])cache[cacheKey];

                        if (targets == null)
                        {
                            const string ENTITY_NAME_LOGICAL_NAME = "msdyn_entityname";

                            FilterExpression filter = new FilterExpression();
                            filter.AddCondition("statecode", ConditionOperator.Equal, 0);
                            filter.AddCondition("statuscode", ConditionOperator.Equal, 1);

                            QueryExpression query = new QueryExpression()
                            {
                                EntityName = "msdyn_postconfig",
                                ColumnSet = new ColumnSet(ENTITY_NAME_LOGICAL_NAME),
                                Criteria = filter,
                            };

                            EntityCollection response = proxy.RetrieveMultiple(query);
                            field.Targets = new string[response.Entities.Count];

                            for (int entityIndex = 0; entityIndex < response.Entities.Count; entityIndex++)
                            {
                                field.Targets[entityIndex] = response.Entities[entityIndex].Attributes[ENTITY_NAME_LOGICAL_NAME].ToString();
                            }

                            cache[cacheKey] = field.Targets;
                        }
                    }
                }
            }

            return field;
        }
    }
}
