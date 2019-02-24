using System.Collections.Generic;

namespace ScottLane.DataTidy.Core
{
    public interface ISecondaryDataSourceFieldsProvider
    {
        List<DataTableField> GetSecondaryDataSourceFields();
    }
}
