using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365ProcessesProvider
    {
        List<Dynamics365Process> GetProcesses();
    }
}
