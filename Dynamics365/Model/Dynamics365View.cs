using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365View
    {
        /// <summary>
        /// The unique view identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// The CRM display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The FetchXml query.
        /// </summary>
        public string FetchXml { get; set; }

        /// <summary>
        /// The view owner identifier.
        /// </summary>
        public Guid OwnerID { get; set; }

        public Dynamics365View()
        {
        }

        /// <summary>
        /// Gets views of the specified type for the specified enttiy.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="viewType"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Dynamics365View> GetViews(Dynamics365Entity entity, ViewType viewType, Dynamics365Connection connection)
        {
            List<Dynamics365View> views = null;

            if (viewType == ViewType.System)
            {
                views = GetSystemViews(entity, connection);
            }
            else if (viewType == ViewType.Personal)
            {
                views = GetPersonalViews(entity, connection);
            }

            return views;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static List<Dynamics365View> GetSystemViews(Dynamics365Entity entity, IConnection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetSystemViews:{0}:{1}", typeof(Dynamics365View).Name, entity.LogicalName);
            List<Dynamics365View> views = (List<Dynamics365View>)cache[cacheKey];

            if (views == null)
            {
                views = new List<Dynamics365View>();
                QueryExpression query = new QueryExpression("savedquery");
                query.Criteria.AddCondition("returnedtypecode", ConditionOperator.Equal, entity.ObjectTypeCode);
                query.Criteria.AddCondition("fetchxml", ConditionOperator.NotNull);
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);   // todo - not working?
                query.ColumnSet = new ColumnSet(new string[] { "name", "fetchxml" });

                RetrieveMultipleRequest request = new RetrieveMultipleRequest()
                {
                    Query = query
                };

                using (OrganizationServiceProxy proxy = ((Dynamics365Connection)connection).OrganizationServiceProxy)
                {
                    RetrieveMultipleResponse response = (RetrieveMultipleResponse)proxy.Execute(request);

                    foreach (Entity viewEntity in response.EntityCollection.Entities)
                    {
                        Dynamics365View view = new Dynamics365View()
                        {
                            ID = viewEntity.Id,
                            DisplayName = (string)viewEntity.Attributes["name"],
                            FetchXml = (string)viewEntity.Attributes["fetchxml"]
                        };
                        views.Add(view);
                    }
                }

                views.Sort((view1, view2) => view1.DisplayName.CompareTo(view2.DisplayName));
                cache[cacheKey] = views;
            }

            return views;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static List<Dynamics365View> GetPersonalViews(Dynamics365Entity entity, IConnection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetPersonalViews:{0}:{1}", typeof(Dynamics365View).Name, entity.LogicalName);
            List<Dynamics365View> views = (List<Dynamics365View>)cache[cacheKey];

            if (views == null)
            {
                views = new List<Dynamics365View>();
                QueryExpression query = new QueryExpression("userquery");
                query.Criteria.AddCondition("returnedtypecode", ConditionOperator.Equal, entity.ObjectTypeCode);
                query.Criteria.AddCondition("fetchxml", ConditionOperator.NotNull);
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                query.ColumnSet = new ColumnSet(new string[] { "name", "fetchxml" });   // todo - not working?

                RetrieveMultipleRequest request = new RetrieveMultipleRequest()
                {
                    Query = query
                };

                using (OrganizationServiceProxy proxy = ((Dynamics365Connection)connection).OrganizationServiceProxy)
                {
                    RetrieveMultipleResponse response = (RetrieveMultipleResponse)proxy.Execute(request);

                    foreach (Entity viewEntity in response.EntityCollection.Entities)
                    {
                        Dynamics365View view = new Dynamics365View()
                        {
                            ID = viewEntity.Id,
                            DisplayName = (string)viewEntity.Attributes["name"],
                            FetchXml = (string)viewEntity.Attributes["fetchxml"]
                        };
                        views.Add(view);
                    }

                    views.Sort((view1, view2) => view1.DisplayName.CompareTo(view2.DisplayName));
                }

                cache[cacheKey] = views;
            }

            return views;
        }
    }

    public enum ViewType
    {
        System,
        Personal
    }
}
