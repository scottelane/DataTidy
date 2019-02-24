using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365Team
    {
        public const string TEAM_ENTITY_NAME = "team";

        /// <summary>
        /// Gets or sets the team id.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the team name.
        /// </summary>
        public string Name { get; set; }

        public EntityReference ToEntityReference()
        {
            return new EntityReference(TEAM_ENTITY_NAME, ID);
        }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Compares the object with another TeamInfo object.
        /// </summary>
        /// <param name="other">The other TeamInfo object.</param>
        /// <returns>The comparison result.</returns>
        public int CompareTo(Dynamics365Team other)
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

        public static List<Dynamics365Team> GetTeams(Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = "GetTeams:{0}";
            List<Dynamics365Team> teams = (List<Dynamics365Team>)cache[cacheKey];

            if (teams == default(List<Dynamics365Team>))
            {
                teams = new List<Dynamics365Team>();
                QueryExpression teamQuery = new QueryExpression("team")
                {
                    ColumnSet = new ColumnSet(true)
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    if (connection.IsVersionOrAbove(CrmVersion.Crm2013))
                    {
                        teamQuery.Criteria.AddCondition(new ConditionExpression("teamtype", ConditionOperator.Equal, 0));
                    }

                    RetrieveMultipleRequest teamRequest = new RetrieveMultipleRequest()
                    {
                        Query = teamQuery
                    };
                    RetrieveMultipleResponse teamResponse = (RetrieveMultipleResponse)proxy.Execute(teamRequest);

                    foreach (Microsoft.Xrm.Sdk.Entity teamMetadata in teamResponse.EntityCollection.Entities)
                    {
                        teams.Add(new Dynamics365Team()
                        {
                            ID = teamMetadata.Id,
                            Name = teamMetadata.Attributes["name"].ToString()
                        });
                    }

                    teams.Sort((team1, team2) => team1.Name.CompareTo(team2.Name));
                }

                cache[cacheKey] = teams;
            }

            return teams;
        }
    }
}
