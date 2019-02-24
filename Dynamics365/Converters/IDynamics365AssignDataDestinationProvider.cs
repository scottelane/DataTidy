using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface IDynamics365AssignDataDestinationFieldsProvider
    {
        List<Field> GetDynamics365AssignDataDestinationFields();
    }
}