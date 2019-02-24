using System;
using System.Threading;

namespace ScottLane.DataTidy.Core
{
    public interface IRecordCountProvider
    {
        int GetRecordCount(CancellationToken cancel, IProgress<ExecutionProgress> progress);
    }
}
