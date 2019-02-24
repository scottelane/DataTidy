using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface IDynamics365SetStateDataDestinationFieldsProvider
    {
        List<Field> GetDynamics365SetStateDataDestinationFields();
    }
}