using System;
using System.Data;

namespace ScottLane.DataTidy.Core
{
    public interface IDataColumnCreator
    {
        DataColumn CreateDataColumn(Type type);
    }
}
