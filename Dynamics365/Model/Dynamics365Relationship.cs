using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365Relationship
    {
        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the primary entity logical name.
        /// </summary>
        public string EntityLogicalName { get; set; }

        /// <summary>
        /// Gets or sets the related entity logical name.
        /// </summary>
        public string RelatedEntityLogicalName { get; set; }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The schema name.</returns>
        public override string ToString()
        {
            return SchemaName.ToString();
        }

        public static List<Dynamics365Relationship> GetRelationships(Dynamics365Entity entity, Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetRelationships:{0}", entity.LogicalName);
            List<Dynamics365Relationship> relationships = (List<Dynamics365Relationship>)cache[cacheKey];

            if (relationships == null)
            {
                relationships = new List<Dynamics365Relationship>();
                RetrieveEntityRequest request = new RetrieveEntityRequest()
                {
                    LogicalName = entity.LogicalName,
                    EntityFilters = EntityFilters.Relationships,
                    RetrieveAsIfPublished = false
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    RetrieveEntityResponse response = (RetrieveEntityResponse)proxy.Execute(request);

                    foreach (ManyToManyRelationshipMetadata metadata in response.EntityMetadata.ManyToManyRelationships)
                    {
                        Dynamics365Relationship relationship = new Dynamics365Relationship()
                        {
                            SchemaName = metadata.SchemaName,
                            EntityLogicalName = metadata.Entity1LogicalName == entity.LogicalName ? metadata.Entity1LogicalName : metadata.Entity2LogicalName,
                            RelatedEntityLogicalName = metadata.Entity1LogicalName == entity.LogicalName ? metadata.Entity2LogicalName : metadata.Entity1LogicalName
                        };
                        relationships.Add(relationship);
                    }

                    foreach (OneToManyRelationshipMetadata metadata in response.EntityMetadata.ManyToOneRelationships)
                    {
                        Dynamics365Relationship relationship = new Dynamics365Relationship()
                        {
                            SchemaName = metadata.SchemaName,
                            EntityLogicalName = metadata.ReferencedEntity == entity.LogicalName ? metadata.ReferencedEntity : metadata.ReferencingEntity,
                            RelatedEntityLogicalName = metadata.ReferencedEntity == entity.LogicalName ? metadata.ReferencingEntity : metadata.ReferencedEntity
                        };
                        relationships.Add(relationship);
                    }

                    foreach (OneToManyRelationshipMetadata metadata in response.EntityMetadata.OneToManyRelationships)
                    {
                        Dynamics365Relationship relationship = new Dynamics365Relationship()
                        {
                            SchemaName = metadata.SchemaName,
                            EntityLogicalName = metadata.ReferencedEntity == entity.LogicalName ? metadata.ReferencedEntity : metadata.ReferencingEntity,
                            RelatedEntityLogicalName = metadata.ReferencedEntity == entity.LogicalName ? metadata.ReferencingEntity : metadata.ReferencedEntity
                        };
                        relationships.Add(relationship);
                    }
                }

                cache[cacheKey] = relationships;
            }

            return relationships;
        }
    }
}
