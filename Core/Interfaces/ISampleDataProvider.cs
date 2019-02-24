using System;
using System.Data;
using System.Threading;

namespace ScottLane.DataTidy.Core
{
    public interface ISampleDataProvider
    {
        DataTable GetSampleData(CancellationToken cancel, IProgress<ExecutionProgress> progress, int recordLimit);
    }
}
