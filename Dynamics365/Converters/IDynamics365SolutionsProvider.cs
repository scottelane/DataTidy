using System.Collections.Generic;

namespace ScottLane.DataTidy.Dynamics365
{
    public interface IDynamics365SolutionsProvider
    {
        List<Dynamics365Solution> GetSolutions();
    }
}
