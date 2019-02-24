using System;
using System.ComponentModel;

namespace ScottLane.DataTidy.Core
{
    public interface IOperation
    {
        Guid ID { get; set; }
        string Name { get; set; }
        bool ClearCache { get; set; }
        bool Enabled { get; set; }
        Batch ParentBatch { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void UpdateParentReferences(Batch parentBatch);
        BindingList<IDataSource> GetDataSources();
        IOperation Clone(bool addSuffix);
        ValidationResult Validate();
    }
}