using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface ILookupSourceFieldsProvider
    {
        List<DataTableField> GetLookupSourceFields();
    }
}
