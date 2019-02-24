using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365User
    {
        public const string USER_ENTITY_NAME = "systemuser";

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name { get; set; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(USER_ENTITY_NAME, ID);
        }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The user name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Compares the object with another UserInfo objects.
        /// </summary>
        /// <param name="other">The other UserInfo object to compare to.</param>
        /// <returns>The comparison result.</returns>
        public int CompareTo(Dynamics365User other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return Name.CompareTo(other.Name);
            }
        }

        public static List<Dynamics365User> GetUsers(Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = "GetUsers";
            List<Dynamics365User> users = (List<Dynamics365User>)cache[cacheKey];

            if (users == null)
            {
                users = new List<Dynamics365User>();
                QueryExpression userQuery = new QueryExpression("systemuser")
                {
                    ColumnSet = new ColumnSet(true)
                };
                RetrieveMultipleRequest userRequest = new RetrieveMultipleRequest()
                {
                    Query = userQuery
                };

                using (OrganizationServiceProxy proxy = (connection).OrganizationServiceProxy)
                {
                    RetrieveMultipleResponse formResponse = (RetrieveMultipleResponse)proxy.Execute(userRequest);

                    foreach (Entity userMetadata in formResponse.EntityCollection.Entities)
                    {
                        Dynamics365User user = new Dynamics365User()
                        {
                            ID = userMetadata.Id,
                            Name = userMetadata.Attributes["fullname"].ToString()
                        };
                        users.Add(user);
                    }

                    users.Sort((user1, user2) => user1.Name.CompareTo(user2.Name));
                }

                cache[cacheKey] = users;
            }

            return users;
        }
    }
}
