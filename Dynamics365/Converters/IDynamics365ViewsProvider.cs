using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    interface IDynamics365ViewsProvider
    {
        List<Dynamics365View> GetDynamics365Views();
    }
}
