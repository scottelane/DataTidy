using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    interface IDynamics365StatusesProvider
    {
        List<Dynamics365Status> GetStatuses();
    }
}
