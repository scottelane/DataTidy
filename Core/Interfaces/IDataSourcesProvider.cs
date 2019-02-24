using System.ComponentModel;

namespace ScottLane.DataTidy.Core
{
    public interface IDataSourcesProvider
    {
        BindingList<IDataSource> GetDataSources();
    }
}
