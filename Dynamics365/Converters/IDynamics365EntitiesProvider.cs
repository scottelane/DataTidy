using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365EntitiesProvider
    {
        List<Dynamics365Entity> GetEntities();
    }
}
