using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365UsersProvider
    {
        List<Dynamics365User> GetUsers();
    }
}
