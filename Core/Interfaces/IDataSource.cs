using System;
using System.ComponentModel;
using System.Data;
using System.Threading;

namespace ScottLane.DataTidy.Core
{
    public interface IDataSource
    {
        Guid ID { get; set; }
        string Name { get; set; }
        IConnection Parent { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void UpdateParentReferences(IConnection parentConnection);
        DataColumnCollection GetDataColumns();
        DataTable GetDataTable(CancellationToken cancel, IProgress<ExecutionProgress> progress);
        DataTable GetDataTable(CancellationToken cancel);
        IDataSource Clone(bool addSuffix);
        ValidationResult Validate();
    }
}