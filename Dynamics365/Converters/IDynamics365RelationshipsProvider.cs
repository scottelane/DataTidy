using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365RelationshipsProvider
    {
        List<Dynamics365Relationship> GetRelationships();
    }
}
