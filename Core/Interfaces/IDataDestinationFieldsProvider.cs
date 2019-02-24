using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface IDataDestinationFieldsProvider
    {
        List<Field> GetDataDestinationFields();
    }
}
