using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365StatesProvider
    {
        List<Dynamics365State> GetStates();
    }
}
