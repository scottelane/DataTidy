using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface IDataSourceFieldsProvider
    {
        List<DataTableField> GetDataSourceFields();
    }
}
