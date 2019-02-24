using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365TargetEntitiesProvider
    {
        List<Dynamics365Entity> GetTargetEntities();
    }
}
