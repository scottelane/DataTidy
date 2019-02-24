using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365EntityFieldsProvider
    {
        List<Dynamics365Field> GetDynamics365EntityFields();
    }
}
