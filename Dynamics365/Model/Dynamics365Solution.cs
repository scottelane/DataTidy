using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365Solution
    {
        /// <summary>
        /// Gets or sets the unique solution name.
        /// </summary>
        public string UniqueName { get; set; }

        /// <summary>
        /// Gets or sets the friendly name.
        /// </summary>
        public string FriendlyName { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Dynamics365Solution> GetSolutions(Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = "GetSolutions:{0}";
            List<Dynamics365Solution> solutions = (List<Dynamics365Solution>)cache[cacheKey];

            if (solutions == default(List<Dynamics365Solution>))
            {
                // get a list of all solutions in the specified Dynamics 365 instance.
                solutions = new List<Dynamics365Solution>();

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    QueryExpression query = new QueryExpression()
                    {
                        EntityName = "solution",
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression()
                    };

                    EntityCollection response = proxy.RetrieveMultiple(query);

                    foreach (Entity entity in response.Entities)
                    {
                        Dynamics365Solution solution = new Dynamics365Solution()
                        {
                            UniqueName = entity.Attributes["uniquename"].ToString(),
                            FriendlyName = entity.Attributes["friendlyname"].ToString(),
                            Version = entity.Attributes["version"].ToString()
                        };

                        solutions.Add(solution);
                    }

                    solutions.Sort((solution1, solution2) => solution1.FriendlyName.CompareTo(solution2.FriendlyName));
                }

                cache[cacheKey] = solutions;
            }

            return solutions;
        }

    }
}
