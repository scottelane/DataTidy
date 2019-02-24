using System;
using System.Threading;

namespace ScottLane.SurgeonV2.Core
{
    public interface IAvailable
    {
        ConnectivityResult IsAvailable(CancellationToken cancel, IProgress<ExecutionProgress> progress);
    }
}
