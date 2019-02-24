using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Models a Dynamics 365 entity state.
    /// </summary>
    public class Dynamics365State
    {
        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the logical name of the state field.
        /// </summary>
        public string LogicalName { get; set; }

        public List<Dynamics365Status> Statuses { get; set; }

        /// <summary>
        /// Initialises a new instance of the StateInfo class.
        /// </summary>
        public Dynamics365State()
        {
            Statuses = new List<Dynamics365Status>();
        }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString()
        {
            return Name?.ToString();
        }

        public static List<Dynamics365State> GetStates(Dynamics365Entity entity, Dynamics365Connection connection)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = string.Format("GetStates:{0}", entity.LogicalName);
            List<Dynamics365State> states = (List<Dynamics365State>)cache[cacheKey];

            if (states == null)
            {
                states = new List<Dynamics365State>();
                RetrieveEntityRequest request = new RetrieveEntityRequest()
                {
                    LogicalName = entity.LogicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = false
                };

                using (OrganizationServiceProxy proxy = connection.OrganizationServiceProxy)
                {
                    RetrieveEntityResponse response = (RetrieveEntityResponse)proxy.Execute(request);
                    StateAttributeMetadata stateMetadata = (StateAttributeMetadata)response.EntityMetadata.Attributes.FirstOrDefault(findField => findField is StateAttributeMetadata);

                    if (stateMetadata != null)
                    {
                        foreach (StateOptionMetadata stateOption in stateMetadata.OptionSet.Options)
                        {
                            Dynamics365State state = new Dynamics365State()
                            {
                                Code = (int)stateOption.Value,
                                Name = stateOption.Label.UserLocalizedLabel.Label,
                                LogicalName = stateMetadata.LogicalName
                            };
                            StatusAttributeMetadata statusMetadata = (StatusAttributeMetadata)response.EntityMetadata.Attributes.FirstOrDefault(findField => findField is StatusAttributeMetadata);

                            if (statusMetadata != null)
                            {
                                foreach (StatusOptionMetadata statusOption in statusMetadata.OptionSet.Options)
                                {
                                    if (statusOption.State == state.Code)
                                    {
                                        Dynamics365Status status = new Dynamics365Status()
                                        {
                                            Code = (int)statusOption.Value,
                                            Name = statusOption.Label.UserLocalizedLabel.Label,
                                            LogicalName = statusMetadata.LogicalName
                                        };
                                        state.Statuses.Add(status);
                                    }
                                }

                                //state.Statuses.Sort((status1, status2) => status1.Name.CompareTo(status2.Name));
                            }

                            states.Add(state);
                        }
                    }
                }

                //states.Sort((state1, state2) => state1.Name.CompareTo(state2.Name));
                cache[cacheKey] = states;
            }

            return states;
        }
    }
}
