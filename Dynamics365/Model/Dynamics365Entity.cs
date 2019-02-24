using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365Entity : DataDestination, IComparable<Dynamics365Entity>
    {
        /// <summary>
        /// Gets or sets the object type code.
        /// </summary>
        public int ObjectTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the logical name.
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// Gets or sets the plural name.
        /// </summary>
        public string PluralName { get; set; }

        /// <summary>
        /// Gets or sets the primary field name.
        /// </summary>
        public string PrimaryFieldName { get; set; }

        /// <summary>
        /// Gets or sets the primary key field name.
        /// </summary>
        public string PrimaryIdFieldName { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the entity is user or team owned.
        /// </summary>
        public bool IsUserTeamOwned { get; set; }

        /// <summary>
        /// Initialises a new instance of the Dynamics365Entity class.
        /// </summary>
        public Dynamics365Entity()
        { }

        public EntityMetadata GetEntityMetadata(Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetEntityMetadata:{0}", LogicalName);
            EntityMetadata entityMetadata = (EntityMetadata)cache[cacheKey];

            if (entityMetadata == default(EntityMetadata))
            {
                RetrieveEntityRequest request = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.Attributes,
                    LogicalName = LogicalName,
                    RetrieveAsIfPublished = true
                };

                using (OrganizationServiceProxy proxy = (connection).OrganizationServiceProxy)
                {
                    RetrieveEntityResponse response = (RetrieveEntityResponse)proxy.Execute(request);
                    entityMetadata = response.EntityMetadata;
                }

                cache[cacheKey] = entityMetadata;
            }

            return entityMetadata;
        }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The display name.</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        public int CompareTo(Dynamics365Entity other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return LogicalName.CompareTo(other.LogicalName);
            }
        }

        /// <summary>
        /// Creates a new instance of the Dynamics365Entity class from an entity with the specified logical name in the specified Dynamics instance.
        /// </summary>
        /// <param name="logicalName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static Dynamics365Entity Create(string logicalName, Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetEntity:{0}", logicalName);
            Dynamics365Entity entity = (Dynamics365Entity)cache[cacheKey];

            if (entity == null)
            {
                RetrieveEntityRequest request = new RetrieveEntityRequest()
                {
                    LogicalName = logicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = false
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    RetrieveEntityResponse response = (RetrieveEntityResponse)proxy.Execute(request);
                    entity = CreateFromMetadata(response.EntityMetadata, connection, true);
                }

                cache[cacheKey] = entity;
            }

            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <param name="saveMetadata"></param>
        /// <returns></returns>
        public static Dynamics365Entity CreateFromMetadata(EntityMetadata entityMetadata, Dynamics365Connection connection, bool saveMetadata)
        {
            Dynamics365Entity entity = new Dynamics365Entity()
            {
                DisplayName = entityMetadata.DisplayName.UserLocalizedLabel.Label,
                LogicalName = entityMetadata.LogicalName,
                PluralName = entityMetadata.DisplayCollectionName.UserLocalizedLabel.Label,
                PrimaryFieldName = entityMetadata.PrimaryNameAttribute,
                PrimaryIdFieldName = entityMetadata.PrimaryIdAttribute
            };
            entity.DisplayName = entityMetadata.DisplayName.UserLocalizedLabel.Label;
            entity.IsUserTeamOwned = entityMetadata.OwnershipType == OwnershipTypes.TeamOwned || entityMetadata.OwnershipType == OwnershipTypes.UserOwned ? true : false;
            entity.ObjectTypeCode = (int)entityMetadata.ObjectTypeCode;

            if (saveMetadata)
            {
                ConnectionCache cache = new ConnectionCache(connection);
                string cacheKey = string.Format("GetEntityMetadata:{0}", entity.LogicalName);
                cache[cacheKey] = entityMetadata;
            }

            return entity;
        }

        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Dynamics365Entity> GetEntities(Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = "GetEntities";
            List<Dynamics365Entity> entities = (List<Dynamics365Entity>)cache[cacheKey];

            if (entities == default(List<Dynamics365Entity>))
            {
                // Gets a list of all entities in the specified Dynamics 365 instance.
                entities = new List<Dynamics365Entity>();
                RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = false
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)proxy.Execute(request);

                    foreach (EntityMetadata entityMetadata in response.EntityMetadata)
                    {
                        if (entityMetadata.DisplayName.UserLocalizedLabel != null && (entityMetadata.IsCustomizable.Value || !entityMetadata.IsManaged.Value || entityMetadata.LogicalName == "asyncoperation" || entityMetadata.LogicalName == "post"))
                        {
                            Dynamics365Entity entity = CreateFromMetadata(entityMetadata, connection, false);
                            entities.Add(entity);
                        }
                    }

                    entities.Sort((entity1, entity2) => entity1.DisplayName.CompareTo(entity2.DisplayName));
                }

                cache[cacheKey] = entities;
            }

            return entities;
        }

        public override List<Field> GetFields(IConnection connection)
        {
            return Dynamics365Field.GetFields(this, (Dynamics365Connection)connection).ConvertAll(field => (Field)field);
        }
    }
}