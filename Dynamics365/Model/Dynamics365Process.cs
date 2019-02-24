using System;
using System.Collections.Generic;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365Process
    {
        public static readonly int STATE_CODE_ACTIVE = 0;
        public static readonly int STATE_CODE_INACTIVE = 1;

        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        public string Name { get; set; }

        public static List<Dynamics365Process> GetProcesses(Dynamics365Connection connection, string entityLogicalName)
        {
            ConnectionCache cache = new ConnectionCache(connection);
            string cacheKey = "GetProcesses";
            List<Dynamics365Process> processes = (List<Dynamics365Process>)cache[cacheKey];

            if (processes == default(List<Dynamics365Process>))
            {
                processes = new List<Dynamics365Process>();
                processes.Sort((process1, process2) => process1.Name.CompareTo(process2.Name));
                cache[cacheKey] = processes;
            }

            return processes;
        }
    }
}
