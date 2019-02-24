using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    interface IDynamics365TeamsProvider
    {
        List<Dynamics365Team> GetTeams();
    }
}
