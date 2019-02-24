using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365UserOrTeamOwnedEntitiesProvider
    {
        List<Dynamics365Entity> GetUserOrTeamOwnedEntities();
    }
}
