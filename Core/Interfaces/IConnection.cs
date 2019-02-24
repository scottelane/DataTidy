using System;
using System.ComponentModel;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Defines the interface for a connection to a server, file or other connected system.
    /// </summary>
    public interface IConnection
    {
        Guid ID { get; set; }
        string Name { get; set; }
        Project Parent { get; set; }
        BindingList<IDataSource> DataSources { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void UpdateParentReferences(Project parentProject);
        IConnection Clone();
        ValidationResult Validate();
    }
}