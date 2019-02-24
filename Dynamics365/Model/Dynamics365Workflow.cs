using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Models a Dynamics 365 on-demand workflow.
    /// </summary>
    public class Dynamics365Workflow
    {
        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the workflow name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityLogicalName"></param>
        /// <returns></returns>
        public static List<Dynamics365Workflow> GetWorkflows(Dynamics365Connection connection, string entityLogicalName)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetOnDemandWorkflows:{0}", entityLogicalName);
            List<Dynamics365Workflow> workflows = (List<Dynamics365Workflow>)cache[cacheKey];

            if (workflows == default(List<Dynamics365Workflow>))
            {
                workflows = new List<Dynamics365Workflow>();

                QueryExpression workflowQuery = new QueryExpression("workflow")
                {
                    ColumnSet = new ColumnSet(true)
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    workflowQuery.Criteria.AddCondition(new ConditionExpression("category", ConditionOperator.Equal, 0));   // workflow
                    workflowQuery.Criteria.AddCondition(new ConditionExpression("primaryentity", ConditionOperator.Equal, entityLogicalName));
                    workflowQuery.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 1)); // activated
                    workflowQuery.Criteria.AddCondition(new ConditionExpression("type", ConditionOperator.Equal, 1));   // definition
                    workflowQuery.Criteria.AddCondition(new ConditionExpression("ondemand", ConditionOperator.Equal, true));

                    RetrieveMultipleRequest workflowRequest = new RetrieveMultipleRequest()
                    {
                        Query = workflowQuery
                    };
                    RetrieveMultipleResponse workflowResponse = (RetrieveMultipleResponse)proxy.Execute(workflowRequest);

                    foreach (Microsoft.Xrm.Sdk.Entity workflowMetadata in workflowResponse.EntityCollection.Entities)
                    {
                        workflows.Add(new Dynamics365Workflow()
                        {
                            ID = workflowMetadata.Id,
                            Name = workflowMetadata.Attributes["name"].ToString()
                        });
                    }
                }

                workflows.Sort((workflow1, workflow2) => workflow1.Name.CompareTo(workflow2.Name));
                cache[cacheKey] = workflows;
            }

            return workflows;
        }
    }
}