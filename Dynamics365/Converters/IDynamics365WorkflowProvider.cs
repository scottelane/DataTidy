using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365WorkflowProvider
    {
        List<Dynamics365Workflow> GetWorkflows();
    }
}
