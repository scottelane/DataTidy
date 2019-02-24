using System;
using System.Threading;

namespace ScottLane.DataTidy.Core
{
    public interface IAvailableProvider
    {
        ConnectivityResult IsAvailable(CancellationToken cancel, IProgress<ExecutionProgress> progress);
    }
}
